// InventoryController.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    [Header("UI Link")]
    [SerializeField] private UIInventoryPage inventoryUI; // Assign your primary UIInventoryPage here if it's always in the same scene as the controller

    [SerializeField]
    private InventorySO inventoryData;

    // --- Private Data ---
    private List<InventorySlot> _inventorySlots;
    private const string INVENTORY_SAVE_KEY = "PlayerInventoryData_Unified";
    private bool _hasAssignedUIToController = false;

    // --- Public Events ---
    public event System.Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;
    public int InventorySize => inventoryData.Size;
    public event System.Action<int, ItemData, int> OnInventorySlotUpdated; // slotIndex, itemData, newQuantity
    public event System.Action OnInventoryRefreshed; // For full UI refresh

    // --- Public Properties ---
    public List<InventorySlot> InventorySlots_ReadOnly => new List<InventorySlot>(_inventorySlots); // For read-only external access if needed

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeInventory(); // Initialize internal data structure
            LoadInventory();       // Load saved data into the structure

            // For testing: attempt to add an item shortly after start
            // Ensure ItemDataManager is ready and an item "potion_health_01" exists
            // Invoke(nameof(DebugForceAddItem), 2.5f); // Increased from 1.5f
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // If a UIInventoryPage is assigned via the Inspector and not yet linked by Awake/another script
        if (inventoryUI != null && !_hasAssignedUIToController)
        {
            AssignAndInitializeUI(inventoryUI);
        }
    }

    //private void Update()
    //{
    //    // Attempt to find and assign a UI if none is currently active/valid
    //    // This is mainly for scenarios where UI might be dynamically loaded/unloaded
    //    // or if the controller starts before the UI is ready.
    //    if (inventoryUI == null || !inventoryUI.gameObject.scene.isLoaded)
    //    {
    //        UIInventoryPage sceneUI = FindFirstObjectByType<UIInventoryPage>();
    //        if (sceneUI != null && sceneUI.gameObject.activeInHierarchy)
    //        {
    //            AssignAndInitializeUI(sceneUI);
    //        }
    //    }

    //    // If, after checks, we still don't have a valid UI, don't process input for it
    //    if (inventoryUI == null || !inventoryUI.gameObject.scene.isLoaded) return;

    //    // Toggle inventory UI with 'I' key
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        ToggleInventoryPanel();
    //        foreach (var item in inventoryData.GetCurrentInventoryState())
    //        {
    //            inventoryUI.UpdateData(item.Key, item.Value, item.ItemImage, item.Value.quantity);

    //        }

    //    }
    //}
    // This is the CORRECTED way
    private void Update()
    {
        // Example toggle with 'I' key
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI != null && !inventoryUI.gameObject.activeSelf)
            {
                inventoryUI.Show();
                // Notify UI to update with the latest data
                OnInventoryUpdated?.Invoke(inventoryData.GetCurrentInventoryState());
            }
            else
            {
                inventoryUI.Hide();
            }
        }
    }


    #region UI Management
    public void AssignAndInitializeUI(UIInventoryPage uiPage)
    {
        if (uiPage == null)
        {
            Debug.LogWarning("InventoryController: Attempted to assign a null UI page.");
            return;
        }

        if (this.inventoryUI != uiPage || !_hasAssignedUIToController)
        {
            this.inventoryUI = uiPage;
            this.inventoryUI.InitializeInventoryUI(inventoryData.Size);
            OnInventoryRefreshed?.Invoke();
            _hasAssignedUIToController = true;
            Debug.Log($"InventoryController: UIInventoryPage '{uiPage.gameObject.name}' assigned and initialized.");
        }
        else if (this.inventoryUI == uiPage && _hasAssignedUIToController)
        {
            OnInventoryRefreshed?.Invoke(); // Same UI, just refresh
        }
    }

    public void ToggleInventoryPanel()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryController: No inventoryUI assigned to toggle.");
            return;
        }

        if (!inventoryUI.gameObject.activeSelf)
        {
            OnInventoryRefreshed?.Invoke(); // Refresh data before showing
            inventoryUI.Show();
        }
        else
        {
            inventoryUI.Hide();
        }
    }
    #endregion

    #region Inventory Initialization and Data Access
    private void InitializeInventory()
    {
        _inventorySlots = new List<InventorySlot>(inventoryData.Size);
        for (int i = 0; i < inventoryData.Size; i++)
        {
            _inventorySlots.Add(new InventorySlot(null, 0)); // itemID, quantity
        }
        Debug.Log($"InventoryController initialized with {inventoryData.Size} slots.");
    }

    public ItemData GetItemDataInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || ItemDataManager.Instance == null || _inventorySlots[slotIndex].IsEmpty())
            return null;
        return ItemDataManager.Instance.GetItemByID(_inventorySlots[slotIndex].itemID);
    }

    public int GetQuantityInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || _inventorySlots[slotIndex].IsEmpty())
            return 0;
        return _inventorySlots[slotIndex].quantity;
    }

    public bool IsSlotEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count) return true; // Out of bounds is considered empty
        return _inventorySlots[slotIndex].IsEmpty();
    }
    #endregion

    #region Item Manipulation Methods
    public bool AddItem(ItemData itemToAdd, int quantity = 1)
    {
        if (itemToAdd == null || quantity <= 0 || ItemDataManager.Instance == null)
        {
            Debug.LogError($"AddItem failed: Null item ({itemToAdd?.itemName}), zero quantity, or ItemDataManager missing.");
            return false;
        }

        bool itemAddedOrStackedSuccessfully = false;
        int remainingQuantity = quantity;

        // 1. Try to stack on existing items
        if (itemToAdd.isStackable)
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (_inventorySlots[i].itemID == itemToAdd.itemID)
                {
                    int spaceInStack = itemToAdd.maxStackSize - _inventorySlots[i].quantity;
                    int amountToStack = Mathf.Min(remainingQuantity, spaceInStack);

                    if (amountToStack > 0)
                    {
                        _inventorySlots[i].quantity += amountToStack;
                        OnInventorySlotUpdated?.Invoke(i, itemToAdd, _inventorySlots[i].quantity);
                        remainingQuantity -= amountToStack;
                        itemAddedOrStackedSuccessfully = true;
                        if (remainingQuantity <= 0) break;
                    }
                }
            }
        }

        // 2. Add to new empty slots if quantity remains
        if (remainingQuantity > 0)
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (IsSlotEmpty(i))
                {
                    int amountForNewSlot = itemToAdd.isStackable ? Mathf.Min(remainingQuantity, itemToAdd.maxStackSize) : 1;

                    _inventorySlots[i].itemID = itemToAdd.itemID;
                    _inventorySlots[i].quantity = amountForNewSlot;
                    OnInventorySlotUpdated?.Invoke(i, itemToAdd, _inventorySlots[i].quantity);
                    remainingQuantity -= amountForNewSlot;
                    itemAddedOrStackedSuccessfully = true;

                    if (remainingQuantity <= 0) break;
                    if (!itemToAdd.isStackable && remainingQuantity > 0) continue; // For non-stackable, each takes one slot
                }
            }
        }

        if (itemAddedOrStackedSuccessfully) SaveInventory();
        if (remainingQuantity > 0)
        {
            Debug.LogWarning($"Inventory full or could not add all {quantity} of {itemToAdd.itemName}. {remainingQuantity} remaining.");
        }
        return itemAddedOrStackedSuccessfully && (remainingQuantity <= 0); // True if some were added and all intended quantity got placed
    }

    public bool RemoveItemFromSlot(int slotIndex, int quantityToRemove = 1)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || IsSlotEmpty(slotIndex) || quantityToRemove <= 0)
            return false;

        ItemData itemDataInSlot = GetItemDataInSlot(slotIndex); // Get data before modification
        _inventorySlots[slotIndex].quantity -= quantityToRemove;

        if (_inventorySlots[slotIndex].quantity <= 0)
        {
            _inventorySlots[slotIndex].itemID = null;
            _inventorySlots[slotIndex].quantity = 0;
            OnInventorySlotUpdated?.Invoke(slotIndex, null, 0); // Notify UI of empty slot
        }
        else
        {
            OnInventorySlotUpdated?.Invoke(slotIndex, itemDataInSlot, _inventorySlots[slotIndex].quantity);
        }
        SaveInventory();
        return true;
    }

    public bool RemoveItemByData(ItemData itemToRemove, int quantity = 1) // For ShopSystem
    {
        if (itemToRemove == null || quantity <= 0) return false;
        int totalRemoved = 0;
        for (int i = _inventorySlots.Count - 1; i >= 0; i--) // Iterate backwards for safe removal/modification
        {
            if (_inventorySlots[i].itemID == itemToRemove.itemID)
            {
                int amountInSlot = _inventorySlots[i].quantity;
                int amountToRemoveFromThisSlot = Mathf.Min(quantity - totalRemoved, amountInSlot);

                if (amountToRemoveFromThisSlot > 0)
                {
                    RemoveItemFromSlot(i, amountToRemoveFromThisSlot); // This handles UI update and saving
                    totalRemoved += amountToRemoveFromThisSlot;
                    if (totalRemoved >= quantity) break; // All required items removed
                }
            }
        }
        if (totalRemoved < quantity) Debug.LogWarning($"Could not remove all {quantity} of {itemToRemove.itemName}. Removed: {totalRemoved}");
        return totalRemoved > 0; // True if at least one item was removed
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= _inventorySlots.Count || indexB < 0 || indexB >= _inventorySlots.Count || indexA == indexB)
            return;

        InventorySlot tempSlot = new InventorySlot(_inventorySlots[indexA].itemID, _inventorySlots[indexA].quantity);

        _inventorySlots[indexA].itemID = _inventorySlots[indexB].itemID;
        _inventorySlots[indexA].quantity = _inventorySlots[indexB].quantity;

        _inventorySlots[indexB].itemID = tempSlot.itemID;
        _inventorySlots[indexB].quantity = tempSlot.quantity;

        OnInventorySlotUpdated?.Invoke(indexA, GetItemDataInSlot(indexA), GetQuantityInSlot(indexA));
        OnInventorySlotUpdated?.Invoke(indexB, GetItemDataInSlot(indexB), GetQuantityInSlot(indexB));
        SaveInventory();
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count || IsSlotEmpty(slotIndex))
            return;

        ItemData itemToUse = GetItemDataInSlot(slotIndex);
        if (itemToUse != null)
        {
            Debug.Log($"Attempting to use item: {itemToUse.itemName} from slot {slotIndex}");
            // --- IMPLEMENT ACTUAL ITEM USAGE LOGIC HERE ---
            // Example: Check itemToUse.itemType or specific itemID
            // if (itemToUse.isConsumable) // Assuming ItemData has 'isConsumable'
            // {
            //     RemoveItemFromSlot(slotIndex, 1);
            // }
            // For now, simple consume for "potion"
            if (itemToUse.itemName.ToLower().Contains("potion"))
            {
                Debug.Log($"{itemToUse.itemName} used and consumed.");
                RemoveItemFromSlot(slotIndex, 1);
            }
            else
            {
                Debug.Log($"{itemToUse.itemName} 'used' (no consume effect implemented).");
            }
        }
    }
    #endregion

    #region Saving and Loading
    [System.Serializable]
    private class InventorySaveWrapper { public List<InventorySlot> slotsToSave; }

    public void SaveInventory()
    {
        if (_inventorySlots == null) return;
        InventorySaveWrapper wrapper = new InventorySaveWrapper { slotsToSave = _inventorySlots };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(INVENTORY_SAVE_KEY, json);
        PlayerPrefs.Save();
        // Debug.Log("Inventory Saved.");
    }

    public void LoadInventory()
    {
        if (_inventorySlots == null || _inventorySlots.Capacity < inventoryData.Size)
        {
            InitializeInventory(); // Ensures _inventorySlots exists and is correctly sized
        }
        else // Clear existing runtime data if list already exists
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                _inventorySlots[i].itemID = null;
                _inventorySlots[i].quantity = 0;
            }
            while (_inventorySlots.Count < inventoryData.Size) _inventorySlots.Add(new InventorySlot(null, 0)); // Grow if needed
            while (_inventorySlots.Count > inventoryData.Size) _inventorySlots.RemoveAt(_inventorySlots.Count - 1); // Shrink if needed

        }

        if (PlayerPrefs.HasKey(INVENTORY_SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(INVENTORY_SAVE_KEY);
            try
            {
                InventorySaveWrapper wrapper = JsonUtility.FromJson<InventorySaveWrapper>(json);
                if (wrapper != null && wrapper.slotsToSave != null)
                {
                    for (int i = 0; i < inventoryData.Size; i++)
                    {
                        if (i < wrapper.slotsToSave.Count && i < _inventorySlots.Count)
                        {
                            // Validate loaded itemID against ItemDataManager
                            if (!string.IsNullOrEmpty(wrapper.slotsToSave[i].itemID) &&
                                ItemDataManager.Instance.GetItemByID(wrapper.slotsToSave[i].itemID) == null)
                            {
                                Debug.LogWarning($"LoadInventory: Saved itemID '{wrapper.slotsToSave[i].itemID}' in slot {i} not found in ItemDataManager. Discarding.");
                                _inventorySlots[i].itemID = null;
                                _inventorySlots[i].quantity = 0;
                            }
                            else
                            {
                                _inventorySlots[i].itemID = wrapper.slotsToSave[i].itemID;
                                _inventorySlots[i].quantity = wrapper.slotsToSave[i].quantity;
                            }
                        }
                    }
                    Debug.Log($"Inventory Loaded. Saved slots: {wrapper.slotsToSave.Count}, Current inv size: {_inventorySlots.Count}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading inventory: {e.Message}. Save data might be corrupt. Deleting save.");
                PlayerPrefs.DeleteKey(INVENTORY_SAVE_KEY);
                InitializeInventory(); // Re-initialize to a clean state
            }
        }
        // OnInventoryRefreshed?.Invoke(); // UI will be refreshed when it's assigned or shown.
    }
    #endregion

    #region Debug Methods
    //private void DebugForceAddItem()
    //{
    //    if (ItemDataManager.Instance == null)
    //    {
    //        Debug.LogError("DEBUG: ItemDataManager not ready for ForceAddItem."); return;
    //    }
    //    ItemData testPotion = ItemDataManager.Instance.GetItemByID("potion_health_01"); // Ensure this itemID exists
    //    if (testPotion != null)
    //    {
    //        Debug.Log($"DEBUG: Attempting to ForceAdd '{testPotion.itemName}'.");
    //        if (!AddItem(testPotion, 1)) Debug.LogWarning($"DEBUG: ForceAdd FAILED for '{testPotion.itemName}'.");
    //    }
    //    else Debug.LogError("DEBUG: Test item 'potion_health_01' NOT FOUND in ItemDataManager.");
    //}

    [ContextMenu("DEBUG: Add Test Potion")]
    // private void DebugAddTestPotionContextMenu() => DebugForceAddItem();

    [ContextMenu("DEBUG: Clear Inventory Save Data & Reset Runtime")]
    private void DebugClearInventoryAndReset()
    {
        PlayerPrefs.DeleteKey(INVENTORY_SAVE_KEY);
        InitializeInventory(); // Reset runtime slots
        if (inventoryUI != null && inventoryUI.gameObject.activeSelf)
        {
            OnInventoryRefreshed?.Invoke(); // Refresh UI if it's visible
        }
        Debug.Log("DEBUG: Inventory Save Data Cleared. Runtime inventory reset. UI refreshed if active.");
    }



    // Add this method inside InventoryController.cs
    public int GetTotalQuantityOfItem(string itemID)
    {
        int total = 0;
        foreach (var slot in _inventorySlots)
        {
            if (slot.itemID == itemID)
            {
                total += slot.quantity;
            }
        }
        return total;
    }
    #endregion

}
#endregion


