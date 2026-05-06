using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator2D : MonoBehaviour
{
    [Header("Map Bounds")]
    public float mapRadius = 500f;
    public Vector2 mapCenter = Vector2.zero;

    [SerializeField] public PolygonCollider2D cameraBound;
    [SerializeField] public CinemachineConfiner2D confiner;

    [Header("Player Spawn")]
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject evacuationPointPrefab;
    private Vector2 PlayerStartPosition = Vector2.zero;
    private GameObject playerInstance;

    //Camera setup
    [SerializeField] public CinemachineCamera cinemachineCam;
    
    [Header("Object Categories")]
    public List<SpawnCategory> planetCategories = new List<SpawnCategory>();
    public List<SpawnCategory> spaceshipCategories = new List<SpawnCategory>();

    private List<GameObject> spawnedObjects = new List<GameObject>();


    private void Start()
    {
        if (mapRadius <= 40f) mapRadius = 41f;
        GenerateMap();
    }

    public void RegenerateMap()
    {
        ClearMap();
        GenerateMap();
    }

    public void ClearMap()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    private void GenerateMap()
    {

        //Player Spawn
        if (playerInstance == null || playerInstance.Equals(null))
        {
            PlayerStartPosition = mapCenter + (Random.insideUnitCircle * 5f);
            playerInstance = Instantiate(playerPrefab,
                new Vector3(PlayerStartPosition.x, PlayerStartPosition.y, 0f),
                Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayer(playerInstance.GetComponent<Player>());
        }

        //Set Camera to follow player
        Debug.Log($"{cinemachineCam} + {playerInstance}");
        if (cinemachineCam != null && playerInstance != null)
        {
            cinemachineCam.Target.TrackingTarget = playerInstance.transform;
        }

        //Evacuation Point
        Vector2 randomOffset = Random.insideUnitCircle * 10f;
        Vector2 randomSpawnPosition = PlayerStartPosition + randomOffset;
        Instantiate(evacuationPointPrefab, new Vector3(randomSpawnPosition.x, randomSpawnPosition.y, 0f), Quaternion.identity);

        if (planetCategories != null)
        {
            foreach (SpawnCategory category in planetCategories) GenerateCategory(category);
        }

        if (spaceshipCategories != null)
        {
            foreach (SpawnCategory category in spaceshipCategories) GenerateCategory(category);
        }

        CreateCameraBound();
    }

    private void GenerateCategory(SpawnCategory category)
    {
        if (category.prefabs == null || category.prefabs.Count == 0) 
            { 
            return; 
        }

        int count = category.randomizeCount
            ? Random.Range(category.minCount, category.maxCount + 1)
            : Mathf.Max(0, category.minCount);

        for (int i = 0; i < count; i++)
        {
            TrySpawnObject(category);
        }
    }

    private void TrySpawnObject(SpawnCategory category)
    {
        for (int attempt = 0; attempt < category.maxPlacementRetries; attempt++)
        {
            Vector2 position = GetRandomPosition();

            if (category.avoidSafeZone)
            {
                int safetyCounter = 0;

                while (Vector2.Distance(position, mapCenter) < 30f && safetyCounter < 100)
                {
                    position = GetRandomPosition();
                    safetyCounter++;
                }

                if (safetyCounter >= 100) return;
            }

            if (IsPositionFree(position,category))
            {
                InstantiateObject(category, position);
                return;
            }
        }
    }

    private Vector2 GetRandomPosition()
    {
        return mapCenter + (Random.insideUnitCircle * mapRadius);
    }

    private bool IsPositionFree(Vector2 position, SpawnCategory category)
    {
        Collider2D overlap = Physics2D.OverlapCircle(position, category.overlapCheckRadius, category.overlapLayerMask);
        return overlap == null;
    }

    private void InstantiateObject(SpawnCategory category, Vector2 position)
    {
        GameObject prefab = category.GetRandomPrefab();

        if (prefab == null) 
            return;

        float scale = category.randomizeScale
            ? Random.Range(category.minScale, category.maxScale)
            : 1f;

        GameObject instance = 
        Instantiate(prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity);
        
        instance.transform.localScale = Vector3.one * scale;

        spawnedObjects.Add(instance);
        Physics2D.SyncTransforms();
    }

    private void CreateCameraBound()
    {
        int segments = 32;
        float angleStep = 360f / segments;

        Vector2 localCenter = transform.InverseTransformPoint(mapCenter);
        Vector2[] edgePoints = new Vector2[segments + 1];
        Vector2[] polyPoints = new Vector2[segments];

        // Draw the circle using math
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 pointOnCircle = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * mapRadius + localCenter;

            edgePoints[i] = pointOnCircle;
            polyPoints[i] = pointOnCircle;
        }

        // Close the EdgeCollider loop
        edgePoints[segments] = edgePoints[0];

        // 1. Apply to Physical Boundary
        EdgeCollider2D edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
        if (edgeCollider == null) edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = edgePoints;

        // 2. Apply to Cinemachine Boundary
        if (cameraBound != null)
        {
            cameraBound.points = polyPoints;
            cameraBound.isTrigger = true;

            if (confiner != null)
            {
                confiner.InvalidateBoundingShapeCache();
            }
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector3(mapCenter.x, mapCenter.y, 0f), mapRadius);

        if (cameraBound != null && cameraBound.points != null && cameraBound.points.Length > 0)
        {
            Gizmos.color = Color.yellow;
            Vector2[] points = cameraBound.points;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 currentPoint = points[i];
                Vector2 nextPoint = points[(i + 1) % points.Length];

                Vector3 worldCurrentPoint = cameraBound.transform.TransformPoint(currentPoint);
                Vector3 worldNextPoint = cameraBound.transform.TransformPoint(nextPoint);

                Gizmos.DrawLine(worldCurrentPoint, worldNextPoint);
            }
        }
    }
}

[System.Serializable]
public class WeightedPrefab
{
    public GameObject prefab;

    [Tooltip("The relative chance of this prefab spawning.")]
    [Range(0f, 100f)]
    public float weight = 10f;
}
[System.Serializable]
public class SpawnCategory
{
    public string name = "Category";

    public List<WeightedPrefab> prefabs = new List<WeightedPrefab>();

    [Header("Spawn Rules")]
    [Tooltip("If true, this object will not spawn within 30 units of the center.")]
    public bool avoidSafeZone = true;

    [Header("Count")]
    public bool randomizeCount = true;
    public int minCount = 10;
    public int maxCount = 20;

    [Header("Scale")]
    public bool randomizeScale = false;
    [Range(1f, 10f)]
    public float minScale = 1f;
    [Range(1f, 10f)]
    public float maxScale = 1f;

    [Header("Overlap Rules")]
    public LayerMask overlapLayerMask;
    public float overlapCheckRadius = 50f;
    public int maxPlacementRetries = 100;

    public GameObject GetRandomPrefab()
    {
        if (prefabs == null || prefabs.Count == 0) return null;

        float totalWeight = 0f;
        foreach (WeightedPrefab wp in prefabs)
        {
            totalWeight += wp.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (WeightedPrefab wp in prefabs)
        {
            currentWeight += wp.weight;
            if (randomValue <= currentWeight)
            {
                return wp.prefab;
            }
        }

        return prefabs[0].prefab;
    }
}