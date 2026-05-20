using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EmergencyEvacuation : MonoBehaviour
{
    private ProjectMSInputAction playerInputActions;

    private Image uiImage;

    public void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
        uiImage = GetComponent<Image>();

        HideUI();
    }
    public void Start()
    {
        if (GameManager.Instance.HasEmerEvac)
        {
            ShowUI();
            SaveScoreManager.Instance.RemoveItemFromInventory("0_EmerEvac", 1);
        }
    }
    public void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.PlayerControl.EmerEvac.started += OnEvacStarted;
        playerInputActions.PlayerControl.EmerEvac.performed += OnEvacPerformed;
        playerInputActions.PlayerControl.EmerEvac.canceled += OnEvacCanceled;
    }

    public void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.PlayerControl.EmerEvac.started -= OnEvacStarted;
            playerInputActions.PlayerControl.EmerEvac.performed -= OnEvacPerformed;
            playerInputActions.PlayerControl.EmerEvac.canceled -= OnEvacCanceled;
            playerInputActions.Disable();
        }
    }

    public void ShowUI()
    {
        if (uiImage != null)
        {
            uiImage.enabled = true;
            uiImage.raycastTarget = true; 
        }
    }

    public void HideUI()
    {
        if (uiImage != null)
        {
            uiImage.enabled = false; 
            uiImage.raycastTarget = false;  
        }
    }


    private void OnEvacStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Evacuation started... hold it!");
    }

    private void OnEvacPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Evacuation successful! Blasting off!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver(0);
        }
    }

    private void OnEvacCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Evacuation canceled or released.");
    }
}