//using UnityEngine;
//using System.Collections.Generic;
//using System.Linq;

//public class InventoryController : MonoBehaviour
//{
//    public static InventoryController Instance { get; private set; }

//    [Header("Settings")]
//    public int inventorySize = 24;

//    [Header("UI Link")]
//    private UIInventoryPage _currentInventoryUI;

//    // --- Private Data ---
//    private List<InventorySlot> _inventorySlots;
//    private const string INVENTORY_SAVE_KEY = "PlayerInventory_Main";

//    // --- Public Events ---
//    public event System.Action<int, ItemData, int> OnInventorySlotUpdated;
//    public event System.Action OnInventoryRefreshed;

//    // --- Public Properties ---
//    public List<InventorySlot> InventorySlots_ReadOnly => new List<InventorySlot>(_inventorySlots);
//    public int InventorySize => inventorySize;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            InitializeInventory();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        LoadInventory();
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            if (_currentInventoryUI != null)
//            {
//                if (_currentInventoryUI.gameObject.activeSelf)
//                {
//                    _currentInventoryUI.Hide();
//                }
//                else
//                {
//                    OnInventoryRefreshed?.Invoke();
//                    _currentInventoryUI.Show();
//                }
//            }
//        }
//    }

//    private void InitializeInventory()
//    {
//        _inventorySlots = new List<InventorySlot>();
//        for (int i = 0; i < inventorySize; i++)
//        {
//            _inventorySlots.Add(new InventorySlot(null, 0));
//        }
//    }

