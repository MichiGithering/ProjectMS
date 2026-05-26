using System.Collections;
using UnityEngine;
using Unity.Services.LevelPlay;

public enum CustomBannerPosition
{
    TopCenter,
    BottomCenter
}

public class BannerAds : MonoBehaviour
{
    [SerializeField] public bool showBannerAds = false;
    public CustomBannerPosition AdsPosition;

    private void Awake()
    {
        if (AdsManager.Instance == null)
        {
            Debug.LogError("AdsManager instance not found. Please ensure AdsManager is initialized before using BannerAds.");
            return;
        }

        // 1. DESTROY the old banner instead of hiding it, so we can rebuild it!
        AdsManager.Instance.DestroyBanner();
    }

    private void Start()
    {
        if (AdsManager.Instance == null) return;

        if (AdsManager.Instance.isSdkInitialized)
        {
            ControlBanner();
        }
        else
        {
            StartCoroutine(WaitForSdkThenControl());
        }
    }

    private IEnumerator WaitForSdkThenControl()
    {
        while (!AdsManager.Instance.isSdkInitialized)
            yield return new WaitForSeconds(0.5f);

        ControlBanner();
    }

    private void ControlBanner()
    {
        if (showBannerAds)
        {
            LevelPlayBannerPosition requestedPosition = LevelPlayBannerPosition.TopCenter;

            if (AdsPosition == CustomBannerPosition.BottomCenter)
            {
                requestedPosition = LevelPlayBannerPosition.BottomCenter;
            }

            AdsManager.Instance.LoadBannerAd(requestedPosition);
            AdsManager.Instance.ShowBannerAd();
        }
        else
        {
            // 2. DESTROY it here too!
            AdsManager.Instance.DestroyBanner();
        }
    }

    // 3. ADD THIS BACK IN!
    // This guarantees that when you leave this scene, the banner is wiped out.
    // That way, the next scene can spawn it wherever it wants.
    private void OnDestroy()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.DestroyBanner();
        }
    }
}