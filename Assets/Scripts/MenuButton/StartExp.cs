using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartExp : MonoBehaviour
{
    private ProjectMSInputAction playerInputActions;

    private void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
    }

    public void StartExpedition()
    {
        Debug.Log("Starting Expedition... Loading Main Gameplay Scene!");

        SceneManager.LoadScene("MainGameplayScene");
    }

    private void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.GameControl.MenuExpedition.performed += OnStartExpeditionPerformed;
    }

    private void OnDisable()
    {
        playerInputActions.GameControl.MenuExpedition.performed -= OnStartExpeditionPerformed;

        playerInputActions.Disable();
    }

    private void OnStartExpeditionPerformed(InputAction.CallbackContext context)
    {
        StartExpedition();
    }
}