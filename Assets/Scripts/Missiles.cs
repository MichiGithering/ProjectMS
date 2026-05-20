using UnityEngine;

public class Missiles : Entity
{
    [Header("Missile")]
    [SerializeField] private staticMovement movementScript;
    [SerializeField] public MissileConfig _missileConfig;

    // REMOVED ImpactEffect variable (it inherits it from Entity!)

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        movementScript = GetComponent<staticMovement>();
    }

    // -------------------------------------------------------------------------
    // Collision
    // -------------------------------------------------------------------------

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Debug.Log($"Missile hit: {collision.gameObject.name} | Tag: {collision.tag}");
        Planet hitPlanet = collision.GetComponent<Planet>();

        if (hitPlanet != null)
        {
            if (!hitPlanet.Mirage)
            {
                Debug.Log("Missile exploded on a solid planet!");
                Explode();
            }
            return;
        }

        // Hit an enemy (Pirate, Debris, etc.)
        Entity hitEntity = collision.GetComponent<Entity>();
        if (hitEntity != null && !collision.CompareTag("Player"))
        {
            int damage = _missileConfig != null
                ? Mathf.RoundToInt(_missileConfig.Damage)
                : 1;

            hitEntity.TakeDamage(damage);
            Explode();
        }
    }
}