using UnityEngine;

// This adds a new option to Unity's right-click Create menu!
[CreateAssetMenu(fileName = "entityConfig")]
public class EntityConfig : ScriptableObject
{
    [Header("Information")]
    public Sprite itemIcon;
    public string Name;
    public string Description;

    [Header("Configuration")]
    public int MaxHP;
    public float MaxSpeed;
    public float MaxFuel;
    public int MaxMissiles;
}