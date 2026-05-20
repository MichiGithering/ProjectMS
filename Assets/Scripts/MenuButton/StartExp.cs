using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartExp : MonoBehaviour
{
    private ProjectMSInputAction playerInputActions;
    [SerializeField] private GameObject player;
    private void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
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
        if(SceneTransition.Instance != null)
        {
            SceneTransition.Instance.TransitionToScene("MainGameplayScene");
        }
        else
        {
            SceneManager.LoadScene("MainGameplayScene");
        }

        if (player != null)
        {
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocity = Vector2.up;
            }
        }
    }
}