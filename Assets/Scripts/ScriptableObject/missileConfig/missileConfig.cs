using UnityEngine;

// This adds a new option to Unity's right-click Create menu!
[CreateAssetMenu(fileName = "missileConfig")]
public class MissileConfig : ScriptableObject
{
    [Header("Information")]
    public Sprite itemIcon;
    public string Name;
    public string Description;

    [Header("Configuration")]
    public float ExplosionRadius;
    public float Damage;
}