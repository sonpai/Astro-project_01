//////// ShopSystem.cs
//////using UnityEngine;
//////using System.Collections.Generic;

//////public class ShopSystem : MonoBehaviour
//////{
//////    [Header("Shop Inventory (Items this Shop Sells)")]
//////    public List<ItemData> shopStock; // Assign ItemData ScriptableObjects in the Inspector

//////    // Method for ShopUIManager to get the current list of items for sale
//////    public List<ItemData> GetItemsForSale()
//////    {
//////        // Could add logic here for dynamic stock, limited quantities, etc.
//////        // For now, just returns the assigned list.
//////        return new List<ItemData>(shopStock); // Return a copy to prevent external modification
//////    }
//////    public List<ItemData> itemsForSale;
//////    public static event Action<string> OnTransactionStatus;

//////    public void TryPurchaseItem(ItemData item)
//////    {
//////        if (!PlayerWallet.Instance.SpendCoins(item.buyPrice))
//////        {
//////            OnTransactionStatus?.Invoke("Not enough coins!");
//////            return;
//////        }
//////        if (!InventoryController.Instance.AddItem(item, 1))
//////        {
//////            OnTransactionStatus?.Invoke("Inventory is full!");
//////            PlayerWallet.Instance.AddCoins(item.buyPrice); // Refund
//////            return;
//////        }
//////        OnTransactionStatus?.Invoke($"Purchased {item.itemName}!");
//////    }

//////    public void TrySellItem(int slotIndex)
//////    {
//////        ItemData itemToSell = InventoryController.Instance.GetItemDataInSlot(slotIndex);
//////        if (itemToSell == null || !itemToSell.canBeSold) return;

//////        InventoryController.Instance.RemoveItemFromSlot(slotIndex, 1);
//////        PlayerWallet.Instance.AddCoins(itemToSell.sellPrice);
//////        OnTransactionStatus?.Invoke($"Sold {itemToSell.itemName}!");
//////    }

//////    // Called by ShopItemUIController when a "Buy" button is clicked
//////    public bool PlayerBuysItem(ItemData itemDataToBuy, int quantity = 1)
//////    {
//////        if (itemDataToBuy == null || quantity <= 0)
//////        {
//////            Debug.LogError("PlayerBuysItem: Invalid itemData or quantity.");
//////            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
//////            return false;
//////        }

//////        if (!itemDataToBuy.canBeBought)
//////        {
//////            ShopUIManager.Instance?.ShowNotification($"{itemDataToBuy.itemName} is not for sale.");
//////            return false;
//////        }

//////        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
//////        {
//////            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
//////            ShopUIManager.Instance?.ShowNotification("System Error!");
//////            return false;
//////        }

//////        int totalCost = itemDataToBuy.buyPrice * quantity;
//////        if (PlayerWallet.Instance.SpendCoins(totalCost)) // SpendCoins returns true if successful
//////        {
//////            // Attempt to add item(s) to player's inventory
//////            if (InventoryController.Instance.AddItem(itemDataToBuy, quantity))
//////            {
//////                ShopUIManager.Instance?.ShowNotification($"Bought {quantity}x {itemDataToBuy.itemName} for {totalCost} coins.");
//////                // Optionally: Logic for limited shop stock (e.g., remove 'itemDataToBuy' from 'shopStock' or reduce its count)
//////                return true;
//////            }
//////            else
//////            {
//////                // Inventory was full or another issue adding item, so refund the coins
//////                PlayerWallet.Instance.AddCoins(totalCost);
//////                ShopUIManager.Instance?.ShowNotification($"Inventory full! Could not buy {itemDataToBuy.itemName}.");
//////                return false;
//////            }
//////        }
//////        else
//////        {
//////            ShopUIManager.Instance?.ShowNotification($"Not enough coins for {itemDataToBuy.itemName}. Need {totalCost}.");
//////            return false;
//////        }
//////    }

