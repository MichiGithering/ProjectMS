using System.Runtime.CompilerServices;
using TMPro;
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

        // Get player start position from Player, if not exist, set to 0
        if (player != null)
        {
            StartPositionX = Mathf.RoundToInt(player.transform.position.x);
            StartPositionY = Mathf.RoundToInt(player.transform.position.y);
        }
        else
        {
            Debug.Log("Player not found in GameManager.");
            StartPositionX = 0;
            StartPositionY = 0;
        }
    }

    private void Update()
    {
        PlayerStatsUpdate();
    }
    private void PlayerStatsUpdate()
    {
        if (player != null)
        {
            ExpandTime += Time.deltaTime;
            TravelDistance = Vector2.Distance(new Vector2(StartPositionX, StartPositionY), player.transform.position);
            
            if(ExpandTime % 1f < 0.01f)
            {
                minimumReturnFuel += (TravelDistance * 0.01f) + (ExpandTime / 500);
            }
        }
        if(Fuel < minimumReturnFuel)
        {
            Debug.Log("Warning: Fuel is below the minimum required to return!");
        }
        if(Fuel <= 0)
        {
            Debug.Log("Game Over: You ran out of fuel!");
        }
    }

}



