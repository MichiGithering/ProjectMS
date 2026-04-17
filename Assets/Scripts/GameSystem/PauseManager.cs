using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    private ProjectMSInputAction playerInputActions;

    private void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.GameControl.Pause.performed += PauseToggle;
    }

    private void OnDisable()
    {
        playerInputActions.Disable();

        playerInputActions.GameControl.Pause.performed -= PauseToggle;
    }

    private void PauseToggle(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePause();
    }
}