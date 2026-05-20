using UnityEngine;

public class Pirate : Spaceship
{
    [Header("Pirate")]
    public Movement movementScript;

    [Header("AI Tracking")]
    public Transform target;
    public float ChaseRange = 20f;

    [Header("Damage")]
    [SerializeField] private float ramFuelDrain = 20f; // How much fuel stolen on ram

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    public override void Start()
    {
        base.Start();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        Fuel = Random.Range(MaxFuel / 3f, MaxFuel);

        if (movementScript == null)
            movementScript = GetComponent<Movement>();
    }

    // -------------------------------------------------------------------------
    // AI Update
    // -------------------------------------------------------------------------

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float distanceToTarget = target != null
            ? Vector2.Distance(transform.position, target.position)
            : Mathf.Infinity;

        if (distanceToTarget <= ChaseRange && Fuel > 0 && movementScript != null && movementScript.enabled)
        {
            Vector2 direction = (target.position - transform.position).normalized;

            // Reset then set — required by Movement script's input guard
            movementScript.MoveHorizontal(0f);
            movementScript.MoveHorizontal(direction.x);

            movementScript.MoveVertical(0f);
            movementScript.MoveVertical(direction.y);

            FaceTarget(direction);
        }
        else if (movementScript != null)
        {
            movementScript.MoveHorizontal(0f);
            movementScript.MoveVertical(0f);
        }
    }

    private void FaceTarget(Vector2 direction)
    {
        float rotationSpeed = 450f;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle - 90f);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
    }

    // -------------------------------------------------------------------------
    // Collision — Ram Attack
    // -------------------------------------------------------------------------

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Player playerScript = collision.GetComponent<Player>();
        if (playerScript != null)
        {
            // Flash the PLAYER to show they got hit
            playerScript.HitFlash();

            // Flash self before dying
            HitFlash();

            // Steal fuel from the player (prototype)
            playerScript.Fuel -= ramFuelDrain;

            // Pirate self-destructs on ram
            OnDeath();
        }
    }

    // -------------------------------------------------------------------------
    // Death Override
    // -------------------------------------------------------------------------

    protected override void OnDeath()
    {
        Debug.Log($"{name} destroyed!");
        // You can add loot drop, explosion effect, etc. here later
        Destroy(gameObject);
    }

    // -------------------------------------------------------------------------
    // Editor Gizmo
    // -------------------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }
}