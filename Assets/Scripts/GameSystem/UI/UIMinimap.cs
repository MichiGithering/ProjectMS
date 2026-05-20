using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private CanvasGroup minimapCanvasGroup;
    [SerializeField] private float fadeSpeed = 5f;

    private ProjectMSInputAction playerInputActions;
    private Coroutine activeFadeCoroutine;
    private bool isMapOpen = false;

    private void Awake()
    {
        playerInputActions = new ProjectMSInputAction();

        if (minimapCanvasGroup == null)
        {
            minimapCanvasGroup = GetComponent<CanvasGroup>();
        }

        if (minimapCanvasGroup != null)
        {
            minimapCanvasGroup.alpha = 0f;
            minimapCanvasGroup.interactable = false;
            minimapCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            Debug.LogError($"[Minimap UI Error] Please drag your 'Minimap' GameObject (with the CanvasGroup attached) into the Inspector slot on {name}!");
        }
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.PlayerControl.OpenMinimap.performed += OnMinimapPerformed;
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.PlayerControl.OpenMinimap.performed -= OnMinimapPerformed;
            playerInputActions.Disable();
        }
    }

    private void OnMinimapPerformed(InputAction.CallbackContext context)
    {
        ToggleMinimap();
    }

    private void ToggleMinimap()
    {
        if (minimapCanvasGroup != null)
        {
            isMapOpen = !isMapOpen;

            if (SceneTransition.Instance != null)
            {
                float desiredAlpha = isMapOpen ? 0.5f : 0f;
                SceneTransition.Instance.ChangeOverlayOpacity(desiredAlpha);
            }

            if (activeFadeCoroutine != null)
            {
                StopCoroutine(activeFadeCoroutine);
            }

            float targetMapAlpha = isMapOpen ? 1f : 0f;
            activeFadeCoroutine = StartCoroutine(FadeMinimapRoutine(targetMapAlpha));

            minimapCanvasGroup.interactable = isMapOpen;
            minimapCanvasGroup.blocksRaycasts = isMapOpen;
        }
    }

    private IEnumerator FadeMinimapRoutine(float targetAlpha)
    {
        float currentAlpha = minimapCanvasGroup.alpha;

        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            minimapCanvasGroup.alpha = currentAlpha;
            yield return null;
        }

        minimapCanvasGroup.alpha = targetAlpha;
        activeFadeCoroutine = null;
    }
}