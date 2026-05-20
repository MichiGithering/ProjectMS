using TMPro;
using UnityEngine;

public class RPOnMenuUpdate : MonoBehaviour
{
    public TextMeshProUGUI displayRP;
    public static RPOnMenuUpdate Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
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