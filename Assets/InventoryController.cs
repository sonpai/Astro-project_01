
//////    [System.Serializable]
//////    private class InventorySaveData { public List<InventorySlot> slots; }
//////}

////// InventoryController.cs
////using UnityEngine;
////using System.Collections.Generic;

////public class InventoryController : MonoBehaviour
////{
////    public static InventoryController Instance { get; private set; }

////    [Header("Settings")]
////    public int inventorySize = 24;

////    [Header("Item Drop")]
////    [SerializeField] private GameObject groundItemPrefab; // Assign your ground item prefab

////    private UIInventoryPage _currentInventoryUI;
////    private List<InventorySlot> _inventorySlots;
////    private const string SAVE_KEY = "PlayerInventory";

////    public event System.Action<int, ItemData, int> OnInventorySlotUpdated;
////    public event System.Action OnInventoryRefreshed;

////    private void Awake()
////    {
////        if (Instance == null)
////        {
////            Instance = this;
////            DontDestroyOnLoad(gameObject);
////            InitializeInventory();
////        }
////        else
////        {
////            Destroy(gameObject);
////        }
////    }

////    private void Start()
////    {
////        LoadInventory();
////        // Check if the inventory is empty after loading, and if so, add starting items.
////        // This prevents adding items every single time you start the game.
////        if (IsInventoryEmpty())
////        {
////            AddStartingItems();
////        }
////    }

////    private void Update()
////    {
////        if (Input.GetKeyDown(KeyCode.I) && _currentInventoryUI != null)
////        {
////            if (_currentInventoryUI.gameObject.activeSelf) _currentInventoryUI.Hide();
////            else
////            {
////                OnInventoryRefreshed?.Invoke();
////                _currentInventoryUI.Show();
////            }
////        }
////    }

////    // --- NEW HELPER FUNCTION ---
////    private bool IsInventoryEmpty()
////    {
////        foreach (var slot in _inventorySlots)
////        {
////            if (!slot.IsEmpty())
////            {
////                return false; // Found an item, so it's not empty
////            }
////        }
////        return true; // No items were found
////    }

////    // --- NEW FUNCTION TO ADD ITEMS ON START ---
////    private void AddStartingItems()
////    {
////        if (ItemDataManager.Instance == null)
////        {
////            Debug.LogError("Cannot add starting items because ItemDataManager is not ready!");
////            return;
////        }

////        AddItem(ItemDataManager.Instance.GetItemByID("1"), 5);
////        AddItem(ItemDataManager.Instance.GetItemByID("2"), 10);
////        AddItem(ItemDataManager.Instance.GetItemByID("6"), 3);


////        Debug.Log("Added starting items to a fresh inventory.");

////    }

////    public void AssignAndInitializeUI(UIInventoryPage uiPage)
////    {
////        _currentInventoryUI = uiPage;
////        if (_currentInventoryUI != null)
////        {
////            _currentInventoryUI.InitializeInventoryUI(inventorySize);
////            OnInventoryRefreshed?.Invoke();
////        }
////    }

////    public bool AddItem(ItemData item, int quantity = 1)
////    {
////        if (item == null)
////        {
////            Debug.LogWarning("Tried to add a null item.");
////            return false;
////        }

////        // --- ADDING STACKING LOGIC ---
////        // 1. Try to stack on existing items
////        if (item.isStackable)
////        {
////            for (int i = 0; i < inventorySize; i++)
////            {
////                // If the slot has the same item and has space
////                if (!_inventorySlots[i].IsEmpty() && _inventorySlots[i].itemID == item.itemID)
////                {
////                    int spaceAvailable = item.maxStackSize - _inventorySlots[i].quantity;
////                    if (spaceAvailable > 0)
////                    {
////                        int amountToAdd = Mathf.Min(quantity, spaceAvailable);
////                        _inventorySlots[i].quantity += amountToAdd;
////                        quantity -= amountToAdd;
////                        OnInventorySlotUpdated?.Invoke(i, item, _inventorySlots[i].quantity);
////                    }
////                }
////                if (quantity <= 0) { SaveInventory(); return true; }
////            }
////        }

