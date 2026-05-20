using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Debris : Entity
{
    [Header("Debris Visuals")]
    [Tooltip("Drag your different debris sprites into this list in the Inspector!")]
    [SerializeField] private Sprite[] randomSprites;

    private int damage = 1;

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        PickRandomSprite();
        PickRandomDirection();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Player playerScript = collision.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
            OnDeath();
        }
    }

    // -------------------------------------------------------------------------
    // Visuals & Movement
    // -------------------------------------------------------------------------

    private void PickRandomSprite()
    {
        if (randomSprites != null && randomSprites.Length > 0)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                int randomIndex = Random.Range(0, randomSprites.Length);
                spriteRenderer.sprite = randomSprites[randomIndex];
            }
        }
        else
        {
            Debug.LogWarning($"Debris '{name}' doesn't have any sprites assigned in its Random Sprites array!");
        }
    }

    private void PickRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randomAngle);
    }

    // -------------------------------------------------------------------------
    // Death Override
    // -------------------------------------------------------------------------

    protected override void OnDeath()
    {
        Explode();
    }
}