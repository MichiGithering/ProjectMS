using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : Object
{

    [Header("Entity")]
    [SerializeField] public EntityConfig _entityConfig;
    [SerializeField] private Movement movementScript;
    protected override void Awake()
    {
        base.Awake();
        if (_entityConfig == null)
        {
            _entityConfig = new EntityConfig();
        }
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}

