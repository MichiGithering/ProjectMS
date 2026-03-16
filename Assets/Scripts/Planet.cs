using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Planet : Object
{

    [Header("Planet")]

    [SerializeField] public RewardConfig _rewardConfig;

    [Header("Reward")]
    private float ReFuel;
    private int ReMissile;
    protected override void Awake()
    {
        base.Awake();
        if(_rewardConfig == null && Mirage == true)
        {
            _rewardConfig = new RewardConfig();
        }
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D (collision);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {

        }
    }
}
