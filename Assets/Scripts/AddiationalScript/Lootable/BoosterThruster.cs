using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoosterThruster : MonoBehaviour
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
        if (GameManager.Instance.HasThruster)
        {
            ShowUI();
            SaveScoreManager.Instance.RemoveItemFromInventory("1_Thruster", 1);
        }
    }
    public void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.PlayerControl.Thruster.started += OnThrusterPerformed;
        playerInputActions.PlayerControl.Thruster.canceled += OnThrusterCanceled;
    }

    public void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.PlayerControl.Thruster.started -= OnThrusterPerformed;
            playerInputActions.PlayerControl.Thruster.canceled -= OnThrusterCanceled;
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

    private void OnThrusterPerformed(InputAction.CallbackContext context)
    {
        if (Player.Instance != null && Player.Instance.movementScript != null)
        {
            Player.Instance.movementScript.ApplyBoosterThruster();
        }
    }

    private void OnThrusterCanceled(InputAction.CallbackContext context)
    {
        if (Player.Instance != null && Player.Instance.movementScript != null)
        {
            Player.Instance.movementScript.StopBoosterThruster();
        }
    }
}