using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EvacuationPoint : InteractableObject
{

    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.rotation = Random.Range(0f, 360f);

        float spinSpeed = Random.Range(1f, 8f);
        float direction = Random.value > 0.5f ? 1f : -1f;

        rb.angularVelocity = spinSpeed * direction;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetInteractable(true);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.EnterEvacZone();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetInteractable(false);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ExitEvacZone();
            }
        }
    }
    protected override void InteractionActive()
    {
        if(GameManager.Instance.minimumReturnFuel > GameManager.Instance.Fuel)
        {
            Debug.Log("Not enough fuel to evacuate! Minimum return fuel required: " + GameManager.Instance.minimumReturnFuel);
            return;
        }
        Debug.Log("Player reached the Evacuation Point! Evacuation sequence triggered!");

        GameManager.Instance.GameOver(0); // Evacuation
    }

    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.green;

            if (col is CircleCollider2D circle)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
            }
        }
    }
}