using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Player Resources")]
    public float Fuel;
    public float MaxFuel;
    public int Missiles;
    public int MaxMissiles;

    [Header("Player Stats")]
    public float ExpandTime { get; set; }
    public float TravelDistance;
    public int StartPositionX;
    public int StartPositionY;
    public float minimumReturnFuel;

    [Header("Game Stats")]
    public int ResearchPoints = 0;
    public enum GameState { Playing, Paused, GameOver }
    public GameState currentState = GameState.Playing;

    public static GameManager Instance { get; private set; }

    private float secondTimer = 0f;
    private bool hasGivenFuelWarning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ExpandTime = 0f;
        TravelDistance = 0f;

        if (player != null)
        {
            StartPositionX = Mathf.RoundToInt(player.transform.position.x);
            StartPositionY = Mathf.RoundToInt(player.transform.position.y);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameplayScene")
        {
            ResetForNewRun();
        }
    }

    public void ResetForNewRun()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;          // ? THIS is why player was frozen!
        ExpandTime = 0f;
        TravelDistance = 0f;
        minimumReturnFuel = 0f;
        hasGivenFuelWarning = false;
        ResearchPoints = 0;
        CargoBag.Clear();
        player = null;
    }

    private void Update()
    {
        if (currentState != GameState.Playing)
            return;

        PlayerStatsUpdate();
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f; // Freezes Unity's physics and time
        Debug.Log("Game Paused!");

        // UI Manager to show the Pause Menu
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f; // Unfreezes Unity
        Debug.Log("Game Resumed!");

        // UI Manager to hide the Pause Menu
    }
    
    public void SetPlayer(Player newlySpawnedPlayer)
    {
        player = newlySpawnedPlayer;

        StartPositionX = Mathf.RoundToInt(player.transform.position.x);
        StartPositionY = Mathf.RoundToInt(player.transform.position.y);

        TravelDistance = 0f;
        minimumReturnFuel = 0f;
        ExpandTime = 0f;
    }

    private void PlayerStatsUpdate()
    {
        if (player != null)
        {
            ExpandTime += Time.deltaTime;

            secondTimer += Time.deltaTime;
            if (secondTimer >= 1f)
            {
                TravelDistance = Vector2.Distance(new Vector2(StartPositionX, StartPositionY), player.transform.position);
                minimumReturnFuel += (TravelDistance * 0.01f) + (ExpandTime / 500);
                secondTimer -= 1f;
            }
        }

        if (Fuel < minimumReturnFuel && !hasGivenFuelWarning)
        {
            Debug.Log("Warning: Fuel is below the minimum required to return!");
            hasGivenFuelWarning = true;
        }

        if (Fuel <= 0)
        {
            GameOver(1); // Ran out of fuel
        }
    }
    public void AddResearchPoints(int amount)
    {
        int AddedPoints = amount + Mathf.RoundToInt((ExpandTime / 10f) + (TravelDistance / 50f));
        ResearchPoints += AddedPoints;
    }
    // Game over: 0 = evacuate, 1 = ran out of fuel, 2 = hit an obstacle, 3 = other
    public void GameOver(int context)
    {
        currentState = GameState.GameOver;

        if (context == 0)
        {
            Debug.Log("You successfully evacuated! Congratulations!");


            if (SaveScoreManager.Instance != null)
            {
                SaveScoreManager.Instance.SaveRun(TravelDistance, ResearchPoints, CargoBag);
            }


        }
        else
        {

            if (context == 1)
            {
                Debug.Log("You ran out of fuel and couldn't return to the station. Better luck next time!");

            }
            else if (context == 2)
            {
                Debug.Log("You hit an obstacle and your ship was destroyed. Be more careful next time!");
            }
            else
            {
                Debug.Log("Your ship was lost due to unforeseen circumstances. Try again!");
            }

            if (SaveScoreManager.Instance != null)
            {
                SaveScoreManager.Instance.SaveRun(TravelDistance, 0, null);
            }
        }

        SceneManager.LoadScene("MainMenuScene");
    }

    [Header("Run Inventory")]
    public List<SaveScoreManager.InventoryItem> CargoBag = new List<SaveScoreManager.InventoryItem>();

    // Call this when the player investigates and successfully collects loot. It will add the loot to the Cargo Bag, stacking it if it's already there.
    public void CollectLoot(string lootName, int quantity)
    {
        // Check if it's already in the bag to stack it
        SaveScoreManager.InventoryItem existingLoot = CargoBag.Find(item => item.ItemName == lootName);

        if (existingLoot != null)
        {
            existingLoot.Quantity += quantity;
        }
        else
        {
            CargoBag.Add(new SaveScoreManager.InventoryItem(quantity, lootName));
        }

        Debug.Log($"Collected {quantity} {lootName}! It is in the Cargo Bag.");
    }
}