//    public void AssignAndInitializeUI(UIInventoryPage uiPage)
//    {
//        _currentInventoryUI = uiPage;
//        if (_currentInventoryUI != null)
//        {
//            _currentInventoryUI.InitializeInventoryUI(inventorySize);
//            OnInventoryRefreshed?.Invoke();
//        }
//    }

//    public bool AddItem(ItemData itemToAdd, int quantity = 1)
//    {
//        if (itemToAdd == null || quantity <= 0) return false;

//        // Try to stack first
//        if (itemToAdd.isStackable)
//        {
//            for (int i = 0; i < _inventorySlots.Count; i++)
//            {
//                if (!_inventorySlots[i].IsEmpty() && _inventorySlots[i].itemID == itemToAdd.itemID)
//                {
//                    int spaceAvailable = itemToAdd.maxStackSize - _inventorySlots[i].quantity;
//                    if (spaceAvailable > 0)
//                    {
//                        int amountToAdd = Mathf.Min(quantity, spaceAvailable);
//                        _inventorySlots[i].quantity += amountToAdd;
//                        quantity -= amountToAdd;
//                        OnInventorySlotUpdated?.Invoke(i, itemToAdd, _inventorySlots[i].quantity);
//                    }
//                }
//                if (quantity <= 0) { SaveInventory(); return true; }
//            }
//        }

