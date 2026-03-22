using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class staticMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float acceleration = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = (Vector2)transform.up * moveSpeed;

        Vector2 newVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = newVelocity;
    }
}