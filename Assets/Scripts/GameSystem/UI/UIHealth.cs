using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIFuel : MonoBehaviour
{
    [Header("Fuel Bar")]
    [SerializeField] private Image fuelBarFill;
    public float liveFuel;
    public float maxFuel;
    public float fuelPercentage;
    public float markerPercentage;
    public float returnFuel;

    [Tooltip("The color when the tank is completely full")]
    public Color fullColor = Color.yellow;
    [Tooltip("The color when the tank is empty")]
    public Color emptyColor = Color.red;

    [Header("Return Fuel Marker")]
    [SerializeField] private Image returnFuelScale;
    [SerializeField] private Image warningFuel;

    private Coroutine blinkCoroutine;


    [Header("HP")]
    [SerializeField] private Image ShieldUI;
    [SerializeField] private TextMeshProUGUI ShieldCountText;
    public int currentHP;
    public int MaxHP;

    void Awake()
    {
        if (fuelBarFill == null)
        {
            Debug.LogError("Fuel Bar Fill Image is not assigned in the inspector.");
        }
        if (returnFuelScale == null)
        {
            Debug.LogWarning("Return Fuel Scale Image is not assigned in the inspector.");
        }

        if (warningFuel != null) warningFuel.gameObject.SetActive(false);

            if (ShieldUI == null)
            {
                Debug.LogWarning("Shield UI Image is not assigned in the inspector.");
            }
        else
        {
            ShieldUI.enabled = false;
        }
            if (ShieldCountText == null)
        {
            Debug.LogWarning("Shield Count Text is not assigned in the inspector.");
        }

    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            /// --- FUEL LOGIC ---
            liveFuel = GameManager.Instance.Fuel;
            maxFuel = GameManager.Instance.MaxFuel;

            if (maxFuel > 0)
            {
                fuelPercentage = liveFuel / maxFuel;
                fuelBarFill.fillAmount = fuelPercentage;
                fuelBarFill.color = Color.Lerp(emptyColor, fullColor, fuelPercentage);

                returnFuel = GameManager.Instance.minimumReturnFuel;
                markerPercentage = Mathf.Clamp01(returnFuel / maxFuel);

                if (returnFuelScale != null)
                {
                    float anchorX = 1f - markerPercentage;
                    returnFuelScale.rectTransform.anchorMin = new Vector2(anchorX, returnFuelScale.rectTransform.anchorMin.y);
                    returnFuelScale.rectTransform.anchorMax = new Vector2(anchorX, returnFuelScale.rectTransform.anchorMax.y);
                    returnFuelScale.rectTransform.anchoredPosition = new Vector2(0f, returnFuelScale.rectTransform.anchoredPosition.y);
                }

                // --- WARNING LOGIC ---
                if (fuelPercentage <= markerPercentage + 0.15f)
                {
                    if (blinkCoroutine == null)
                    {
                        blinkCoroutine = StartCoroutine(WarningBlinkRoutine());
                    }
                }
                else
                {
                    if (blinkCoroutine != null)
                    {
                        StopCoroutine(blinkCoroutine);
                        blinkCoroutine = null; // Reset the tracker

                        // Force the image off so it doesn't get stuck "on" when the coroutine stops
                        warningFuel.gameObject.SetActive(false);
                    }
                }

                // --- COLOR LOGIC ---
                if (returnFuel > liveFuel)
                {
                    returnFuelScale.color = Color.red; // Danger!
                }
                else
                {
                    returnFuelScale.color = Color.white; // Reset to safe color!
                }
            }

            /// --- SHIELD LOGIC ---
            currentHP = GameManager.Instance.Hp;
            MaxHP = GameManager.Instance.MaxHp;

            if (MaxHP > 1 && currentHP > 1)
            {
                if (ShieldUI != null)
                {
                    ShieldUI.enabled = true;
                }

                if (ShieldCountText != null)
                {
                    ShieldCountText.enabled = true;
                    ShieldCountText.text = (currentHP - 1).ToString();
                }
            }
            else
            {
                if (ShieldUI != null)
                {
                    ShieldUI.enabled = false;
                }

                if (ShieldCountText != null)
                {
                    ShieldCountText.enabled = false;
                }
            }
        }
    }

    private IEnumerator WarningBlinkRoutine()
    {
        while (true)
        {
            // Toggle the image on and off
            warningFuel.gameObject.SetActive(!warningFuel.gameObject.activeSelf);

            if (fuelPercentage <= markerPercentage)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else if (fuelPercentage <= markerPercentage + 0.10f)
            {
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}