////        // 2. Add remaining to new slots
////        if (quantity > 0)
////        {
////            for (int i = 0; i < inventorySize; i++)
////            {
////                if (_inventorySlots[i].IsEmpty())
////                {
////                    _inventorySlots[i].itemID = item.itemID;
////                    int amountToAdd = Mathf.Min(quantity, item.maxStackSize);
////                    _inventorySlots[i].quantity = amountToAdd;
////                    quantity -= amountToAdd;
////                    OnInventorySlotUpdated?.Invoke(i, item, _inventorySlots[i].quantity);
////                    if (quantity <= 0) { SaveInventory(); return true; }
////                }
////            }
////        }

////        if (quantity > 0) Debug.LogWarning("Inventory is full! Could not add all items.");
////        SaveInventory();
////        return quantity <= 0;
////    }

////    public void SwapItems(int indexA, int indexB)
////    {
////        InventorySlot temp = _inventorySlots[indexA];
////        _inventorySlots[indexA] = _inventorySlots[indexB];
////        _inventorySlots[indexB] = temp;
////        OnInventoryRefreshed?.Invoke();
////        SaveInventory();
////    }

////    public ItemData GetItemDataInSlot(int index) => IsSlotEmpty(index) ? null : ItemDataManager.Instance.GetItemByID(_inventorySlots[index].itemID);
////    public int GetQuantityInSlot(int index) => IsSlotEmpty(index) ? 0 : _inventorySlots[index].quantity;
////    public bool IsSlotEmpty(int index) => index < 0 || index >= _inventorySlots.Count || _inventorySlots[index].IsEmpty();

////    private void InitializeInventory()
////    {
////        _inventorySlots = new List<InventorySlot>();
////        for (int i = 0; i < inventorySize; i++)
////        {
////            _inventorySlots.Add(new InventorySlot(null, 0));
////        }
////    }

////    private void SaveInventory()
////    {
////        string json = JsonUtility.ToJson(new InventorySaveData { slots = _inventorySlots });
////        PlayerPrefs.SetString(SAVE_KEY, json);
////    }

////    private void LoadInventory()
////    {
////        if (PlayerPrefs.HasKey(SAVE_KEY))
////        {
////            string json = PlayerPrefs.GetString(SAVE_KEY);
////            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
////            if (data != null && data.slots.Count == inventorySize)
////            {
////                _inventorySlots = data.slots;
////            }
////        }
////    }

////    [System.Serializable]
////    private class InventorySaveData { public List<InventorySlot> slots; }
////}


//// InventoryController.cs
//using UnityEngine;
//using System.Collections.Generic;

//public class InventoryController : MonoBehaviour
//{
//    public static InventoryController Instance { get; private set; }

//    [Header("Settings")]
//    public int inventorySize = 24;

//    [Header("Item Drop")]
//    [SerializeField] private GameObject groundItemPrefab;

//    private UIInventoryPage _currentInventoryUI;
//    private List<InventorySlot> _inventorySlots;
//    private const string SAVE_KEY = "PlayerInventory";

//    public event System.Action<int, ItemData, int> OnInventorySlotUpdated;
//    public event System.Action OnInventoryRefreshed;

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
//        if (IsInventoryEmpty())
//        {
//            AddStartingItems();
//        }
//        // After loading and potentially adding items, tell any listening UI to refresh.
//        OnInventoryRefreshed?.Invoke();
//    }

//    // In InventoryController.cs
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.I) && _currentInventoryUI != null)
//        {
//            // The UI's OnEnable/OnDisable now handles refreshing,
//            // so the controller's only job is to toggle its visibility.
//            if (_currentInventoryUI.gameObject.activeSelf)
//            {
//                _currentInventoryUI.Hide();
//            }
//            else
//            {
//                _currentInventoryUI.Show();
//            }
//        }
//    }

