using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : Objects
{

    [Header("Entity")]
    [SerializeField] public EntityConfig _entityConfig;

    protected override void Awake()
    {
        base.Awake();
        if (_entityConfig == null)
        {
            _entityConfig = ScriptableObject.CreateInstance<EntityConfig>();
        }
        MaxMovementSpeed = _entityConfig.MaxSpeed;
        MaxHP = _entityConfig.MaxHP;

    }
    public override void Start()
    {
        base.Start();
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}

