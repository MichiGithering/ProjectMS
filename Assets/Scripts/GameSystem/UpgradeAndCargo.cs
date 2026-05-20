using TMPro;
using UnityEngine;
using System.Collections;

public class UpgradeAncInventoryManager : MonoBehaviour
{
    [Header("UI Menus (Canvas Groups)")]
    [SerializeField] private CanvasGroup UpgradeCanvas;
    [SerializeField] private CanvasGroup CargoCanvas;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Cost Text Labels")]
    [SerializeField] private TextMeshProUGUI UpHpcost;
    [SerializeField] private TextMeshProUGUI UpSpcost;
    [SerializeField] private TextMeshProUGUI UpFtcost;
    [SerializeField] private TextMeshProUGUI UpMscost;

    [Header("Use Items")]
    [SerializeField] private TextMeshProUGUI AmountEmerEvac;
    [SerializeField] private TextMeshProUGUI AmountBooster;
    [SerializeField] private TextMeshProUGUI AmountObliterator;

    [Header("Details Panel Setup")]
    [SerializeField] private CanvasGroup DetailsCanvas;
    [SerializeField] private float slideOffset = 800f; // How far down it goes off-screen
    [SerializeField] private float slideSpeed = 10f; // How fast it slides up/down

    [SerializeField] private TextMeshProUGUI LastestRun;
    [SerializeField] private TextMeshProUGUI LongestRun;
    [SerializeField] private TextMeshProUGUI HPLvl;
    [SerializeField] private TextMeshProUGUI SpdLvl;
    [SerializeField] private TextMeshProUGUI FuelLvl;
    [SerializeField] private TextMeshProUGUI MissileLvl;

    public static UpgradeAncInventoryManager Instance { get; private set; }

    private ProjectMSInputAction playerInputActions;

    private Coroutine upgradeFadeRoutine;
    private Coroutine cargoFadeRoutine;
    private Coroutine detailsSlideRoutine;

    // --- Menu State Trackers ---
    private bool isUpgradeOpen = false;
    private bool isCargoOpen = false;
    private bool isDetailsManuallyOpen = false; // Manual button tracker

    // Movement tracking for the Details panel
    private RectTransform detailsRectTransform;
    private Vector2 detailsOriginalPos;
    private Vector2 detailsHiddenPos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new ProjectMSInputAction();
        playerInputActions.GameControl.MenuUpgrade.performed += ctx => ToggleUpgrade();
        playerInputActions.GameControl.MenuCargo.performed += ctx => ToggleCargo();

        // Wire up the new manual details button!
        playerInputActions.GameControl.ReadDetails.performed += ctx => ToggleDetails();

