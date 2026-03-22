using UnityEngine;

public class Planet : Objects
{
    [Header("Planet")]
    [SerializeField] public RewardConfig _rewardConfig;

    [Header("Reward")]
    public bool HasReward = true;
    private float ReFuel;
    private int ReMissile;

    protected override void Awake()
    {
        base.Awake();

        if (_rewardConfig == null)
        {
            Debug.LogWarning($"Planet {gameObject.name} is missing a RewardConfig!");
            _rewardConfig = ScriptableObject.CreateInstance<RewardConfig>();
        }

        // 2. Set up the rewards safely
        if (!Mirage)
        {
            HasReward = true;
            ReFuel = _rewardConfig.ReFuel;
            ReMissile = _rewardConfig.ReMissile;
        }
        else
        {
            HasReward = false;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Player playerScript = collision.GetComponent<Player>();

        if (playerScript != null && HasReward)
        {
                playerScript.Fuel += ReFuel;
                playerScript.Missiles += ReMissile;

            GetComponent<Collider2D>().enabled = false;
            HasReward = false;

                Debug.Log($"Collected {ReFuel} Fuel and {ReMissile} Missiles from Planet!");


            // Prototype: Change the planet's appearance to indicate it's been collected
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f); // Turns it gray
        }
    }
}