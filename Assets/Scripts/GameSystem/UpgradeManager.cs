using UnityEngine;

public class UpgradeAncInventoryManager : MonoBehaviour

{   [SerializeField] private Canvas UpgradeCanvas;
    [SerializeField] private Canvas CargoCanvas;

    private bool isUpgradedConfirmed = false;

    private ProjectMSInputAction playerInputActions;
    void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
        playerInputActions.GameControl.MenuUpgrade.performed += ctx => ToggleUpgrade();
        playerInputActions.GameControl.MenuCargo.performed += ctx => ToggleCargo();
    }
    void Start()
    {
        if (UpgradeCanvas != null) UpgradeCanvas.enabled = false;
        if (CargoCanvas != null) CargoCanvas.enabled = false;
    }
    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
    private void ToggleUpgrade()
    {
        if(CargoCanvas != null)
            CargoCanvas.enabled = false;

        if (UpgradeCanvas != null)
        {
            UpgradeCanvas.enabled = !UpgradeCanvas.enabled;
        }
    }
    private void ToggleCargo()
    {
        if(UpgradeCanvas != null)
            UpgradeCanvas.enabled = false;

        if (CargoCanvas != null)
        {
            CargoCanvas.enabled = !CargoCanvas.enabled;
        }
    }

    private void ConfirmUpgrade()
    {
        isUpgradedConfirmed = true;
        SaveScoreManager.Instance.AddItemToInventory("Upgrade", 1);
    }
}
