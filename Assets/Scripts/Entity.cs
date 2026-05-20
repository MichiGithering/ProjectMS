using UnityEngine;

public class Entity : Objects
{
    [Header("Entity")]
    [SerializeField] public EntityConfig _entityConfig;

    // This is defined ONCE here, and all child classes will inherit it!
    [SerializeField] public GameObject ImpactEffect;

    [Header("Health")]
    public int CurrentHP;

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();

        if (_entityConfig == null)
        {
            _entityConfig = ScriptableObject.CreateInstance<EntityConfig>();
        }

        MaxMovementSpeed = _entityConfig.MaxSpeed;
        MaxHP = _entityConfig.MaxHP;
        CurrentHP = MaxHP;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    // -------------------------------------------------------------------------
    // Damage & Death
    // -------------------------------------------------------------------------

    public virtual void TakeDamage(int amount)
    {
        if (CurrentHP <= 0) return; // Already dead — ignore extra hits

        CurrentHP -= amount;
        CurrentHP = Mathf.Max(0, CurrentHP);

        Debug.Log($"{name} took {amount} damage. HP: {CurrentHP} / {MaxHP}");

        HitFlash(); // Visual feedback on every hit

        if (CurrentHP <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        Explode();
    }

    protected virtual void Explode()
    {
        if (ImpactEffect != null)
        {
            Instantiate(ImpactEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}