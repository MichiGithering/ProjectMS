using UnityEngine;
using UnityEngine.InputSystem;

public class EvacuationPoint : InteractableObject
{    
    private bool IsPlayerInRange = false;

    // Initialization
    protected override void Awake()
    {
        base.Awake();
    }

    // Input Handling
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void InteractionActive()
    {
        if(IsPlayerInRange)
        {
            GameManager.Instance.currentState = GameManager.GameState.GameOver;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            Debug.Log("Player reached the Evacuation Point!");
            IsPlayerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision) {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Player left the Evacuation Point!");
            IsPlayerInRange = false;
        }
    }
}
