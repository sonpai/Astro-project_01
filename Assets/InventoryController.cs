using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    [Header("Settings")]
    public int inventorySize = 24;

    [Header("Item Drop")]
    [SerializeField] private GameObject groundItemPrefab;

    // This is the reference to the UI we will toggle
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
        OnInventoryRefreshed?.Invoke();
    }

    // --- THIS IS THE FIX ---
    // This Update method runs constantly because this GameObject is never disabled.
    private void Update()
    {
        // Listen for the 'I' key to toggle the inventory UI
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Make sure we actually have a UI to talk to
            if (_currentInventoryUI != null)
            {
                // If the inventory UI is currently active, hide it. Otherwise, show it.
                if (_currentInventoryUI.gameObject.activeSelf)
                {
                    _currentInventoryUI.Hide();
                }
                else
                {
                    _currentInventoryUI.Show();
                }
            }
        }
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

    // ... (The rest of your InventoryController.cs script is unchanged and correct) ...
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
        if (quantity <= 0) quantity = 1;

        ItemData data = GetItemDataInSlot(slotIndex);
        int quantityInSlot = GetQuantityInSlot(slotIndex);
        int quantityToDrop = Mathf.Min(quantity, quantityInSlot);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 dropPosition = player != null ? player.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) : Vector3.zero;

        GameObject droppedItemObj = Instantiate(groundItemPrefab, dropPosition, Quaternion.identity);
        droppedItemObj.GetComponent<ItemPickup>().Initialize(data, quantityToDrop);

        RemoveItemFromSlot(slotIndex, quantityToDrop);
    }

    public void UseItem(int slotIndex)
    {
        if (IsSlotEmpty(slotIndex)) return;
        ItemData itemToUse = GetItemDataInSlot(slotIndex);

        switch (itemToUse.itemType)
        {
            case ItemTypes.Consumable:
                Debug.Log($"Used {itemToUse.itemName}.");
                RemoveItemFromSlot(slotIndex, 1);
                break;
            case ItemTypes.Weapon:
            case ItemTypes.Armor:
                Debug.Log($"Equipped {itemToUse.itemName}.");
                break;
            case ItemTypes.Material:
                Debug.Log($"Cannot 'use' {itemToUse.itemName}.");
                break;
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

    public void RemoveItemFromSlot(int slotIndex, int quantity = 1)
    {
        if (IsSlotEmpty(slotIndex) || quantity <= 0) return;

        ItemData data = GetItemDataInSlot(slotIndex);
        _inventorySlots[slotIndex].quantity -= quantity;

        if (_inventorySlots[slotIndex].quantity <= 0)
        {
            _inventorySlots[slotIndex] = new InventorySlot(null, 0);
            OnInventorySlotUpdated?.Invoke(slotIndex, null, 0);
        }
        else
        {
            OnInventorySlotUpdated?.Invoke(slotIndex, data, _inventorySlots[slotIndex].quantity);
        }
        SaveInventory();
    }

    public bool IsSlotEmpty(int index) => _inventorySlots[index].IsEmpty();
    public ItemData GetItemDataInSlot(int index) => IsSlotEmpty(index) ? null : ItemDataManager.Instance.GetItemByID(_inventorySlots[index].itemID);
    public int GetQuantityInSlot(int index) => IsSlotEmpty(index) ? 0 : _inventorySlots[index].quantity;

    private void InitializeInventory()
    {
        _inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; i++) _inventorySlots.Add(new InventorySlot(null, 0));
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

    // --- NEW AND IMPROVED METHODS ---

    public bool HasItems(List<UpgradeRequirement> requirements)
    {
        foreach (var req in requirements)
        {
            if (GetTotalQuantityOfItem(req.requiredItem.itemID) < req.requiredQuantity)
            {
                return false; // Not enough of this material
            }
        }
        return true;
    }

    public bool RemoveItemByData(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        int quantityToRemove = quantity;
        // Iterate backwards when removing from a list
        for (int i = _inventorySlots.Count - 1; i >= 0; i--)
        {
            if (!_inventorySlots[i].IsEmpty() && _inventorySlots[i].itemID == item.itemID)
            {
                int amountInSlot = _inventorySlots[i].quantity;
                int amountToRemoveFromSlot = Mathf.Min(quantityToRemove, amountInSlot);

                RemoveItemFromSlot(i, amountToRemoveFromSlot);
                quantityToRemove -= amountToRemoveFromSlot;

                if (quantityToRemove <= 0) break;
            }
        }
        SaveInventory();
        return quantityToRemove <= 0; // Return true if all required items were removed
    }

    public int GetTotalQuantityOfItem(string itemID)
    {
        return _inventorySlots
            .Where(slot => !slot.IsEmpty() && slot.itemID == itemID)
            .Sum(slot => slot.quantity);
    }

    // ... (All other methods like AddItem, SwapItems, Save/Load remain the same)
    // Ensure you have the full script from the previous steps.


[System.Serializable]
    private class InventorySaveData { public List<InventorySlot> slots; }
}