//        // Add to new slots
//        if (quantity > 0)
//        {
//            for (int i = 0; i < _inventorySlots.Count; i++)
//            {
//                if (_inventorySlots[i].IsEmpty())
//                {
//                    _inventorySlots[i].itemID = itemToAdd.itemID;
//                    int amountToAdd = Mathf.Min(quantity, itemToAdd.maxStackSize);
//                    _inventorySlots[i].quantity = amountToAdd;
//                    quantity -= amountToAdd;
//                    OnInventorySlotUpdated?.Invoke(i, itemToAdd, _inventorySlots[i].quantity);
//                    if (quantity <= 0) { SaveInventory(); return true; }
//                }
//            }
//        }

//        SaveInventory();
//        return quantity <= 0;
//    }

//    public bool RemoveItemFromSlot(int slotIndex, int quantity = 1)
//    {
//        if (IsSlotEmpty(slotIndex) || quantity <= 0) return false;

//        _inventorySlots[slotIndex].quantity -= quantity;

//        if (_inventorySlots[slotIndex].quantity <= 0)
//        {
//            _inventorySlots[slotIndex] = new InventorySlot(null, 0); // Clear slot
//            OnInventorySlotUpdated?.Invoke(slotIndex, null, 0);
//        }
//        else
//        {
//            ItemData data = GetItemDataInSlot(slotIndex);
//            OnInventorySlotUpdated?.Invoke(slotIndex, data, _inventorySlots[slotIndex].quantity);
//        }
//        SaveInventory();
//        return true;
//    }