        if (DetailsCanvas != null)
        {
            detailsRectTransform = DetailsCanvas.GetComponent<RectTransform>();
            detailsOriginalPos = detailsRectTransform.anchoredPosition;
            detailsHiddenPos = new Vector2(detailsOriginalPos.x, detailsOriginalPos.y - slideOffset);
        }
    }

    void Start()
    {
        InitializeMenuState(UpgradeCanvas);
        InitializeMenuState(CargoCanvas);

        if (DetailsCanvas != null && detailsRectTransform != null)
        {
            InitializeMenuState(DetailsCanvas);
            detailsRectTransform.anchoredPosition = detailsHiddenPos;
        }

        UpdateUpgradeCosts();
    }

    private void FixedUpdate()
    {
        UpdateItemAmount();
        UpdateDetails();
    }

    private void UpdateDetails()
    {
        if (SaveScoreManager.Instance == null || UpgradeManager.Instance == null) return;

        SaveScoreManager.PlayerProfileData profile = SaveScoreManager.Instance.LoadGame();
        UpgradeManager.UpgradeProfileData upgradeProfile = UpgradeManager.Instance.LoadUpgrades();

        if (LastestRun != null) LastestRun.text = $"{profile.LatestDistanceTraveled:F2} unit";
        if (LongestRun != null) LongestRun.text = $"{profile.LongestDistanceTraveled:F2} unit";
        if (HPLvl != null) HPLvl.text = $"{upgradeProfile.HealthLevel}";
        if (SpdLvl != null) SpdLvl.text = $"{upgradeProfile.SpeedLevel}";
        if (FuelLvl != null) FuelLvl.text = $"{upgradeProfile.FuelLevel}";
        if (MissileLvl != null) MissileLvl.text = $"{upgradeProfile.MissileLevel}";
    }

    private void UpdateItemAmount()
    {
        if (SaveScoreManager.Instance == null) return;

        SaveScoreManager.PlayerProfileData profile = SaveScoreManager.Instance.LoadGame();

        int evacCount = 0;
        int boosterCount = 0;
        int obliteratorCount = 0;

        if (profile.SavedItems != null)
        {
            foreach (var item in profile.SavedItems)
            {
                if (item.ItemName == "0_EmerEvac") evacCount = item.Quantity;
                else if (item.ItemName == "1_Booster") boosterCount = item.Quantity;
                else if (item.ItemName == "2_Obliterator") obliteratorCount = item.Quantity;
            }
        }

        if (AmountEmerEvac != null) AmountEmerEvac.text = evacCount.ToString();
        if (AmountBooster != null) AmountBooster.text = boosterCount.ToString();
        if (AmountObliterator != null) AmountObliterator.text = obliteratorCount.ToString();
    }

    private void InitializeMenuState(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Disable();
        }
    }

    private void ToggleUpgrade()
    {
        if (UpgradeCanvas == null) return;

        isUpgradeOpen = !isUpgradeOpen;

        if (isUpgradeOpen && isCargoOpen)
        {
            isCargoOpen = false;
            SafeStartFade(ref cargoFadeRoutine, CargoCanvas, 0f);
        }

        float targetAlpha = isUpgradeOpen ? 1f : 0f;
        SafeStartFade(ref upgradeFadeRoutine, UpgradeCanvas, targetAlpha);

        CheckDetailsVisibility();
    }

    private void ToggleCargo()
    {
        if (CargoCanvas == null) return;

        isCargoOpen = !isCargoOpen;

        if (isCargoOpen && isUpgradeOpen)
        {
            isUpgradeOpen = false;
            SafeStartFade(ref upgradeFadeRoutine, UpgradeCanvas, 0f);
        }

        float targetAlpha = isCargoOpen ? 1f : 0f;
        SafeStartFade(ref cargoFadeRoutine, CargoCanvas, targetAlpha);

        CheckDetailsVisibility();
    }

    private void ToggleDetails()
    {
        isDetailsManuallyOpen = !isDetailsManuallyOpen;
        CheckDetailsVisibility();
    }

    private void CheckDetailsVisibility()
    {
        if (DetailsCanvas == null || detailsRectTransform == null) return;

        bool shouldShowDetails = isUpgradeOpen || isCargoOpen || isDetailsManuallyOpen;

        if (detailsSlideRoutine != null)
        {
            StopCoroutine(detailsSlideRoutine);
        }
        detailsSlideRoutine = StartCoroutine(SlideDetailsRoutine(shouldShowDetails));
    }

    private IEnumerator SlideDetailsRoutine(bool show)
    {
        Vector2 targetPos = show ? detailsOriginalPos : detailsHiddenPos;
        float targetAlpha = show ? 1f : 0f;

        DetailsCanvas.interactable = show;
        DetailsCanvas.blocksRaycasts = show;

        while (Vector2.Distance(detailsRectTransform.anchoredPosition, targetPos) > 1f || !Mathf.Approximately(DetailsCanvas.alpha, targetAlpha))
        {
            detailsRectTransform.anchoredPosition = Vector2.Lerp(detailsRectTransform.anchoredPosition, targetPos, Time.unscaledDeltaTime * slideSpeed);
            DetailsCanvas.alpha = Mathf.MoveTowards(DetailsCanvas.alpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            yield return null;
        }

        detailsRectTransform.anchoredPosition = targetPos;
        DetailsCanvas.alpha = targetAlpha;
    }

    private void SafeStartFade(ref Coroutine activeRoutine, CanvasGroup group, float targetAlpha)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(FadeMenuRoutine(group, targetAlpha));
    }

    private IEnumerator FadeMenuRoutine(CanvasGroup canvasGroup, float targetAlpha)
    {
        if (targetAlpha == 0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        float currentAlpha = canvasGroup.alpha;

        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            canvasGroup.alpha = currentAlpha;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (targetAlpha == 1f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void UpdateUpgradeCosts()
    {
        if (UpgradeManager.Instance == null || SaveScoreManager.Instance == null) return;

        UpgradeManager.UpgradeProfileData profile = UpgradeManager.Instance.LoadUpgrades();
        SaveScoreManager.PlayerProfileData RPprofile = SaveScoreManager.Instance.LoadGame();

        int hpCost = 20 + ((profile.HealthLevel - 1) * (3 + profile.HealthLevel));
        int spCost = 20 + ((profile.SpeedLevel - 1) * (5 + profile.SpeedLevel));
        int ftCost = 40 + ((profile.FuelLevel - 1) * (5 + profile.FuelLevel));
        int msCost = 30 + ((profile.MissileLevel - 1) * (7 + profile.MissileLevel));

        if (UpHpcost != null) UpHpcost.text = hpCost.ToString();
        if (UpSpcost != null) UpSpcost.text = spCost.ToString();
        if (UpFtcost != null) UpFtcost.text = ftCost.ToString();
        if (UpMscost != null) UpMscost.text = msCost.ToString();

        if (RPOnMenuUpdate.Instance != null)
        {
            RPOnMenuUpdate.Instance.UpdateRP(RPprofile.TotalResearchPoints);
        }
    }
}