//////    // Called by ShopItemUIController when a "Sell" button is clicked
//////    public bool PlayerSellsItem(ItemData itemDataToSell, int quantityToSell = 1)
//////    {
//////        if (itemDataToSell == null || quantityToSell <= 0)
//////        {
//////            Debug.LogError("PlayerSellsItem: Invalid itemData or quantity.");
//////            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
//////            return false;
//////        }

//////        if (!itemDataToSell.canBeSold || itemDataToSell.sellPrice <= 0)
//////        {
//////            ShopUIManager.Instance?.ShowNotification($"{itemDataToSell.itemName} cannot be sold or has no value.");
//////            return false;
//////        }

//////        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
//////        {
//////            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
//////            ShopUIManager.Instance?.ShowNotification("System Error!");
//////            return false;
//////        }

//////        // InventoryController.RemoveItemByData will check if the player has enough and remove them.
//////        if (InventoryController.Instance.RemoveItemByData(itemDataToSell, quantityToSell))
//////        {
//////            int earnings = itemDataToSell.sellPrice * quantityToSell;
//////            PlayerWallet.Instance.AddCoins(earnings);
//////            ShopUIManager.Instance?.ShowNotification($"Sold {quantityToSell}x {itemDataToSell.itemName} for {earnings} coins!");
//////            // Optionally: Add the sold item to the shop's 'shopStock' if you want vendors to accumulate player-sold items
//////            return true;
//////        }
//////        else
//////        {
//////            // This message might be redundant if RemoveItemByData already logs a more specific reason
//////            ShopUIManager.Instance?.ShowNotification($"Could not sell {quantityToSell}x {itemDataToSell.itemName}. (Not enough in inventory or item not found).");
//////            return false;
//////        }
//////    }
//////}

////// ShopSystem.cs
////using UnityEngine;
////using System.Collections.Generic;

////public class ShopSystem : MonoBehaviour
////{
////    [Header("Shop Inventory (Items this Shop Sells)")]
////    public List<ItemData> shopStock; // Assign ItemData ScriptableObjects in the Inspector

////    // Method for ShopUIManager to get the current list of items for sale
////    public List<ItemData> GetItemsForSale()
////    {
////        // Could add logic here for dynamic stock, limited quantities, etc.
////        // For now, just returns the assigned list.
////        return new List<ItemData>(shopStock); // Return a copy to prevent external modification
////    }

////    // Called by ShopItemUIController when a "Buy" button is clicked
////    public bool PlayerBuysItem(ItemData itemDataToBuy, int quantity = 1)
////    {
////        if (itemDataToBuy == null || quantity <= 0)
////        {
////            Debug.LogError("PlayerBuysItem: Invalid itemData or quantity.");
////            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
////            return false;
////        }

////        if (!itemDataToBuy.canBeBought)
////        {
////            ShopUIManager.Instance?.ShowNotification($"{itemDataToBuy.itemName} is not for sale.");
////            return false;
////        }

////        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
////        {
////            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
////            ShopUIManager.Instance?.ShowNotification("System Error!");
////            return false;
////        }

////        int totalCost = itemDataToBuy.buyPrice * quantity;
////        if (PlayerWallet.Instance.SpendCoins(totalCost)) // SpendCoins returns true if successful
////        {
////            // Attempt to add item(s) to player's inventory
////            if (InventoryController.Instance.AddItem(itemDataToBuy, quantity))
////            {
////                ShopUIManager.Instance?.ShowNotification($"Bought {quantity}x {itemDataToBuy.itemName} for {totalCost} coins.");
////                // Optionally: Logic for limited shop stock (e.g., remove 'itemDataToBuy' from 'shopStock' or reduce its count)
////                return true;
////            }
////            else
////            {
////                // Inventory was full or another issue adding item, so refund the coins
////                PlayerWallet.Instance.AddCoins(totalCost);
////                ShopUIManager.Instance?.ShowNotification($"Inventory full! Could not buy {itemDataToBuy.itemName}.");
////                return false;
////            }
////        }
////        else
////        {
////            ShopUIManager.Instance?.ShowNotification($"Not enough coins for {itemDataToBuy.itemName}. Need {totalCost}.");
////            return false;
////        }
////    }

