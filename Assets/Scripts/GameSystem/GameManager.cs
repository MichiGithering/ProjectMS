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
    public int Hp;
    public int MaxHp;

    [Header("Player Stats")]
    public float ExpandTime { get; set; }
    public float TravelDistance;
    public int StartPositionX;
    public int StartPositionY;
    public float minimumReturnFuel;

    [Header("Player Item")]
    public bool HasEmerEvac;
    public bool HasThruster;
    public bool HasObliterator;

    [Header("Game Stats")]
    public int ResearchPoints = 0;
    public enum GameState { Playing, Paused, GameOver }
    public GameState currentState = GameState.Playing;

    public static GameManager Instance { get; private set; }

    private float secondTimer = 0f;
    public bool hasGivenFuelWarning = false;

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

    public void UseItemFromCargo(int itemIndex)
    {
        switch (itemIndex)
        {
            case 0:
                HasEmerEvac = true;
                break;
            case 1:
                HasThruster = true;
                break;
            case 2:
                HasObliterator = true;
                break;
            default:
                break;
        }
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
        Time.timeScale = 1f;
        ExpandTime = 0f;
        TravelDistance = 0f;
        minimumReturnFuel = 0f;
        hasGivenFuelWarning = false;
        ResearchPoints = 0;
        secondTimer = 0f;
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
        Time.timeScale = 0f;
        Debug.Log("Game Paused!");
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed!");
    }

    public void SetPlayer(Player newlySpawnedPlayer)
    {
        player = newlySpawnedPlayer;

        StartPositionX = Mathf.RoundToInt(player.transform.position.x);
        StartPositionY = Mathf.RoundToInt(player.transform.position.y);

        TravelDistance = 0f;
        minimumReturnFuel = 0f;
        ExpandTime = 0f;

        // FIX: Instantly copy the player's initial baseline values right away
        // to prevent any 1-frame UI structural flickering during initialization loading.
        Fuel = player.Fuel;
        MaxFuel = player.MaxFuel;
        Missiles = player.Missiles;
        MaxMissiles = player.MaxMissiles;
        Hp = player.CurrentHP;
        MaxHp = player.MaxHP;
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

        // FIX: Only trigger a fuel-based defeat state if the ship's armor is still intact (Hp > 0)!
        // This allows the ship's structural explosion time frame to finish unhindered.
        if (Fuel <= 0 && Hp > 0)
        {
            GameOver(1);
        }
    }

    public void AddResearchPoints(int amount)
    {
        int AddedPoints = amount + Mathf.RoundToInt((ExpandTime / 10f) + (TravelDistance / 50f));
        ResearchPoints += AddedPoints;
    }

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

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.TransitionToScene("MainMenuScene");
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        HasEmerEvac = false;
        HasThruster = false;
        HasObliterator = false;
    }

    [Header("Run Inventory")]
    public List<SaveScoreManager.InventoryItem> CargoBag = new List<SaveScoreManager.InventoryItem>();

    public void CollectLoot(string lootName, int quantity)
    {
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