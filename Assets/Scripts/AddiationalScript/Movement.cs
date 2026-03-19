using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;

    // Public property so your "other script" can easily set this
    [HideInInspector] public float moveInput = 0f;
    public float moveSpeed = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Apply the moveInput to the Y axis for vertical movement
        // We keep the current X velocity to avoid freezing horizontal physics
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, moveInput * moveSpeed, rb.linearVelocity.z);
    }
}