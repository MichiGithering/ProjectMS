using UnityEngine;

public class Pirate : Spaceship
{
    [Header("Pirate")]
    public Movement movementScript;

    [Header("AI Tracking")]
    public Transform target;

    public override void Start()
    {
        base.Start();

        // Find the player automatically if not assigned
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        Fuel = UnityEngine.Random.Range(MaxFuel/3 , MaxFuel);

        // Auto-assign movement script just in case you forgot to drag it in
        if (movementScript == null)
        {
            movementScript = GetComponent<Movement>();
        }
    }

    public override void FixedUpdate()
    {
        // Drains fuel automatically by calling Spaceship's FixedUpdate
        base.FixedUpdate();

        // The "Robot Brain" logic
        if (target != null && Fuel > 0 && movementScript != null && movementScript.enabled)
        {
            // Calculate the exact direction to the target
            Vector2 direction = (target.position - transform.position).normalized;

            // THE TRICK: We send 0f first to unlock your Movement script's "if" statement, 
            // then immediately send the actual direction!
            movementScript.MoveHorizontal(0f);
            movementScript.MoveHorizontal(direction.x);

            movementScript.MoveVertical(0f);
            movementScript.MoveVertical(direction.y);

            // Make the Pirage rotate to look at what it's chasing
            FaceTarget(direction);
        }
        else if (movementScript != null)
        {
            // If it loses the target or runs out of fuel, send 0 to stop engines
            movementScript.MoveHorizontal(0f);
            movementScript.MoveVertical(0f);
        }
    }

    private void FaceTarget(Vector2 direction)
    {
        // Smooth rotation math so it turns naturally towards the player
        float rotationSpeed = 450f;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90f);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Player hitPlayer = collision.GetComponent<Player>();

        // If the thing we bumped into has a Player script...
        if (hitPlayer != null)
        {
            //This is protoype
            hitPlayer.Fuel -= 20; // Steal some fuel from the player!
            Destroy(gameObject); // Then self-destruct in a kamikaze attack
        }
    }
}