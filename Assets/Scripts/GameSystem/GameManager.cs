using UnityEngine;

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
    public int Score;
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
    
    //In case of player spawning instead of placing in editor
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
            Time.timeScale = 0f; // Freeze the game when they die
        }
    }

    //Game over : 0 = evacuate , 1 = ran out of fuel , 2 = hit an obstacle , 3 = other
    public void GameOver(int context)
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f; // Freeze the game

        Debug.Log("Game Over! Context: " + context  );
    }
}