//    public bool RemoveItemByData(ItemData itemToRemove, int quantity)
//    {
//        int totalRemoved = 0;
//        for (int i = _inventorySlots.Count - 1; i >= 0; i--)
//        {
//            if (!_inventorySlots[i].IsEmpty() && _inventorySlots[i].itemID == itemToRemove.itemID)
//            {
//                int amountToRemove = Mathf.Min(quantity - totalRemoved, _inventorySlots[i].quantity);
//                if (RemoveItemFromSlot(i, amountToRemove))
//                {
//                    totalRemoved += amountToRemove;
//                }
//            }
//            if (totalRemoved >= quantity) break;
//        }
//        return totalRemoved >= quantity;
//    }

//    public void SwapItems(int indexA, int indexB)
//    {
//        InventorySlot temp = _inventorySlots[indexA];
//        _inventorySlots[indexA] = _inventorySlots[indexB];
//        _inventorySlots[indexB] = temp;
//        OnInventoryRefreshed?.Invoke();
//        SaveInventory();
//    }

//    public void UseItem(int slotIndex)
//    {
//        ItemData itemToUse = GetItemDataInSlot(slotIndex);
//        if (itemToUse == null) return;

//        Debug.Log($"Using {itemToUse.itemName}");
//        // Add Use Logic here (e.g., equipping, consuming)
//    }

