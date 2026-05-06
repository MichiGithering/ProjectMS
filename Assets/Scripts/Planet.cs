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
    }
    public void GetReward(Player playerScript)
    {
        if (HasReward && !Mirage)
        {
            playerScript.Fuel += ReFuel;
            playerScript.Missiles += ReMissile;

            // Turn off the collider so they can fly through the empty husk
            GetComponent<Collider2D>().enabled = false;
            HasReward = false;

            Debug.Log($"Collected {ReFuel} Fuel and {ReMissile} Missiles from Planet!");
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);

            GameManager.Instance.AddResearchPoints(_rewardConfig.RewardPoints);
        }

    }
}