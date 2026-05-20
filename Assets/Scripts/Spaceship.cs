using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Spaceship : Entity
{
    [Header("Resources")]
    public float Fuel;
    public float MaxFuel;
    public int Missiles;
    public int MaxMissiles;
    public float FuelConsumptionRate = 1f;
    [SerializeField] public GameObject missilePrefab;
    [SerializeField] public ParticleSystem thrusterPrefab;

    protected ParticleSystem activeThruster;
    protected override void Awake()
    {
        base.Awake();

        if (_entityConfig != null)
        {
            MaxFuel = _entityConfig.MaxFuel;
            MaxMissiles = _entityConfig.MaxMissiles;
        }
    }
    public override void Start()
    {
        base.Start();

        if (thrusterPrefab != null && activeThruster == null)
        {
            activeThruster = Instantiate(thrusterPrefab, transform);

            activeThruster.transform.localPosition = new Vector3(0, -1f, 0);

            activeThruster.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        }
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected virtual void LaunchMissile()
    {
        if (Missiles > 0)
        {
            if (missilePrefab != null)
            {
                Instantiate(missilePrefab, transform.position, transform.rotation);

                Debug.Log("Missile launched!");
                Missiles--;
            }
            else
            {
                Debug.LogWarning("Missile Prefab is not assigned in the Inspector!");
            }
        }
        else
        {
            Debug.Log("No missiles left!");
        }
    }
    
    public virtual void FixedUpdate()
    {
        if (Fuel > 0)
        {
            Fuel -= Time.deltaTime * FuelConsumptionRate;
        }
        else if (Fuel <= 0 && activeThruster != null)
        {
            activeThruster.Stop();
        }
    }
}

