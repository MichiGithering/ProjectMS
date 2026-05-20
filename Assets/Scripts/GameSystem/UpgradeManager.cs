using System.IO;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    private ProjectMSInputAction playerInputActions;

    private string saveFilePath;

    [System.Serializable]
    public class UpgradeProfileData
    {
        public int HealthLevel = 1;
        public int SpeedLevel = 1;
        public int FuelLevel = 1;
        public int MissileLevel = 1;
    }

    private void Awake()
    {
        playerInputActions = new ProjectMSInputAction();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Application.persistentDataPath + "/upgradeProfile.json";
    }
    protected virtual void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.GameControl.UpgradeContext.performed += OnMenuUpgradePerformed;
    }

    protected virtual void OnDisable()
    {
        playerInputActions.Disable();
        playerInputActions.GameControl.UpgradeContext.performed -= OnMenuUpgradePerformed;
    }
    private void OnMenuUpgradePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        string keyPressed = context.control.name;

        switch (keyPressed)
        {
            case "1":
                UpgradeLevel(0); // Health
                break;
            case "2":
                UpgradeLevel(1); // Speed
                break;
            case "3":
                UpgradeLevel(2); // Fuel
                break;
            case "4":
                UpgradeLevel(3); // Missile
                break;
            default:
                Debug.LogWarning("An unmapped key triggered MenuUpgrade: " + keyPressed);
                break;
        }
    }
    public UpgradeProfileData LoadUpgrades()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonString = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<UpgradeProfileData>(jsonString);
        }
        else
        {
            return new UpgradeProfileData();
        }
    }

    public void SaveUpgrades(UpgradeProfileData profile)
    {
        string jsonString = JsonUtility.ToJson(profile, true);
        File.WriteAllText(saveFilePath, jsonString);
    }

    public void UpgradeLevel(int context)
    {
        UpgradeProfileData profile = LoadUpgrades();
        SaveScoreManager.PlayerProfileData inventory = SaveScoreManager.Instance.LoadGame();

        int cost = 0;

        // FIXED: The Backend now uses your new "Static Base Cost" math!
        switch (context)
        {
            case 0: cost = 20 + ((profile.HealthLevel - 1) * (3 + profile.HealthLevel)); break;
            case 1: cost = 20 + ((profile.SpeedLevel - 1) * (5 + profile.SpeedLevel)); break;
            case 2: cost = 40 + ((profile.FuelLevel - 1) * (5 + profile.FuelLevel)); break;
            case 3: cost = 30 + ((profile.MissileLevel - 1) * (7 + profile.MissileLevel)); break;
            default: return;
        }

        // 2. Check if the player can afford it
        if (inventory.TotalResearchPoints >= cost)
        {
            // 3. Take their money!
            SaveScoreManager.Instance.AdjustResearchPoints(-cost);

            // 4. Apply the upgrade
            switch (context)
            {
                case 0: profile.HealthLevel += 1; Debug.Log("Upgraded Health!"); break;
                case 1: profile.SpeedLevel += 1; Debug.Log("Upgraded Speed!"); break;
                case 2: profile.FuelLevel += 1; Debug.Log("Upgraded Fuel!"); break;
                case 3: profile.MissileLevel += 1; Debug.Log("Upgraded Missiles!"); break;
            }

            SaveUpgrades(profile);

            // Instantly refresh the UI so the player sees the new price for the next level!
            UpgradeAncInventoryManager.Instance.UpdateUpgradeCosts();
        }
        else
        {
            Debug.Log($"Not enough Research Points! You have {inventory.TotalResearchPoints}, but need {cost} RP.");
        }
    }
}