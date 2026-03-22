using UnityEngine;

public class Missiles : Entity
{
    [Header("Missiles")]
    private staticMovement movementScript;
    [SerializeField] public MissileConfig _missileConfig;

    protected override void Awake()
    {
        base.Awake();
        movementScript = GetComponent<staticMovement>();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Planet hitPlanet = collision.GetComponent<Planet>();

        if (hitPlanet != null)
        {
            if (!hitPlanet.Mirage)
            {
                Debug.Log("Missile exploded on a solid planet!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Missile flew right through a Mirage!");
            }
        }

        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Missile hit an enemy!");
            Destroy(gameObject);
        }
    }
}