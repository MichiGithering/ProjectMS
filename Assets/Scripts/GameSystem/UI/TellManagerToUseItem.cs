using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class TellManagerToUseItem : MonoBehaviour
{
    private ProjectMSInputAction inputActions;
    [SerializeField] public UseItemFromCargo EmerEvacButton;
    [SerializeField] public UseItemFromCargo ThrusterButton;
    [SerializeField] public UseItemFromCargo ObliteratorButton;
    private void Awake()
    {
        inputActions = new ProjectMSInputAction();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.GameControl.UseEmerEvac.performed += ctx => TellManagerToUseEmerEvacAction();
        inputActions.GameControl.UseThruster.performed += ctx => TellManagerToUseBoosterAction();
        inputActions.GameControl.UseObliterator.performed += ctx => TellManagerToUseObliteratorAction();

    }

    private void OnDisable()
    {
        inputActions.GameControl.UseEmerEvac.performed -= ctx => TellManagerToUseEmerEvacAction();
        inputActions.GameControl.UseThruster.performed -= ctx => TellManagerToUseBoosterAction();
        inputActions.GameControl.UseObliterator.performed -= ctx => TellManagerToUseObliteratorAction();
        inputActions.Disable();

    }
    private void Start()
    {
        CheckItemQuantity();
    }
    private void CheckItemQuantity()
    {
        SaveScoreManager.PlayerProfileData profile = SaveScoreManager.Instance.LoadGame();

        if (profile != null && profile.SavedItems != null)
        {
            // 2. Safely search for the items by their exact string names
            var emerEvac = profile.SavedItems.Find(item => item.ItemName == "0_EmerEvac");
            var thruster = profile.SavedItems.Find(item => item.ItemName == "1_Thruster");
            var obliterator = profile.SavedItems.Find(item => item.ItemName == "2_Obliterator");

            if (emerEvac != null && emerEvac.Quantity > 0)
            {
                EmerEvacButton.allowToggle = true;
            }

            if (thruster != null && thruster.Quantity > 0)
            {
                ThrusterButton.allowToggle = true;
            }

            if (obliterator != null && obliterator.Quantity > 0)
            {
                ObliteratorButton.allowToggle = true;
            }
        }
    }

    private void TellManagerToUseEmerEvacAction()
    {
        if (EmerEvacButton.allowToggle)
            GameManager.Instance.HasEmerEvac = (!GameManager.Instance.HasEmerEvac);
    }
    private void TellManagerToUseBoosterAction()
    {
            if (ThrusterButton.allowToggle)
                GameManager.Instance.HasThruster = (!GameManager.Instance.HasThruster);
    }
    private void TellManagerToUseObliteratorAction()
    {
        if (ObliteratorButton.allowToggle)
            GameManager.Instance.HasObliterator = (!GameManager.Instance.HasObliterator);
    }
}