//    // --- The Rest of the Script (with minor improvements) ---

//    private bool IsInventoryEmpty()
//    {
//        foreach (var slot in _inventorySlots)
//        {
//            if (!slot.IsEmpty()) return false;
//        }
//        return true;
//    }

//    private void AddStartingItems()
//    {
//        if (ItemDataManager.Instance == null) return;
//        // Make sure these IDs match your ItemData assets!
//        // Example:
//        // AddItem(ItemDataManager.Instance.GetItemByID("axe_id"), 1);
//        // AddItem(ItemDataManager.Instance.GetItemByID("iron_ore_id"), 5);
//    }

//    public void AssignAndInitializeUI(UIInventoryPage uiPage)
//    {
//        _currentInventoryUI = uiPage;
//        if (_currentInventoryUI != null)
//        {
//            _currentInventoryUI.InitializeInventoryUI(inventorySize);
//            // Refresh the newly assigned UI with current data
//            OnInventoryRefreshed?.Invoke();
//        }
//    }

//    public bool AddItem(ItemData item, int quantity = 1)
//    {
//        if (item == null || quantity <= 0) return false;

//        if (item.isStackable)
//        {
//            for (int i = 0; i < inventorySize; i++)
//            {
//                if (!_inventorySlots[i].IsEmpty() && _inventorySlots[i].itemID == item.itemID)
//                {
//                    int spaceAvailable = item.maxStackSize - _inventorySlots[i].quantity;
//                    if (spaceAvailable > 0)
//                    {
//                        int amountToAdd = Mathf.Min(quantity, spaceAvailable);
//                        _inventorySlots[i].quantity += amountToAdd;
//                        quantity -= amountToAdd;
//                        OnInventorySlotUpdated?.Invoke(i, item, _inventorySlots[i].quantity);
//                    }
//                }
//                if (quantity <= 0) { SaveInventory(); return true; }
//            }
//        }

//        if (quantity > 0)
//        {
//            for (int i = 0; i < inventorySize; i++)
//            {
//                if (_inventorySlots[i].IsEmpty())
//                {
//                    _inventorySlots[i].itemID = item.itemID;
//                    int amountToAdd = Mathf.Min(quantity, item.maxStackSize);
//                    _inventorySlots[i].quantity = amountToAdd;
//                    quantity -= amountToAdd;
//                    OnInventorySlotUpdated?.Invoke(i, item, _inventorySlots[i].quantity);
//                    if (quantity <= 0) { SaveInventory(); return true; }
//                }
//            }
//        }

//        if (quantity > 0) Debug.LogWarning("Inventory is full!");
//        SaveInventory();
//        return quantity <= 0;
//    }

//    public void SwapItems(int indexA, int indexB)
//    {
//        InventorySlot temp = _inventorySlots[indexA];
//        _inventorySlots[indexA] = _inventorySlots[indexB];
//        _inventorySlots[indexB] = temp;
//        // **CRITICAL FIX**: After swapping, a full refresh is more reliable
//        // than trying to update two specific slots.
//        OnInventoryRefreshed?.Invoke();
//        SaveInventory();
//    }

//    public ItemData GetItemDataInSlot(int index) => IsSlotEmpty(index) ? null : ItemDataManager.Instance.GetItemByID(_inventorySlots[index].itemID);
//    public int GetQuantityInSlot(int index) => IsSlotEmpty(index) ? 0 : _inventorySlots[index].quantity;
//    public bool IsSlotEmpty(int index) => index < 0 || index >= _inventorySlots.Count || _inventorySlots[index].IsEmpty();

//    private void InitializeInventory()
//    {
//        _inventorySlots = new List<InventorySlot>();
//        for (int i = 0; i < inventorySize; i++)
//        {
//            _inventorySlots.Add(new InventorySlot(null, 0));
//        }
//    }