//    public int GetTotalQuantityOfItem(string itemID)
//    {
//        return _inventorySlots.Where(slot => slot.itemID == itemID).Sum(slot => slot.quantity);
//    }

//    public bool IsSlotEmpty(int index) => _inventorySlots[index].IsEmpty();
//    public ItemData GetItemDataInSlot(int index) => IsSlotEmpty(index) ? null : ItemDataManager.Instance.GetItemByID(_inventorySlots[index].itemID);
//    public int GetQuantityInSlot(int index) => IsSlotEmpty(index) ? 0 : _inventorySlots[index].quantity;

//    private void SaveInventory()
//    {
//        string json = JsonUtility.ToJson(new InventorySaveData { inventorySlots = _inventorySlots });
//        PlayerPrefs.SetString(INVENTORY_SAVE_KEY, json);
//        PlayerPrefs.Save();
//    }

//    private void LoadInventory()
//    {
//        if (PlayerPrefs.HasKey(INVENTORY_SAVE_KEY))
//        {
//            string json = PlayerPrefs.GetString(INVENTORY_SAVE_KEY);
//            InventorySaveData loadedData = JsonUtility.FromJson<InventorySaveData>(json);
//            if (loadedData != null && loadedData.inventorySlots.Count == inventorySize)
//            {
//                _inventorySlots = loadedData.inventorySlots;
//            }
//        }
//    }

//    [System.Serializable]
//    private class InventorySaveData
//    {
//        public List<InventorySlot> inventorySlots;
//    }
//}

