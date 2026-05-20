using TMPro;
using UnityEngine;

public class GameStatsUpdate : MonoBehaviour
{
    public static GameStatsUpdate Instance { get; private set; }

    [Header("Game Stats")]
    public TextMeshProUGUI currentFuelText;
    public TextMeshProUGUI currentMissilesText;
    public TextMeshProUGUI currentMinimumFuelText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        TextUpdate();
    }

    public void TextUpdate()
    {
        if (currentFuelText != null)
        {
            if (GameManager.Instance.Fuel < GameManager.Instance.minimumReturnFuel)
            {
                currentFuelText.color = Color.red;
                currentFuelText.text = "Fuel: NOT ENOUGH";
            }
            else
            {
                currentFuelText.color = Color.white;
                currentFuelText.text = $"Fuel: {GameManager.Instance.Fuel:F1} / {GameManager.Instance.MaxFuel:F1}";
            }
        }

        if (currentMissilesText != null)
        {
            currentMissilesText.text = $"Missiles: {GameManager.Instance.Missiles} / {GameManager.Instance.MaxMissiles}";
        }

        if (currentMinimumFuelText != null)
        {
            currentMinimumFuelText.text = $"Return Fuel: {GameManager.Instance.minimumReturnFuel:F1}";
        }
    }

    // You can safely delete this if you are already calculating this in the GameManager!
    public void minimunFuelIncrease()
    {

    }
}