//    private void SaveInventory()
//    {
//        string json = JsonUtility.ToJson(new InventorySaveData { slots = _inventorySlots });
//        PlayerPrefs.SetString(SAVE_KEY, json);
//    }

//    private void LoadInventory()
//    {
//        if (PlayerPrefs.HasKey(SAVE_KEY))
//        {
//            string json = PlayerPrefs.GetString(SAVE_KEY);
//            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
//            if (data != null && data.slots.Count == inventorySize)
//            {
//                _inventorySlots = data.slots;
//            }
//        }
//        // **NOTE**: We do NOT invoke refresh here. It's now handled in Start()
//        // to ensure it happens AFTER a potential AddStartingItems() call.
//    }

//    [System.Serializable]
//    private class InventorySaveData { public List<InventorySlot> slots; }
//}

// InventoryController.cs
using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    [Header("Settings")]
    public int inventorySize = 24;

    [Header("Item Drop")]
    [SerializeField] private GameObject groundItemPrefab; // Assign your ground item prefab

    private UIInventoryPage _currentInventoryUI;
    private List<InventorySlot> _inventorySlots;
    private const string SAVE_KEY = "PlayerInventory";

    public event System.Action<int, ItemData, int> OnInventorySlotUpdated;
    public event System.Action OnInventoryRefreshed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadInventory();
        if (IsInventoryEmpty())
        {
            AddStartingItems();
        }
        OnInventoryRefreshed?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && _currentInventoryUI != null)
        {
            if (_currentInventoryUI.gameObject.activeSelf) _currentInventoryUI.Hide();
            else _currentInventoryUI.Show(); // The UI's OnEnable will handle the refresh
        }
    }

    private bool IsInventoryEmpty()
    {
        foreach (var slot in _inventorySlots)
        {
            if (!slot.IsEmpty()) return false;
        }
        return true;
    }

    private void AddStartingItems()
    {
        if (ItemDataManager.Instance == null) return;
        // IMPORTANT: Make sure these IDs match your ItemData assets!
        // This is why you might only be seeing one item. Check your IDs.
        AddItem(ItemDataManager.Instance.GetItemByID("1"), 2); // Axe
        AddItem(ItemDataManager.Instance.GetItemByID("7"), 5); // Leather
        AddItem(ItemDataManager.Instance.GetItemByID("6"), 3); // Iron Ore
    }

    public void AssignAndInitializeUI(UIInventoryPage uiPage)
    {
        _currentInventoryUI = uiPage;
        if (_currentInventoryUI != null)
        {
            _currentInventoryUI.InitializeInventoryUI(inventorySize);
            OnInventoryRefreshed?.Invoke();
        }
    }

    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        if (item.isStackable)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (!_inventorySlots[i].IsEmpty() && _inventorySlots[i].itemID == item.itemID)
                {
                    int spaceAvailable = item.maxStackSize - _inventorySlots[i].quantity;
                    if (spaceAvailable > 0)
                    {
                        int amountToAdd = Mathf.Min(quantity, spaceAvailable);
                        _inventorySlots[i].quantity += amountToAdd;
                        quantity -= amountToAdd;
                        OnInventorySlotUpdated?.Invoke(i, item, _inventorySlots[i].quantity);
                    }
                }
                if (quantity <= 0) { SaveInventory(); return true; }
            }
        }

        if (quantity > 0)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (_inventorySlots[i].IsEmpty())
                {
                    _inventorySlots[i].itemID = item.itemID;
                    int amountToAdd = Mathf.Min(quantity, item.maxStackSize);
                    _inventorySlots[i].quantity = amountToAdd;
                    quantity -= amountToAdd;
                    OnInventorySlotUpdated?.Invoke(i, item, _inventorySlots[i].quantity);
                    if (quantity <= 0) { SaveInventory(); return true; }
                }
            }
        }

        if (quantity > 0) Debug.LogWarning("Inventory is full!");
        SaveInventory();
        return quantity <= 0;
    }

    // --- NEW & UPDATED FUNCTIONS ---

    public void RemoveItemFromSlot(int slotIndex, int quantity = 1)
    {
        if (IsSlotEmpty(slotIndex) || quantity <= 0) return;

        ItemData data = GetItemDataInSlot(slotIndex); // Get data before it's modified
        _inventorySlots[slotIndex].quantity -= quantity;

        if (_inventorySlots[slotIndex].quantity <= 0)
        {
            _inventorySlots[slotIndex] = new InventorySlot(null, 0); // Clear the slot
            OnInventorySlotUpdated?.Invoke(slotIndex, null, 0);
        }
        else
        {
            OnInventorySlotUpdated?.Invoke(slotIndex, data, _inventorySlots[slotIndex].quantity);
        }
        SaveInventory();
    }

    public void SwapItems(int indexA, int indexB)
    {
        InventorySlot temp = _inventorySlots[indexA];
        _inventorySlots[indexA] = _inventorySlots[indexB];
        _inventorySlots[indexB] = temp;
        OnInventoryRefreshed?.Invoke();
        SaveInventory();
    }

    public void DropItem(int slotIndex, int quantity)
    {
        if (IsSlotEmpty(slotIndex) || groundItemPrefab == null) return;
        if (quantity <= 0) quantity = 1; // Default to dropping one

        ItemData data = GetItemDataInSlot(slotIndex);
        int quantityInSlot = GetQuantityInSlot(slotIndex);
        int quantityToDrop = Mathf.Min(quantity, quantityInSlot);

        // Find the player to drop the item near
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 dropPosition = player != null ? player.transform.position : Vector3.zero;

        // Create the item on the ground
        GameObject droppedItemObj = Instantiate(groundItemPrefab, dropPosition, Quaternion.identity);
        droppedItemObj.GetComponent<ItemPickup>().Initialize(data, quantityToDrop);

        // Remove the dropped quantity from the inventory
        RemoveItemFromSlot(slotIndex, quantityToDrop);
    }
   

    public void UseItem(int slotIndex)
    {
        if (IsSlotEmpty(slotIndex)) return;

        ItemData itemToUse = GetItemDataInSlot(slotIndex);
        Debug.Log($"Using item: {itemToUse.itemName}");

        //if (itemToUse.itemType == ItemType.Consumable)
        //{
        //    Debug.Log("This is a consumable, like a potion. You would heal the player here.");
        //    RemoveItemFromSlot(slotIndex, 1); // Consume one
        //}
        //else if (itemToUse.itemType == ItemType.Weapon)
        //{
        //    Debug.Log("This is a weapon. You would equip it here.");
        //    // (Equip logic would be more complex, often involving another inventory/character panel)
        //}
    }

    // --- HELPER & SAVE/LOAD FUNCTIONS (UNCHANGED) ---

    public ItemData GetItemDataInSlot(int index) => IsSlotEmpty(index) ? null : ItemDataManager.Instance.GetItemByID(_inventorySlots[index].itemID);
    public int GetQuantityInSlot(int index) => IsSlotEmpty(index) ? 0 : _inventorySlots[index].quantity;
    public bool IsSlotEmpty(int index) => index < 0 || index >= _inventorySlots.Count || _inventorySlots[index].IsEmpty();

    private void InitializeInventory()
    {
        _inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; i++)
        {
            _inventorySlots.Add(new InventorySlot(null, 0));
        }
    }


   

    private void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData { slots = _inventorySlots });
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
            if (data != null && data.slots.Count == inventorySize)
            {
                _inventorySlots = data.slots;
            }
        }
    }

    [System.Serializable]
    private class InventorySaveData { public List<InventorySlot> slots; }
}