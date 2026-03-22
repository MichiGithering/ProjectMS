using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 100f;
    public float acceleration = 30f;
    public float deceleration = 35f;

    [Header("Fuel Handling")]
    private Spaceship spaceship;
    private float RemainFuel;
    private bool HasFuel = true;

    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;

        //Connect to config
        EntityConfig entityConfig = GetComponent<Entity>()?._entityConfig;
        if (entityConfig != null)
        {
            moveSpeed = entityConfig.MaxSpeed;
        }

        spaceship = GetComponent<Spaceship>();
    }

    private void FixedUpdate()
    {
        
        if(spaceship != null)
        {
            RemainFuel = spaceship.Fuel;
            HasFuel = RemainFuel > 0;
            if(HasFuel)
            {
                ApplyMovement();
            }
        }
        else
        {
            ApplyMovement();
        }
    }

    public void MoveHorizontal(float input)
    {
        if (moveInput.x == 0 || input == 0 || input == -moveInput.x)
        {
            moveInput.x = input;
        }
    }
    public void MoveVertical(float input)
    {
        if (moveInput.y == 0 || input == 0 || input == -moveInput.y)
        {
            moveInput.y = input;
        }
    }

    public void ApplyMovement()
    {        
        // 1. Calculate Target Velocity (Normalized so diagonals aren't too fast)
        Vector2 targetVelocity = moveInput.normalized * moveSpeed;

        // 2. Determine if we are speeding up or slowing down
        float speedChange = moveInput.magnitude > 0 ? acceleration : deceleration;

        // 3. Smoothly interpolate current velocity toward target
        float newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelocity.x, speedChange * Time.fixedDeltaTime);
        float newVelY = Mathf.MoveTowards(rb.linearVelocity.y, targetVelocity.y, speedChange * Time.fixedDeltaTime);

        // 4. Apply to Rigidbody with max speed limit
        Vector2 newVelocity = new Vector2(newVelX, newVelY);
        if (newVelocity.magnitude > moveSpeed)
        {
            newVelocity = newVelocity.normalized * moveSpeed;
        }
            rb.linearVelocity = newVelocity;
    }
}