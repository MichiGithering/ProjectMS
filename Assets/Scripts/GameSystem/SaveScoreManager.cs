using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveScoreManager : MonoBehaviour
{
    public static SaveScoreManager Instance { get; private set; }

    [System.Serializable]
    public class InventoryItem
    {
        public int Quantity;
        public string ItemName;

        public InventoryItem(int quantity, string name)
        {
            Quantity = quantity;
            ItemName = name;
        }
    }

    [System.Serializable]
    public class PlayerProfileData
    {
        public int TotalResearchPoints;

        public List<InventoryItem> SavedItems = new List<InventoryItem>();

        public float LatestDistanceTraveled;
    }

    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    saveFilePath = Application.persistentDataPath + "/playerProfile.json";
    }

    public void AddItemToInventory(string nameOfItem, int amountToAdd)
    {
        PlayerProfileData profile = LoadGame();

        InventoryItem existingItem = profile.SavedItems.Find(item => item.ItemName == nameOfItem);

        if (existingItem != null)
        {
            existingItem.Quantity += amountToAdd;
        }
        else
        {
            profile.SavedItems.Add(new InventoryItem(amountToAdd, nameOfItem));
        }

        SaveProfile(profile);
        Debug.Log($"Added {amountToAdd} {nameOfItem} to inventory!");
    }

    public void SaveRun(float runDistance, int researchPointsEarned, List<InventoryItem> extractedItems = null)
    {
        PlayerProfileData profile = LoadGame();
        profile.TotalResearchPoints += researchPointsEarned;
        profile.LatestDistanceTraveled = runDistance;

        if (extractedItems != null)
        {
            foreach (InventoryItem loot in extractedItems)
            {
                InventoryItem existingItem = profile.SavedItems.Find(item => item.ItemName == loot.ItemName);

                if (existingItem != null)
                {
                    existingItem.Quantity += loot.Quantity; // Stack it
                }
                else
                {
                    profile.SavedItems.Add(new InventoryItem(loot.Quantity, loot.ItemName)); // Add new
                }
            }
        }

        SaveProfile(profile);
        Debug.Log("Run Saved! Temporary cargo transferred to permanent stash.");
    }

    private void SaveProfile(PlayerProfileData profile)
    {
        string jsonString = JsonUtility.ToJson(profile, true);
        File.WriteAllText(saveFilePath, jsonString);
    }

    public PlayerProfileData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonString = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<PlayerProfileData>(jsonString);
        }
        else
        {
            return new PlayerProfileData();
        }
    }
}