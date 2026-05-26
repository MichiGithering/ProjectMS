using System.Collections;
using TMPro;
using UnityEngine;

public class ConclusionLoop : MonoBehaviour
{
    [Header("Title of Information")]
    [SerializeField] public CanvasGroup DistanceTravelled;
    [SerializeField] public CanvasGroup ExpandTime;
    [SerializeField] public CanvasGroup ResourceGet;
    [SerializeField] public CanvasGroup TheTitle;
    [SerializeField] public CanvasGroup ConclusionText;

    [Header("Stats Tell")]
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI conclusionText;
    [SerializeField] public TextMeshProUGUI distanceText;
    [SerializeField] public TextMeshProUGUI expandTimeText;
    [SerializeField] public TextMeshProUGUI researchPointsText;
    [SerializeField] public TextMeshProUGUI itemGetList;
    public int Context;
    private bool isTransitioning = false;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        Context = GameManager.Instance.EndContext;
        titleText.gameObject.SetActive(false);

        conclusionText.gameObject.SetActive(true);
        conclusionText.color = new Color(conclusionText.color.r, conclusionText.color.g, conclusionText.color.b, 1f);

        DistanceTravelled.alpha = 0;
        ExpandTime.alpha = 0;
        ResourceGet.alpha = 0;
    }

    private void Start()
    {
        switch (Context)
        {
            case 0:
                conclusionText.text = "You have successfully returned to the station. Your efforts have been recognized, and your discoveries have secured the progression of our people, giving them true hope for the future.";
                break;
            case 1:
                conclusionText.text = "You never made it back to the station, and the people fell into despair. More resources must now be sacrificed to mount another expedition. You are left stranded in the middle of nowhere until your time runs out.";
                break;
            case 2:
                conclusionText.text = "You crashed into an asteroid, and the people fell into despair. More resources must now be sacrificed to mount another expedition. No one will ever know that your ship has been reduced to scrap drifting in space.";
                break;
            default:
                conclusionText.text = "For unknown reasons, you never returned to the station. The people fell into despair. More resources must now be sacrificed to draft another expedition.";
                break;
        }

        StartCoroutine(MoveToConclusion(Context));
    }

    private IEnumerator MoveToConclusion(int context)
    {
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(FadeTextAlpha(conclusionText, 0f, fadeDuration));
        conclusionText.gameObject.SetActive(false);

        SetupConclusionData(context);

        titleText.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(DistanceTravelled, 1f, fadeDuration));
        StartCoroutine(FadeCanvasGroup(ExpandTime, 1f, fadeDuration));

        if (context == 0)
        {
            StartCoroutine(FadeCanvasGroup(ResourceGet, 1f, fadeDuration));
        }

        // Start the automatic 5-second countdown to the menu
        StartCoroutine(GoToMenu());
    }

    private void SetupConclusionData(int context)
    {
        switch (context)
        {
            case 0:
                titleText.text = "Safe Expedition";
                break;
            case 1:
            default:
                titleText.text = "Lost in Mirage";
                break;
            case 2:
                titleText.text = "Crushed in Despair";
                break;
        }

        if (GameManager.Instance != null)
        {
            distanceText.text = $"{GameManager.Instance.TravelDistance:F2} units";
            expandTimeText.text = $"{GameManager.Instance.ExpandTime:F0} seconds";

            if (researchPointsText != null)
            {
                researchPointsText.text = $"{GameManager.Instance.ResearchPoints}";
            }

            if (itemGetList != null)
            {
                if (GameManager.Instance.CargoBag.Count == 0)
                {
                    itemGetList.text = "No items collected.";
                }
                else
                {
                    string itemsString = "";
                    foreach (var item in GameManager.Instance.CargoBag)
                    {
                        itemsString += $"{item.Quantity}x {item.ItemName}\n";
                    }
                    itemGetList.text = itemsString;
                }
            }
        }
    }

    private IEnumerator FadeTextAlpha(TextMeshProUGUI textTarget, float targetAlpha, float duration)
    {
        Color c = textTarget.color;
        float startAlpha = c.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            textTarget.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        textTarget.color = c;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cgTarget, float targetAlpha, float duration)
    {
        float startAlpha = cgTarget.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            cgTarget.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        cgTarget.alpha = targetAlpha;
    }

    private IEnumerator GoToMenu()
    {
        yield return new WaitForSeconds(5f);
        SkipToMenu();
    }

    public void SkipToMenu()
    {
        // 1. LOCK THE DOOR IMMEDIATELY! 
        // If we have already started the leaving process, ignore all future clicks!
        if (isTransitioning) return;
        isTransitioning = true;

        // 2. Roll the dice
        if (IntersitialAds.Instance != null)
        {
            IntersitialAds.Instance.TryShowRandomAd();
        }

        // 3. Route the player
        if (!IntersitialAds.isAdShowing)
        {
            SceneTransition.Instance.TransitionToScene("MainMenuScene");
        }
        else
        {
            StartCoroutine(WaitForAdToFinish());
        }
    }

    private IEnumerator WaitForAdToFinish()
    {
        while (IntersitialAds.isAdShowing)
        {
            yield return null;
        }

        // The ad just finished! We are already locked, so just load the scene safely.
        SceneTransition.Instance.TransitionToScene("MainMenuScene");
    }
}