using UnityEngine;

// This adds a new option to Unity's right-click Create menu!
[CreateAssetMenu(fileName = "rewardConfig")]
public class RewardConfig : ScriptableObject
{
    [Header("Information")]
    public Sprite itemIcon;
    public string Name;
    public string Description;

    [Header("Configuration")]
    public float ReFuel;
    public int ReMissile;
    public int RewardPoints; 
    
    [Range(0f, 1f)]
    public float chanceToBlank = 0.1f;


}