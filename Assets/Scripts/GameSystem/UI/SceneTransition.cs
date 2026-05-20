using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1.5f;

    private Coroutine activeFadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 1f);
            fadeImage.raycastTarget = false;
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn(1f, 0.2f));
    }

    public IEnumerator FadeIn(float startAlpha, float delay)
    {
        if (fadeImage == null) yield break;

        yield return new WaitForSecondsRealtime(delay);

        float alpha = startAlpha;
        while (alpha > 0f)
        {
            alpha -= Time.unscaledDeltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha));
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    }

    public void ChangeOverlayOpacity(float targetAlpha)
    {
        if (fadeImage == null) return;

        // If a fade is already running, stop it so they don't fight
        if (activeFadeCoroutine != null)
        {
            StopCoroutine(activeFadeCoroutine);
        }

        activeFadeCoroutine = StartCoroutine(FadeToAlphaRoutine(targetAlpha));
    }

    private IEnumerator FadeToAlphaRoutine(float targetAlpha)
    {
        float currentAlpha = fadeImage.color.a;

        // Smoothly slide from whatever the CURRENT alpha is to your target alpha
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            fadeImage.color = new Color(0f, 0f, 0f, currentAlpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, targetAlpha);
        activeFadeCoroutine = null;
    }

    public void TransitionToScene(string sceneName)
    {
        if (activeFadeCoroutine != null) StopCoroutine(activeFadeCoroutine);
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("No fade image assigned! Loading scene instantly.");
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        fadeImage.raycastTarget = true;
        float alpha = fadeImage.color.a;

        while (alpha < 1f)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha));
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}