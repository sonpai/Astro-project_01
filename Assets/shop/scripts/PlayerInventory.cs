//// PlayerInventory.cs
//using UnityEngine;
//using System.Collections.Generic;
//using System.Linq; // For Linq queries like FirstOrDefault
//using System;

//public class PlayerInventory : MonoBehaviour // Inherit from MonoBehaviour
//{
//    public static PlayerInventory Instance { get; private set; }

//    private List<InventorySlot> _items = new List<InventorySlot>();
//    public List<InventorySlot> Items => new List<InventorySlot>(_items); // Return a copy for read-only access

//    public event Action OnInventoryChanged; // Generic event for UI updates

//    private const string INVENTORY_PREFS_KEY = "PlayerInventoryJSON_Corrected"; // Use a distinct key
//    private const int MAX_UNIQUE_SLOTS = 100;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject); // Now valid
//            LoadInventory(); // Load inventory in Awake
//        }
//        else
//        {
//            Destroy(gameObject); // Now valid
//        }
//    }

//    public void AddItem(string itemID, int quantity = 1)
//    {
//        if (quantity <= 0 || string.IsNullOrEmpty(itemID)) return;

//        // Ensure ItemDataManager is available
//        if (ItemDataManager.Instance == null)
//        {
//            Debug.LogError("ItemDataManager.Instance is null. Cannot add item.");
//            return;
//        }

//        ItemData itemData = ItemDataManager.Instance.GetItemByID(itemID);
//        if (itemData == null)
//        {
//            Debug.LogError($"Cannot add item: No ItemData found for ID '{itemID}'");
//            return;
//        }

//        InventorySlot existingSlot = _items.FirstOrDefault(slot => slot.itemID == itemID);

//        if (existingSlot != null)
//        {
//            existingSlot.quantity += quantity;
//        }
//        else
//        {
//            if (_items.Count >= MAX_UNIQUE_SLOTS)
//            {
//                Debug.LogWarning("Inventory is full of unique items. Cannot add new item type.");
//                return;
//            }
//            _items.Add(new InventorySlot(itemID, quantity));
//        }

//        OnInventoryChanged?.Invoke();
//        SaveInventory();
//        Debug.Log($"Added {quantity}x '{itemID}' to inventory. Total for this item: {GetItemQuantity(itemID)}");
//    }

//    public bool RemoveItem(string itemID, int quantity = 1)
//    {
//        if (quantity <= 0 || string.IsNullOrEmpty(itemID)) return false;

//        InventorySlot slotToRemoveFrom = _items.FirstOrDefault(slot => slot.itemID == itemID);

//        if (slotToRemoveFrom != null && slotToRemoveFrom.quantity >= quantity)
//        {
//            slotToRemoveFrom.quantity -= quantity;
//            if (slotToRemoveFrom.quantity <= 0)
//            {
//                _items.Remove(slotToRemoveFrom);
//            }
//            OnInventoryChanged?.Invoke();
//            SaveInventory();
//            Debug.Log($"Removed {quantity}x '{itemID}' from inventory.");
//            return true;
//        }
//        Debug.LogWarning($"Could not remove {quantity}x '{itemID}'. Not enough in inventory or item not found.");
//        return false;
//    }

//    public int GetItemQuantity(string itemID)
//    {
//        InventorySlot slot = _items.FirstOrDefault(s => s.itemID == itemID);
//        return slot?.quantity ?? 0;
//    }

//    public List<(ItemData item, int quantity)> GetSellableItemsWithDetails()
//    {
//        var detailedItems = new List<(ItemData, int)>();
//        if (ItemDataManager.Instance == null)
//        {
//            Debug.LogError("ItemDataManager.Instance is null. Cannot get sellable item details.");
//            return detailedItems; // Return empty list
//        }

//        foreach (var slot in _items)
//        {
//            ItemData data = ItemDataManager.Instance.GetItemByID(slot.itemID);
//            if (data != null && data.canBeSold && data.sellPrice > 0)
//            {
//                detailedItems.Add((data, slot.quantity));
//            }
//        }
//        return detailedItems;
//    }

//    [System.Serializable]
//    private class InventorySaveData
//    {
//        public List<InventorySlot> inventorySlots;
//    }

//    private void SaveInventory()
//    {
//        InventorySaveData saveData = new InventorySaveData { inventorySlots = _items };
//        string json = JsonUtility.ToJson(saveData);
//        PlayerPrefs.SetString(INVENTORY_PREFS_KEY, json); // PlayerPrefs is valid
//        PlayerPrefs.Save();
//    }

//    private void LoadInventory()
//    {
//        string json = PlayerPrefs.GetString(INVENTORY_PREFS_KEY, "{}");
//        InventorySaveData loadedData = JsonUtility.FromJson<InventorySaveData>(json);

//        if (loadedData != null && loadedData.inventorySlots != null)
//        {
//            _items = loadedData.inventorySlots;
//        }
//        else
//        {
//            _items = new List<InventorySlot>();
//        }
//        OnInventoryChanged?.Invoke();
//    }

//    [ContextMenu("DEBUG: Add Test Items")]
//    public void DebugAddTestItems()
//    {
//        if (ItemDataManager.Instance == null) { Debug.LogError("ItemDataManager not ready for DEBUG items."); return; }
//        AddItem("potion_health_01", 3); // Ensure these itemIDs exist in your ItemDataManager
//        AddItem("sword_basic_01", 1);
//        AddItem("gold_ore", 10);
//    }
//    [ContextMenu("DEBUG: Clear Inventory")]
//    public void DebugClearInventory() { _items.Clear(); SaveInventory(); OnInventoryChanged?.Invoke(); }
//}