using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MirageObliterator : MonoBehaviour
{
    [SerializeField] public GameObject burstEffect;

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
        if(GameManager.Instance.HasObliterator)
        {
                ShowUI();
            SaveScoreManager.Instance.RemoveItemFromInventory("2_Obliterator", 1);
        }
    }

    public void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.PlayerControl.Obliterator.started += OnBurstPerformed;
    }

    public void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.PlayerControl.Obliterator.started -= OnBurstPerformed;
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

    private void OnBurstPerformed(InputAction.CallbackContext context)
    {
        if (Player.Instance == null)
        {
            Debug.LogWarning("MirageObliterator: Cannot activate because Player.Instance is missing!");
            return;
        }

        if (burstEffect == null)
        {
            Debug.LogWarning("MirageObliterator: Cannot activate because 'burstEffect' is unassigned in the Inspector!");
            return;
        }

        Instantiate(burstEffect, Player.Instance.transform.position, Quaternion.identity);
        Debug.Log("Mirage Obliterator activated! Burst effect instantiated.");
    }
}