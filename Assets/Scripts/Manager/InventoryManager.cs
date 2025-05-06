using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<InventoryItemSaveData> savedItems;
    public List<InventoryItemSaveData> stashSavedItems;

    public PlayerWeapon.WeaponType selectedWeaponType;
    public PlayerEquipment.EquipmentType selectedHelmetType;
    public PlayerEquipment.EquipmentType selectedVestType;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

 
    public List<InventoryItemSaveData> savedItems      = new();
    public List<InventoryItemSaveData> stashSavedItems = new();

 
    private string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");

    private void Awake()
    {
        
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[Inventory] Awake → LoadFromFile() 호출, 경로: {SavePath}");
            LoadFromFile();
            Debug.Log($"[Inventory] Load 완료 → savedItems: {savedItems.Count}, stashSavedItems: {stashSavedItems.Count}");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("[InventoryManager] F9 pressed → Inventory 초기화");
            ClearInventoryData();
        }
    }
    private void OnApplicationQuit()
    {
        Debug.Log("[InventoryManager] OnApplicationQuit → SaveToFile()");
        SaveToFile();
    }

    public void RegisterItem(InventoryItemSaveData newItem)
    {
        for (int i = 0; i < savedItems.Count; i++)
        {
            if (savedItems[i].itemUuId == newItem.itemUuId)
            {
                savedItems[i] = newItem;
                return;
            }
        }
        savedItems.Add(newItem);
    }

    public void UnregisterItem(string itemUuId)
    {
        savedItems.RemoveAll(item => item.itemUuId == itemUuId);
    }

    public void RegisterStashItem(InventoryItemSaveData newItem)
    {
        for (int i = 0; i < stashSavedItems.Count; i++)
        {
            if (stashSavedItems[i].itemUuId == newItem.itemUuId)
            {
                stashSavedItems[i] = newItem;
                return;
            }
        }
        stashSavedItems.Add(newItem);
    }

    public void UnregisterStashItem(string itemUuId)
    {
        stashSavedItems.RemoveAll(item => item.itemUuId == itemUuId);
    }

  
    public void SaveToFile()
    {
        try
        {
            var wrapper = new InventorySaveData
            {
                savedItems = this.savedItems,
                stashSavedItems = this.stashSavedItems,
                selectedWeaponType = GameData.Instance?.selectedWeaponType ?? PlayerWeapon.WeaponType.NONE,
                selectedHelmetType = GameData.Instance?.selectedHelmetType ?? PlayerEquipment.EquipmentType.NONEHELMET,
                selectedVestType = GameData.Instance?.selectedVestType ?? PlayerEquipment.EquipmentType.NONEVEST
            };

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(SavePath, json);

            Debug.Log($"Inventory saved to {SavePath}");
            Debug.Log($"[SaveCheck] Saving {savedItems.Count} items, {stashSavedItems.Count} stash items");
            Debug.Log($"[SaveCheck] Equipped → Weapon: {wrapper.selectedWeaponType}, Helmet: {wrapper.selectedHelmetType}, Vest: {wrapper.selectedVestType}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save inventory: {e}");
        }
    }


    public void LoadFromFile()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No inventory save file found.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("Inventory JSON file is empty.");
                return;
            }

            var wrapper = JsonUtility.FromJson<InventorySaveData>(json);
            if (wrapper == null)
            {
                Debug.LogError("Parsed inventory JSON is null.");
                return;
            }

            savedItems = wrapper.savedItems ?? new List<InventoryItemSaveData>();
            stashSavedItems = wrapper.stashSavedItems ?? new List<InventoryItemSaveData>();

            if (GameData.Instance != null)
            {
                GameData.Instance.selectedWeaponType = wrapper.selectedWeaponType;
                GameData.Instance.selectedHelmetType = wrapper.selectedHelmetType;
                GameData.Instance.selectedVestType = wrapper.selectedVestType;
            }

            Debug.Log($"Inventory loaded from {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load inventory: {e}");
        }
    }

    public void ClearInventoryData()
    {
        savedItems.Clear();
        stashSavedItems.Clear();

        if (GameData.Instance != null)
        {
            GameData.Instance.selectedWeaponType = PlayerWeapon.WeaponType.NONE;
            GameData.Instance.selectedHelmetType = PlayerEquipment.EquipmentType.NONEHELMET;
            GameData.Instance.selectedVestType = PlayerEquipment.EquipmentType.NONEVEST;
        }

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log($"[InventoryManager] Save file deleted: {SavePath}");
        }
        else
        {
            Debug.Log("[InventoryManager] No save file to delete.");
        }
    }
}
