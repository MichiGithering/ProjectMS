using TMPro;
using UnityEngine;

public class DebugPlanetReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI RewardText;
    [SerializeField] private Planet planet;

    private void Start()
    {
        if (RewardText != null)
        {
            // Turn off the text visually, but keep the GameObject awake!
            RewardText.enabled = false;
        }
    }

    private void Update()
    {
        if (planet != null && !planet.HasReward)
        {
            // Turn the text visibility back on
            RewardText.enabled = true;

            // Turn off this specific script's update loop to save performance
            this.enabled = false;
        }
    }
}