using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    private ProjectMSInputAction playerInputActions;

    protected bool canInteract = false;

    protected virtual void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
    }

    protected virtual void OnEnable()
    {
        playerInputActions.PlayerControl.Interaction.performed += OnInteractionPerformed;
    }

    protected virtual void OnDisable()
    {
        playerInputActions.Disable();
        playerInputActions.PlayerControl.Interaction.performed -= OnInteractionPerformed;
    }

    protected void SetInteractable(bool isInteractable)
    {
        canInteract = isInteractable;

        if (canInteract) playerInputActions.Enable();
        else playerInputActions.Disable();
    }

    protected void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        if (canInteract)
        {
            InteractionActive();
        }
    }

    protected virtual void InteractionActive()
    {
        Debug.Log($"{gameObject.name} was interacted with!");
    }
}