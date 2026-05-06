using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InvestigatePlanet : InteractableObject
{
    private Planet planetTarget;
    private Player currentPlayer;
    private LootChest lootChest;
    protected override void Awake()
    {
        base.Awake();
        planetTarget = GetComponent<Planet>();
        lootChest = GetComponent<LootChest>();
        if (planetTarget == null)
        {
            Debug.LogError($"InvestigatePlanet on {gameObject.name} requires a Planet component!");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            currentPlayer = collision.GetComponent<Player>();

            if (currentPlayer != null)
            {
                Debug.Log("Player entered the Investigatable Planet trigger area.");
                SetInteractable(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited the Investigatable Planet trigger area.");
            SetInteractable(false);

            currentPlayer = null;
        }
    }

    protected override void InteractionActive()
    {
        Debug.Log("Investigation sequence triggered!");

        // Pass the remembered player into the reward function!
        if (currentPlayer != null)
        {
            planetTarget.GetReward(currentPlayer);

            if (lootChest != null)
                lootChest.Open();
        }
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