////    // Called by ShopItemUIController when a "Sell" button is clicked
////    public bool PlayerSellsItem(ItemData itemDataToSell, int quantityToSell = 1)
////    {
////        if (itemDataToSell == null || quantityToSell <= 0)
////        {
////            Debug.LogError("PlayerSellsItem: Invalid itemData or quantity.");
////            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
////            return false;
////        }

////        if (!itemDataToSell.canBeSold || itemDataToSell.sellPrice <= 0)
////        {
////            ShopUIManager.Instance?.ShowNotification($"{itemDataToSell.itemName} cannot be sold or has no value.");
////            return false;
////        }

////        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
////        {
////            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
////            ShopUIManager.Instance?.ShowNotification("System Error!");
////            return false;
////        }

////        // InventoryController.RemoveItemByData will check if the player has enough and remove them.
////        if (InventoryController.Instance.RemoveItemByData(itemDataToSell, quantityToSell))
////        {
////            int earnings = itemDataToSell.sellPrice * quantityToSell;
////            PlayerWallet.Instance.AddCoins(earnings);
////            ShopUIManager.Instance?.ShowNotification($"Sold {quantityToSell}x {itemDataToSell.itemName} for {earnings} coins!");
////            // Optionally: Add the sold item to the shop's 'shopStock' if you want vendors to accumulate player-sold items
////            return true;
////        }
////        else
////        {
////            // This message might be redundant if RemoveItemByData already logs a more specific reason
////            ShopUIManager.Instance?.ShowNotification($"Could not sell {quantityToSell}x {itemDataToSell.itemName}. (Not enough in inventory or item not found).");
////            return false;
////        }
////    }
////}

//// File: ShopSystem.cs
//using UnityEngine;
//using System.Collections.Generic;

//public class ShopSystem : MonoBehaviour
//{
//    [Header("Items this Shop Sells")]
//    public List<ItemData> shopStock; // Assign ItemData ScriptableObjects in the Inspector

//    // Method for the UI manager to get the current list of items for sale
//    public List<ItemData> GetItemsForSale()
//    {
//        return new List<ItemData>(shopStock); // Return a copy to prevent external modification
//    }

//    // Called by ShopItemUIController when a "Buy" button is clicked
//    public bool PlayerBuysItem(ItemData itemDataToBuy, int quantity = 1)
//    {
//        if (itemDataToBuy == null || quantity <= 0)
//        {
//            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
//            return false;
//        }

//        if (!itemDataToBuy.canBeBought)
//        {
//            ShopUIManager.Instance?.ShowNotification($"{itemDataToBuy.itemName} is not for sale.");
//            return false;
//        }

//        // Ensure other systems are ready
//        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
//        {
//            ShopUIManager.Instance?.ShowNotification("System Error!");
//            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
//            return false;
//        }

//        int totalCost = itemDataToBuy.buyPrice * quantity;
//        if (PlayerWallet.Instance.SpendCoins(totalCost)) // Method should return true on success
//        {
//            if (InventoryController.Instance.AddItem(itemDataToBuy, quantity))
//            {
//                ShopUIManager.Instance?.ShowNotification($"Bought {itemDataToBuy.itemName} for {totalCost} coins.");
//                return true;
//            }
//            else
//            {
//                // Refund if inventory is full
//                PlayerWallet.Instance.AddCoins(totalCost);
//                ShopUIManager.Instance?.ShowNotification($"Inventory full! Could not buy {itemDataToBuy.itemName}.");
//                return false;
//            }
//        }
//        else
//        {
//            ShopUIManager.Instance?.ShowNotification($"Not enough coins. Need {totalCost}.");
//            return false;
//        }
//    }

//    // Called by ShopItemUIController when a "Sell" button is clicked
//    public bool PlayerSellsItem(ItemData itemDataToSell, int quantityToSell = 1)
//    {
//        if (itemDataToSell == null || quantityToSell <= 0)
//        {
//            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
//            return false;
//        }

//        if (!itemDataToSell.canBeSold || itemDataToSell.sellPrice <= 0)
//        {
//            ShopUIManager.Instance?.ShowNotification($"{itemDataToSell.itemName} cannot be sold.");
//            return false;
//        }

