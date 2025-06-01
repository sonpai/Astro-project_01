// InventoryController.cs (Corrected to use InventorySlot)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    [SerializeField] private UIInventoryPage inventoryUI; // This is your UI for levels
    public int inventorySize = 10;

    // Uses InventorySlot from your InventorySlot.cs file
    private List<InventorySlot> _inventorySlots;
    public List<InventorySlot> InventorySlots_ReadOnly => new List<InventorySlot>(_inventorySlots);

    public event System.Action<int, ItemData, int> OnInventorySlotUpdated;
    public event System.Action OnInventoryRefreshed;

    private const string INVENTORY_SAVE_KEY = "PlayerInventoryData_Unified";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInventory();
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // UI might not be assigned in Awake if it's scene-specific and this is a persistent manager
    }

    private void Start()
    {
        // If the UIInventoryPage for levels is present in this scene, assign and initialize it.
        // If InventoryController starts in a scene without this specific UI,
        // UIInventoryPage in other scenes should find InventoryController.Instance in its Start/Awake.
        if (inventoryUI != null)
        {
            AssignAndInitializeUI(inventoryUI);
        }
    }

    // Public method for UI pages in different scenes to register themselves
    public void AssignAndInitializeUI(UIInventoryPage uiPage)
    {
        inventoryUI = uiPage; // Assign the current active UI
        if (inventoryUI != null)
        {
            inventoryUI.InitializeInventoryUI(inventorySize);
            OnInventoryRefreshed?.Invoke(); // Ensure it displays current data
            Debug.Log("Inventory UI assigned and refreshed for: " + uiPage.gameObject.name);
        }
    }


    private void Update()
    {
        if (inventoryUI == null || !inventoryUI.gameObject.scene.isLoaded) // Check if UI is valid and its scene is loaded
        {
            // Try to find a UI if current one is null or from an unloaded scene
            UIInventoryPage sceneUI = FindFirstObjectByType<UIInventoryPage>();
            if (sceneUI != null) AssignAndInitializeUI(sceneUI);
            // If still null, then no UI available in this scene to toggle.
            if (inventoryUI == null) return;
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.gameObject.activeSelf == false)
            {
                OnInventoryRefreshed?.Invoke();
                inventoryUI.Show();
            }
            else
            {
                inventoryUI.Hide();
            }
        }
    }

    private void InitializeInventory()
    {
        _inventorySlots = new List<InventorySlot>(inventorySize);
        for (int i = 0; i < inventorySize; i++)
        {
            _inventorySlots.Add(new InventorySlot(null, 0)); // Use constructor from your InventorySlot.cs
        }
        Debug.Log($"InventoryController initialized with {inventorySize} slots.");
    }

    public ItemData GetItemDataInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || ItemDataManager.Instance == null) return null;
        return ItemDataManager.Instance.GetItemByID(_inventorySlots[slotIndex].itemID);
    }

    public int GetQuantityInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count) return 0;
        return _inventorySlots[slotIndex].quantity;
    }

    public bool IsSlotEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count) return true;
        return string.IsNullOrEmpty(_inventorySlots[slotIndex].itemID) || _inventorySlots[slotIndex].quantity <= 0;
    }

    public bool AddItem(ItemData itemToAdd, int quantity = 1)
    {
        if (itemToAdd == null || quantity <= 0 || ItemDataManager.Instance == null)
        {
            Debug.LogError("AddItem failed: Null item, zero quantity, or ItemDataManager not found.");
            return false;
        }

        bool itemAddedOrStacked = false;
        if (itemToAdd.isStackable)
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (_inventorySlots[i].itemID == itemToAdd.itemID)
                {
                    int canAdd = itemToAdd.maxStackSize - _inventorySlots[i].quantity;
                    int amountToAdd = Mathf.Min(quantity, canAdd);
                    if (amountToAdd > 0)
                    {
                        _inventorySlots[i].quantity += amountToAdd;
                        OnInventorySlotUpdated?.Invoke(i, itemToAdd, _inventorySlots[i].quantity);
                        quantity -= amountToAdd;
                        itemAddedOrStacked = true;
                        if (quantity <= 0) break;
                    }
                }
            }
        }

        if (quantity > 0)
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (IsSlotEmpty(i))
                {
                    int amountToAdd = itemToAdd.isStackable ? Mathf.Min(quantity, itemToAdd.maxStackSize) : 1;
                    _inventorySlots[i].itemID = itemToAdd.itemID;
                    _inventorySlots[i].quantity = amountToAdd;
                    OnInventorySlotUpdated?.Invoke(i, itemToAdd, _inventorySlots[i].quantity);
                    quantity -= amountToAdd;
                    itemAddedOrStacked = true;
                    if (quantity <= 0) break;
                    if (!itemToAdd.isStackable && quantity > 0) continue;
                }
            }
        }

        if (itemAddedOrStacked) SaveInventory();
        if (quantity > 0) Debug.LogWarning($"Inventory full or could not add all {quantity} of {itemToAdd.itemName}.");
        return quantity <= 0;
    }

    public bool RemoveItem(int slotIndex, int quantityToRemove = 1)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || IsSlotEmpty(slotIndex) || quantityToRemove <= 0)
            return false;

        ItemData itemData = GetItemDataInSlot(slotIndex); // Get ItemData for event
        _inventorySlots[slotIndex].quantity -= quantityToRemove;

        if (_inventorySlots[slotIndex].quantity <= 0)
        {
            _inventorySlots[slotIndex].itemID = null; // Clear itemID as well
            _inventorySlots[slotIndex].quantity = 0;
            OnInventorySlotUpdated?.Invoke(slotIndex, null, 0);
        }
        else
        {
            OnInventorySlotUpdated?.Invoke(slotIndex, itemData, _inventorySlots[slotIndex].quantity);
        }
        SaveInventory();
        return true;
    }

    public bool RemoveItemByData(ItemData itemToRemove, int quantity = 1)
    {
        if (itemToRemove == null || quantity <= 0) return false;
        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].itemID == itemToRemove.itemID)
            {
                int quantityInSlot = _inventorySlots[i].quantity;
                int amountToRemoveFromThisSlot = Mathf.Min(quantity, quantityInSlot);

                RemoveItem(i, amountToRemoveFromThisSlot); // This will handle saving and events
                quantity -= amountToRemoveFromThisSlot;

                if (quantity <= 0) return true;
            }
        }
        if (quantity > 0) Debug.LogWarning($"Could not remove all {itemToRemove.name}, remaining: {quantity}");
        return quantity <= 0;
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= _inventorySlots.Count || indexB < 0 || indexB >= _inventorySlots.Count || indexA == indexB)
            return;

        InventorySlot temp = new InventorySlot(_inventorySlots[indexA].itemID, _inventorySlots[indexA].quantity);
        _inventorySlots[indexA].itemID = _inventorySlots[indexB].itemID;
        _inventorySlots[indexA].quantity = _inventorySlots[indexB].quantity;
        _inventorySlots[indexB].itemID = temp.itemID;
        _inventorySlots[indexB].quantity = temp.quantity;

        ItemData itemA = GetItemDataInSlot(indexA);
        ItemData itemB = GetItemDataInSlot(indexB);
        OnInventorySlotUpdated?.Invoke(indexA, itemA, _inventorySlots[indexA].quantity);
        OnInventorySlotUpdated?.Invoke(indexB, itemB, _inventorySlots[indexB].quantity);
        SaveInventory();
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || IsSlotEmpty(slotIndex)) return;

        ItemData itemToUse = GetItemDataInSlot(slotIndex);
        if (itemToUse != null)
        {
            Debug.Log($"Attempting to use item: {itemToUse.itemName} from slot {slotIndex}");
            // --- IMPLEMENT ACTUAL ITEM USAGE LOGIC ---
            // Example: if (itemToUse.itemID == "potion_health_01" && PlayerHealth.Instance != null) { PlayerHealth.Instance.Heal(2); }
            bool consumed = false; // Determine if item should be consumed
            if (itemToUse.itemName.ToLower().Contains("potion")) consumed = true; // Example

            if (consumed) RemoveItem(slotIndex, 1);
            // -----------------------------------------
        }
    }

    [System.Serializable]
    private class InventorySaveWrapper { public List<InventorySlot> slotsToSave; } // Uses InventorySlot

    public void SaveInventory()
    {
        InventorySaveWrapper wrapper = new InventorySaveWrapper { slotsToSave = _inventorySlots };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(INVENTORY_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        // Ensure _inventorySlots is initialized before trying to load into it
        if (_inventorySlots == null || _inventorySlots.Capacity != inventorySize)
        {
            InitializeInventory(); // This also sets capacity
        }
        else
        {
            // Clear existing runtime data before loading, but keep the list instance.
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                _inventorySlots[i].itemID = null;
                _inventorySlots[i].quantity = 0;
            }
        }


        if (PlayerPrefs.HasKey(INVENTORY_SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(INVENTORY_SAVE_KEY);
            InventorySaveWrapper wrapper = JsonUtility.FromJson<InventorySaveWrapper>(json);
            if (wrapper != null && wrapper.slotsToSave != null)
            {
                for (int i = 0; i < inventorySize; i++)
                {
                    if (i < wrapper.slotsToSave.Count && i < _inventorySlots.Count) // Ensure we don't go out of bounds for either list
                    {
                        _inventorySlots[i].itemID = wrapper.slotsToSave[i].itemID;
                        _inventorySlots[i].quantity = wrapper.slotsToSave[i].quantity;
                    }
                    // If saved data had fewer slots than inventorySize, remaining _inventorySlots are already empty.
                }
                Debug.Log("Inventory Loaded from PlayerPrefs. Slot count: " + _inventorySlots.Count);
            }
        }
        OnInventoryRefreshed?.Invoke();
    }

    [ContextMenu("DEBUG: Add Test Item (Health Potion)")]
    private void DebugAddHealthPotion()
    {
        if (ItemDataManager.Instance == null) { Debug.LogError("ItemDataManager not ready."); return; }
        ItemData potion = ItemDataManager.Instance.GetItemByID("potion_health_01"); // Make sure this ID exists in your ItemDataManager
        if (potion != null) AddItem(potion, 1);
        else Debug.LogError("Debug item 'potion_health_01' not found. Check ItemDataManager and ItemData asset ID.");
    }
    [ContextMenu("DEBUG: Clear Inventory Data & Refresh")]
    private void DebugClearInventoryAndRefresh()
    {
        PlayerPrefs.DeleteKey(INVENTORY_SAVE_KEY);
        InitializeInventory();
        OnInventoryRefreshed?.Invoke();
        Debug.Log("DEBUG: Inventory Cleared and UI Refreshed.");
    }
}