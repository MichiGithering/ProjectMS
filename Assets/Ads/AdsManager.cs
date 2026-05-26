using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AdsManager : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string AndroidAppID = "YOUR_ANDROID_APP_ID";

    [Header("Ad Unit IDs")]
    [SerializeField] private string AndroidBannerAdUnitID = "YOUR_ANDROID_BANNER_AD_UNIT_ID";
    [SerializeField] private string AndroidInterstitialAdUnitID = "YOUR_ANDROID_INTERSTITIAL_AD_UNIT_ID";

    private LevelPlayBannerAd bannerAd;
    public int position = 0; //0: Top, 1: Bottom
    private LevelPlayInterstitialAd interstitialAd;
    private bool _pendingInit = false;
    public bool isSdkInitialized { get; private set; } = false;

    public static AdsManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //LevelPlay.ValidateIntegration();

        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        // SDK init
        LevelPlay.Init(AndroidAppID);

        LevelPlay.SetMetaData("is_child_directed", "true");
        LevelPlay.SetMetaData("AdMob_TFCD", "true");
        LevelPlay.SetMetaData("AppLovin_AgeRestrictedUser", "true");

    }
    private void Update()
    {
        if (_pendingInit)
        {
            _pendingInit = false;
            isSdkInitialized = true; // ? move this here too, set on main thread
            CreateInterstitialAds();
            LoadInterstitialAd();
            Debug.Log("[Ads] SDK ready. Interstitial created and loading.");
        }
    }
    private void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        _pendingInit = true;
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.LogError($"SDK Initialization Failed: {error.ErrorCode} - {error.ErrorMessage}");
    }

    #region banner
    private void CreateBannerAds(LevelPlayBannerPosition position)
    {
        var adConfig = new LevelPlayBannerAd.Config.Builder()
            .SetPosition(position)
            .Build();

        bannerAd = new LevelPlayBannerAd(AndroidBannerAdUnitID, adConfig);

        bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
    }

    public void LoadBannerAd(LevelPlayBannerPosition position)
    {
        if (bannerAd == null)
        {
            CreateBannerAds(position);
        }

        bannerAd.LoadAd();
    }
    public void DestroyBanner()
    {
        if (bannerAd != null)
        {
            bannerAd.DestroyAd();
            bannerAd = null;
        }
    }

    public void ShowBannerAd() { if (bannerAd != null) bannerAd.ShowAd(); }
    public void HideBannerAd() { if (bannerAd != null) bannerAd.HideAd(); }

    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError) { }
    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }
    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo) { }

    #endregion
    #region interstitial
    private void CreateInterstitialAds()
    {
        interstitialAd = new LevelPlayInterstitialAd(AndroidInterstitialAdUnitID);

        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }

    public void LoadInterstitialAd()
    {
        if (interstitialAd == null)
        {
            CreateInterstitialAds();
        }
        Debug.Log("[Ads] Loading interstitial ad...");
        interstitialAd.LoadAd();
    }
    public bool ShowInterstitialAd()
    {
        Debug.Log("[Ads] Attempting to show interstitial ad...");
        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            interstitialAd.ShowAd();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void DestroyInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.DestroyAd();
            interstitialAd = null; // Clean up properly!
        }
    }
    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error) { }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        IntersitialAds.isAdShowing = false;
        LoadInterstitialAd();       
    }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }
    #endregion


}