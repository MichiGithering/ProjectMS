using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    private GameManager gameManager;
    private ProjectMSInputAction playerInputActions;

    // Initialization
    protected virtual void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
        gameManager = GameManager.Instance;
    }

    // Input Handling
    protected virtual void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.PlayerControl.LaunchMissile.performed += OnInteractionPerformed;
    }

    protected virtual void OnDisable()
    {
        playerInputActions.Disable();

        playerInputActions.PlayerControl.LaunchMissile.performed -= OnInteractionPerformed;
    }

    protected void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        InteractionActive();
    }

    protected virtual void InteractionActive()
    {
    }
}