//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//[CreateAssetMenu]

//public class InventorySO : ScriptableObject
//{
//    [SerializeField]
//    private List<InventoryItem> inventoryItems;

//    [field: SerializeField]
//    public int Size { get; private set; } = 24;


//    public void Initialize()
//    {
//        inventoryItems = new List<InventoryItem>();
       
//        for (int i = 0; i < Size; i++)
//        {
//            inventoryItems.Add(InventoryItem.GetEmptyItem());
//        }
//    }

//    public void AddItem(ItemSO item, int quantity)
//    {
//            for (int i = 0; i < inventoryItems.Count; i++)
//            {
//            if (inventoryItems[i].IsEmpty)
//            {
//                inventoryItems[i] = new InventoryItem
//                {
//                    item = item,
//                    quantity = quantity
//                };
//            }
//            }
//    }
//    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
//    {
//        Dictionary<int, InventoryItem> returnValue =
//            new Dictionary<int, InventoryItem>();

//        for (int i = 0; i < inventoryItems.Count; i++)
//        {
//            if (inventoryItems[i].IsEmpty)
//                continue;
//            returnValue[i] = inventoryItems[i];
//        }
//        return returnValue;
//    }

//}

//[Serializable]
//public struct InventoryItem
//{
//    public int quantity;
//    public ItemSO item;
//    //public List<ItemParameter> itemState;
//    public bool IsEmpty => item == null;

//    public InventoryItem ChangeQuantity(int newQuantity)
//    {
//        return new InventoryItem
//        {
//            item = this.item,
//            quantity = newQuantity,
//            //itemState = new List<ItemParameter>(this.itemState)
//        };
//    }

//    public static InventoryItem GetEmptyItem()
//        => new InventoryItem
//        {
//            item = null,
//            quantity = 0,
//           // itemState = new List<ItemParameter>()
//        };
//}

