using TMPro;
using UnityEngine;

public class RPOnMenuUpdate : MonoBehaviour
{
    public TextMeshProUGUI displayRP;

    public void Start()
    {
        SaveScoreManager.PlayerProfileData profile = SaveScoreManager.Instance.LoadGame();

        UpdateRP(profile.TotalResearchPoints);
    }

    public void UpdateRP(int rp)
    {
        displayRP.text = "RP: " + rp.ToString();
    }
}