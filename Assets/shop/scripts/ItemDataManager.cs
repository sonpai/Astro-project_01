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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this persistent
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        _itemDatabase = new Dictionary<string, ItemData>();
        if (allGameItems == null)
        {
            Debug.LogError("ItemDataManager: allGameItems list is not assigned!");
            allGameItems = new List<ItemData>(); // Prevent null reference if used later
            return;
        }

        foreach (ItemData item in allGameItems)
        {
            if (item == null)
            {
                Debug.LogWarning("ItemDataManager: Found a null ItemData in allGameItems list.");
                continue;
            }
            if (string.IsNullOrEmpty(item.itemID))
            {
                Debug.LogWarning($"ItemDataManager: ItemData '{item.name}' has a null or empty itemID!");
                continue;
            }
            if (!_itemDatabase.ContainsKey(item.itemID))
            {
                _itemDatabase.Add(item.itemID, item);
            }
            else
            {
                Debug.LogWarning($"ItemDataManager: Duplicate itemID '{item.itemID}' found. Using the first instance.");
            }
        }
        Debug.Log($"Item Database Initialized with {_itemDatabase.Count} items.");
    }

    public ItemData GetItemByID(string id)
    {
        _itemDatabase.TryGetValue(id, out ItemData item);
        if (item == null)
        {
            // Debug.LogWarning($"ItemDataManager: Item with ID '{id}' not found.");
        }
        return item;
    }
}