
// ItemDataManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ItemDataManager : MonoBehaviour
{
    public static ItemDataManager Instance { get; private set; }

    public List<ItemData> allGameItems;
    private Dictionary<string, ItemData> _itemDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        foreach (ItemData item in allGameItems)
        {
            if (item != null && !_itemDatabase.ContainsKey(item.itemID))
            {
                _itemDatabase.Add(item.itemID, item);
            }
        }
    }

    public ItemData GetItemByID(string id)
    {
        if (string.IsNullOrEmpty(id) || !_itemDatabase.ContainsKey(id)) return null;
        return _itemDatabase[id];
    }
}