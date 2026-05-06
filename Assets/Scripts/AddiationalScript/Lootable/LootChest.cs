using System.Collections.Generic;
using UnityEngine;

public class LootChest : MonoBehaviour
{

    [System.Serializable]
    public class WeightedItem
    {
        [Tooltip("Name of the item. Matches the ItemName used in SaveScoreManager.")]
        public string itemName = "";

        [Tooltip("The relative chance of this item being selected.")]
        [Range(0f, 100f)]
        public float weight = 10f;
    }


    [Header("Loot Table")]
    [Tooltip("All possible items this can drop, with their relative weights.")]
    [SerializeField] private List<WeightedItem> lootTable = new List<WeightedItem>();

    [Header("Drop Count")]
    [SerializeField] private int minDrops = 1;
    [SerializeField] private int maxDrops = 1;

    [Header("Quantity Per Drop")]
    [SerializeField] private int minQuantity = 1;
    [SerializeField] private int maxQuantity = 1;

    public void Open()
    {

        if (lootTable == null || lootTable.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}: LootChest has no items in its loot table!");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("LootChest: GameManager.Instance is null — cannot add loot to CargoBag.");
            return;
        }


        int dropCount = Random.Range(minDrops, maxDrops + 1);
        List<WeightedItem> availableItems = new List<WeightedItem>(lootTable);

        Debug.Log($"{gameObject.name}: Opening chest — rolling {dropCount} drop(s).");

        for (int i = 0; i < dropCount; i++)
        {
            if (availableItems.Count == 0)
                break;

            WeightedItem selected = RollItem(availableItems);

            if (selected == null)
                break;

            int quantity = Random.Range(minQuantity, maxQuantity + 1);

            // Send loot directly to GameManager cargo
            GameManager.Instance.CollectLoot(selected.itemName, quantity);

            Debug.Log($"  Dropped: {quantity}x {selected.itemName}");
        }
    }


    private WeightedItem RollItem(List<WeightedItem> pool)
    {
        float totalWeight = 0f;
        
        foreach (WeightedItem item in pool)
        {
            totalWeight += item.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (WeightedItem item in pool)
        {
            currentWeight += item.weight;
            if (randomValue <= currentWeight)
                return item;
        }

        return pool[pool.Count - 1];
    }

}