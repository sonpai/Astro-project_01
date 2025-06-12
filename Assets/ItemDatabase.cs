// ItemDatabase.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> allGameItems; // Populate this in the Inspector with your ItemData assets

    public ItemData GetItemByID(string id)
    {
        if (string.IsNullOrEmpty(id) || allGameItems == null)
        {
            return null;
        }
        return allGameItems.FirstOrDefault(item => item != null && item.itemID == id);
    }
}