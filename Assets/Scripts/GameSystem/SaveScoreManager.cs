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

        public float LongestDistanceTraveled;
    }

    private string _saveFilePath;
    private string saveFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(_saveFilePath))
                _saveFilePath = Application.persistentDataPath + "/playerProfile.json";
            return _saveFilePath;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

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
    }
    public void RemoveItemFromInventory(string nameOfItem, int amountToRemove)
    {
        PlayerProfileData profile = LoadGame();
        InventoryItem existingItem = profile.SavedItems.Find(item => item.ItemName == nameOfItem);

        if (existingItem != null && existingItem.Quantity >= amountToRemove)
        {
            existingItem.Quantity -= amountToRemove; // Deduct the amount

            if (existingItem.Quantity <= 0)
            {
                profile.SavedItems.Remove(existingItem);
            }

            SaveProfile(profile);
        }
    }
    public void SaveRun(float runDistance, int researchPointsEarned, List<InventoryItem> extractedItems = null)
    {
        PlayerProfileData profile = LoadGame();
        profile.TotalResearchPoints += researchPointsEarned;
        profile.LatestDistanceTraveled = runDistance;

        if (runDistance > profile.LongestDistanceTraveled)
        {
            profile.LongestDistanceTraveled = runDistance;
        }

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
    }
    public void AdjustResearchPoints(int amount)
    {
        PlayerProfileData profile = LoadGame();
        profile.TotalResearchPoints += amount;
        profile.TotalResearchPoints = Mathf.Max(0, profile.TotalResearchPoints);
        SaveProfile(profile);
    }
    private void SaveProfile(PlayerProfileData profile)
    {
        try
        {
            string jsonString = JsonUtility.ToJson(profile, true);
            string path = saveFilePath;

            // Ensure directory exists — critical on Android
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, jsonString);
            Debug.Log($"[Save] Saved to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Save] FAILED to save: {e.Message}");
        }
    }

    public PlayerProfileData LoadGame()
    {
        try
        {
            string path = saveFilePath;
            Debug.Log($"[Save] Loading from: {path}");

            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                PlayerProfileData data = JsonUtility.FromJson<PlayerProfileData>(jsonString);

                if (data == null)
                {
                    Debug.LogWarning("[Save] JSON parsed but returned null — returning fresh profile.");
                    return new PlayerProfileData();
                }
                return data;
            }
            else
            {
                Debug.Log("[Save] No save file found — returning fresh profile.");
                return new PlayerProfileData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Save] FAILED to load: {e.Message}");
            return new PlayerProfileData();
        }
    }
}