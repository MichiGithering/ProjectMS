using UnityEngine;
using Unity.Collections;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class InvestigatePlanet : InteractableObject
{
    private GameObject effectInstance;

    private Planet planetTarget;
    private Player currentPlayer;
    private LootChest lootChest;

    private int investigatingValue;

    private Coroutine activeTimer;

    // Safety lock to prevent getting the reward twice!
    private bool alreadyLooted = false;

    protected override void Awake()
    {
        base.Awake();
        investigatingValue = 0;
        planetTarget = GetComponent<Planet>();
        lootChest = GetComponent<LootChest>();

        if (planetTarget == null)
        {
            Debug.LogError($"InvestigatePlanet on {gameObject.name} requires a Planet component!");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyLooted) return;

        // 1. Ensure it is actually the Player before doing ANYTHING!
        if (collision.CompareTag("Player"))
        {
            currentPlayer = collision.GetComponent<Player>();

            if (currentPlayer != null)
            {

                // Option A: Turn ON the button listener immediately
                SetInteractable(true);

                // Option B: Start the automatic timer
                activeTimer = StartCoroutine(Investigating());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !alreadyLooted)
        {

            if (activeTimer != null)
            {
                StopCoroutine(activeTimer);
                activeTimer = null;
            }

            investigatingValue = 0;

            SetInteractable(false);
            currentPlayer = null;
        }
    }

    // 2. FIX: Use a loop instead of recursion!
    private IEnumerator Investigating()
    {
        // Keep looping as long as the value is less than 5
        while (investigatingValue < 5)
        {
            planetTarget.HitFlash();
            yield return new WaitForSeconds(1f);

            investigatingValue += 1;
        }

        // If the loop finishes, it means 4 seconds passed! Give the reward.
        GiveReward();
    }

    protected override void InteractionActive()
    {
        GiveReward();
    }

    private void GiveReward()
    {
        if (alreadyLooted || currentPlayer == null) return;

        alreadyLooted = true;

        if (activeTimer != null)
        {
            StopCoroutine(activeTimer);
            activeTimer = null;
        }

        SetInteractable(false);

        planetTarget.GetReward(currentPlayer);
        
        if (lootChest != null)
        {
            lootChest.Open();
        }
    }
}