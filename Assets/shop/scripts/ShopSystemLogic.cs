//// ShopSystem.cs
//using UnityEngine;
//using System.Collections.Generic;
//using System;

//public class ShopSystem : MonoBehaviour
//{
//    [Tooltip("Items available for purchase in this shop.")]
//    public List<ItemData> itemsForSale;

//    public static event Action<string, Color> OnTransactionStatus; // Message, Color for UI

//    // This could also be a Singleton if shop logic needs to be globally accessible.
//    // For now, assuming it's a component in the shop scene.

//    public bool TryPurchaseItem(ItemData itemToBuy)
//    {
//        if (itemToBuy == null)
//        {
//            OnTransactionStatus?.Invoke("Invalid item selected.", Color.red);
//            return false;
//        }

//        if (PlayerWallet.Instance.HasEnoughCoins(itemToBuy.buyPrice))
//        {
//            PlayerWallet.Instance.SpendCoins(itemToBuy.buyPrice);
//            PlayerInventory.Instance.AddItem(itemToBuy.itemID, 1);
//            OnTransactionStatus?.Invoke($"Purchased {itemToBuy.itemName} for {itemToBuy.buyPrice} coins.", Color.green);
//            Debug.Log($"Player bought {itemToBuy.itemName}");
//            return true;
//        }
//        else
//        {
//            OnTransactionStatus?.Invoke($"Not enough coins to buy {itemToBuy.itemName} ({itemToBuy.buyPrice} coins needed).", Color.yellow);
//            Debug.LogWarning($"Player cannot afford {itemToBuy.itemName}");
//            return false;
//        }
//    }

//    public bool TrySellItem(ItemData itemToSell, int quantityToSell = 1)
//    {
//        if (itemToSell == null || !itemToSell.canBeSold || quantityToSell <= 0)
//        {
//            OnTransactionStatus?.Invoke("Cannot sell this item or invalid quantity.", Color.red);
//            return false;
//        }

//        if (PlayerInventory.Instance.GetItemQuantity(itemToSell.itemID) >= quantityToSell)
//        {
//            bool removed = PlayerInventory.Instance.RemoveItem(itemToSell.itemID, quantityToSell);
//            if (removed)
//            {
//                int coinsGained = itemToSell.sellPrice * quantityToSell;
//                PlayerWallet.Instance.AddCoins(coinsGained);
//                OnTransactionStatus?.Invoke($"Sold {quantityToSell}x {itemToSell.itemName} for {coinsGained} coins.", Color.green);
//                Debug.Log($"Player sold {quantityToSell}x {itemToSell.itemName}");
//                return true;
//            }
//            else
//            {
//                OnTransactionStatus?.Invoke($"Error removing {itemToSell.itemName} from inventory.", Color.red);
//                return false;
//            }
//        }
//        else
//        {
//            OnTransactionStatus?.Invoke($"Not enough {itemToSell.itemName} to sell.", Color.yellow);
//            Debug.LogWarning($"Player does not have enough {itemToSell.itemName} to sell {quantityToSell}.");
//            return false;
//        }
//    }
//}