//        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
//        {
//            ShopUIManager.Instance?.ShowNotification("System Error!");
//            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
//            return false;
//        }

//        if (InventoryController.Instance.RemoveItemByData(itemDataToSell, quantityToSell))
//        {
//            int earnings = itemDataToSell.sellPrice * quantityToSell;
//            PlayerWallet.Instance.AddCoins(earnings);
//            ShopUIManager.Instance?.ShowNotification($"Sold {itemDataToSell.itemName} for {earnings} coins!");
//            return true;
//        }
//        else
//        {
//            // The RemoveItemByData method in InventoryController should ideally handle the "not enough items" case.
//            // This is a fallback message.
//            ShopUIManager.Instance?.ShowNotification($"Could not sell {itemDataToSell.itemName}.");
//            return false;
//        }
//    }
//}

// File: ShopSystem.cs
using UnityEngine;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour
{
    [Header("Items this Shop Sells")]
    public List<ItemData> shopStock; // Assign ItemData ScriptableObjects in the Inspector

    // Method for the UI manager to get the current list of items for sale
    public List<ItemData> GetItemsForSale()
    {
        return new List<ItemData>(shopStock); // Return a copy to prevent external modification
    }

    // Called by ShopItemUIController when a "Buy" button is clicked
    public bool PlayerBuysItem(ItemData itemDataToBuy, int quantity = 1)
    {
        if (itemDataToBuy == null || quantity <= 0)
        {
            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
            return false;
        }

        if (!itemDataToBuy.canBeBought)
        {
            ShopUIManager.Instance?.ShowNotification($"{itemDataToBuy.itemName} is not for sale.");
            return false;
        }

        // Ensure other systems are ready
        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
        {
            ShopUIManager.Instance?.ShowNotification("System Error!");
            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
            return false;
        }

        int totalCost = itemDataToBuy.buyPrice * quantity;
        if (PlayerWallet.Instance.SpendCoins(totalCost)) // Method should return true on success
        {
            if (InventoryController.Instance.AddItem(itemDataToBuy, quantity))
            {
                ShopUIManager.Instance?.ShowNotification($"Bought {itemDataToBuy.itemName} for {totalCost} coins.");
                return true;
            }
            else
            {
                // Refund if inventory is full
                PlayerWallet.Instance.AddCoins(totalCost);
                ShopUIManager.Instance?.ShowNotification($"Inventory full! Could not buy {itemDataToBuy.itemName}.");
                return false;
            }
        }
        else
        {
            ShopUIManager.Instance?.ShowNotification($"Not enough coins. Need {totalCost}.");
            return false;
        }
    }

    // Called by ShopItemUIController when a "Sell" button is clicked
    public bool PlayerSellsItem(ItemData itemDataToSell, int quantityToSell = 1)
    {
        if (itemDataToSell == null || quantityToSell <= 0)
        {
            ShopUIManager.Instance?.ShowNotification("Transaction Error!");
            return false;
        }

        if (!itemDataToSell.canBeSold || itemDataToSell.sellPrice <= 0)
        {
            ShopUIManager.Instance?.ShowNotification($"{itemDataToSell.itemName} cannot be sold.");
            return false;
        }

        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
        {
            ShopUIManager.Instance?.ShowNotification("System Error!");
            Debug.LogError("ShopSystem: PlayerWallet or InventoryController instance not found!");
            return false;
        }

        if (InventoryController.Instance.RemoveItemByData(itemDataToSell, quantityToSell))
        {
            int earnings = itemDataToSell.sellPrice * quantityToSell;
            PlayerWallet.Instance.AddCoins(earnings);
            ShopUIManager.Instance?.ShowNotification($"Sold {itemDataToSell.itemName} for {earnings} coins!");
            return true;
        }
        else
        {
            // The RemoveItemByData method in InventoryController should ideally handle the "not enough items" case.
            // This is a fallback message.
            ShopUIManager.Instance?.ShowNotification($"Could not sell {itemDataToSell.itemName}.");
            return false;
        }
    }
}