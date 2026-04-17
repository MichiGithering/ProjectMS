using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EvacuationPoint : InteractableObject
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered the Evacuation Point trigger area.");
            SetInteractable(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited the Evacuation Point trigger area.");
            SetInteractable(false);
        }
    }
    protected override void InteractionActive()
    {
        Debug.Log("Player reached the Evacuation Point! Evacuation sequence triggered!");

        GameManager.Instance.GameOver(0); // Evacuation
        Time.timeScale = 0f;
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