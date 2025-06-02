// ItemDataManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemDataManager : MonoBehaviour
{
    public static ItemDataManager Instance { get; private set; }

    [Tooltip("Assign ALL ItemData ScriptableObjects here.")]
    public List<ItemData> allGameItems;
    private Dictionary<string, ItemData> _itemDatabase;

    private void Awake()
    {
        Debug.Log("ItemDataManager: Awake() called."); // <<< ADD THIS LOG
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this persistent
            InitializeDatabase();
            Debug.Log("ItemDataManager: Instance SET and Database Initialized."); // <<< ADD THIS LOG
        }
        else if (Instance != this) // Ensure it's not just re-awakening on scene load
        {
            Debug.LogWarning("ItemDataManager: Another instance already exists. Destroying this one."); // <<< ADD THIS LOG
            Destroy(gameObject);
        }
    }


    private void InitializeDatabase()
    {
        _itemDatabase = new Dictionary<string, ItemData>();
        if (allGameItems == null)
        {
            Debug.LogError("ItemDataManager: allGameItems list is NOT ASSIGNED in Inspector!");
            allGameItems = new List<ItemData>(); // Prevent null reference
            return;
        }
        if (allGameItems.Count == 0)
        {
            Debug.LogWarning("ItemDataManager: allGameItems list is EMPTY. No items will be loaded.");
        }

        foreach (ItemData item in allGameItems)
        {
            if (item == null)
            {
                Debug.LogWarning("ItemDataManager: Found a NULL ItemData in allGameItems list.");
                continue;
            }
            if (string.IsNullOrEmpty(item.itemID))
            {
                Debug.LogWarning($"ItemDataManager: ItemData asset '{item.name}' has a NULL or EMPTY itemID!");
                continue;
            }
            if (!_itemDatabase.ContainsKey(item.itemID))
            {
                _itemDatabase.Add(item.itemID, item);
            }
            else
            {
                Debug.LogWarning($"ItemDataManager: DUPLICATE itemID '{item.itemID}' found for item '{item.name}'. Using the first instance ('{_itemDatabase[item.itemID].name}').");
            }
        }
        Debug.Log($"ItemDataManager: Database Initialized with {_itemDatabase.Count} unique items.");
    }

    public ItemData GetItemByID(string id)
    {
        if (_itemDatabase == null)
        {
            Debug.LogError("ItemDataManager: GetItemByID called but _itemDatabase is null! Was InitializeDatabase run?");
            return null;
        }
        if (string.IsNullOrEmpty(id)) return null;

        _itemDatabase.TryGetValue(id, out ItemData item);
        // if (item == null) Debug.LogWarning($"ItemDataManager: Item with ID '{id}' not found in database."); // Can be noisy
        return item;
    }
}