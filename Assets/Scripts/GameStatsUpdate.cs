using TMPro;
using UnityEngine;

public class GameStatsUpdate : MonoBehaviour
{
    public static GameStatsUpdate Instance { get; private set; }

    private GameManager gameManager;

    [Header("Game Stats")]
    public TextMeshProUGUI currentFuelText;
    public TextMeshProUGUI currentMissilesText;
    public TextMeshProUGUI currentMinimumFuelText;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TextUpdate()
    {

        if (currentFuelText != null)
            currentFuelText.text = $"Fuel: {GameManager.Instance.Fuel:F1} / {GameManager.Instance.MaxFuel:F1}";

        if (currentMissilesText != null)
            currentMissilesText.text = $"Missiles: {GameManager.Instance.Missiles} / {GameManager.Instance.MaxMissiles}";

        if (currentMinimumFuelText != null)
            currentMinimumFuelText.text = $"Return Fuel: {GameManager.Instance.minimumReturnFuel:F1}";
    }

    public void minimunFuelIncrease()
    {

    }
}
