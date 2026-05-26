using UnityEngine;

public class IntersitialAds : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] private float adChance = 0.1f;
    public static bool isAdShowing = false;

    // The Singleton Bridge
    public static IntersitialAds Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        isAdShowing = false;
    }

    public void TryShowRandomAd()
    {
        if (AdsManager.Instance == null) return;

        if (AdsManager.Instance.isSdkInitialized)
        {
            float randomValue = Random.Range(0f, 1f);

            // Roll the 10% chance
            if (randomValue <= adChance && !isAdShowing)
            {
                bool adSuccessfullyShowed = AdsManager.Instance.ShowInterstitialAd();

                if (adSuccessfullyShowed)
                {
                    isAdShowing = true;
                }
            }
        }
    }
}