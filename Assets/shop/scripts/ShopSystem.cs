// ShopSystem.cs (Modified to use InventoryController)
using UnityEngine;
using System.Collections.Generic;
using System;

public class ShopSystem : MonoBehaviour
{
    [Tooltip("Items available for purchase in this shop.")]
    public List<ItemData> itemsForSale;

    public static event Action<string, Color> OnTransactionStatus;

    public bool TryPurchaseItem(ItemData itemToBuy)
    {
        if (itemToBuy == null || PlayerWallet.Instance == null || InventoryController.Instance == null || ItemDataManager.Instance == null)
        {
            OnTransactionStatus?.Invoke("System error or invalid item.", Color.red);
            return false;
        }

        if (PlayerWallet.Instance.HasEnoughCoins(itemToBuy.buyPrice))
        {
            PlayerWallet.Instance.SpendCoins(itemToBuy.buyPrice);
            // Use the AddItem that takes ItemData
            InventoryController.Instance.AddItem(itemToBuy, 1);
            OnTransactionStatus?.Invoke($"Purchased {itemToBuy.itemName} for {itemToBuy.buyPrice} coins.", Color.green);
            return true;
        }
        else
        {
            OnTransactionStatus?.Invoke($"Not enough coins ({itemToBuy.buyPrice} needed).", Color.yellow);
            return false;
        }
    }

    // Selling now needs to know which item (by ItemData) and how many.
    // The ShopUIManager will iterate through InventoryController's slots to display sellable items.
    // When a "Sell" button on a specific item in the sell UI is clicked, it will pass its ItemData.
    public bool TrySellItem(ItemData itemToSell, int quantityToSell = 1)
    {
        if (itemToSell == null || !itemToSell.canBeSold || quantityToSell <= 0 || PlayerWallet.Instance == null || InventoryController.Instance == null)
        {
            OnTransactionStatus?.Invoke("Cannot sell item or system error.", Color.red);
            return false;
        }

        // Check quantity using InventoryController
        // Note: GetItemQuantity in the new InventoryController would need an ItemData argument or you'd iterate.
        // For simplicity, let's assume we modify InventoryController to have a GetItemQuantity(ItemData item)
        // or ShopUIManager provides the slot index.
        // For now, let's use RemoveItemByData which handles finding the item.

        bool removed = InventoryController.Instance.RemoveItemByData(itemToSell, quantityToSell);
        if (removed)
        {
            int coinsGained = itemToSell.sellPrice * quantityToSell;
            PlayerWallet.Instance.AddCoins(coinsGained);
            OnTransactionStatus?.Invoke($"Sold {quantityToSell}x {itemToSell.itemName} for {coinsGained} coins.", Color.green);
            return true;
        }
        else
        {
             // RemoveItemByData will log if not enough. We can add a more specific message.
             OnTransactionStatus?.Invoke($"Not enough {itemToSell.itemName} to sell or error.", Color.yellow);
             return false;
        }
    }
}