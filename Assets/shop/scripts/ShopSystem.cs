// ShopSystem.cs
using UnityEngine;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour
{
    [Header("Shop Inventory (Items the Shop Sells)")]
    public List<ItemData> shopStock; // Assign ItemData assets the shop sells in the Inspector

    // No need for player inventory reference here, we'll use InventoryController.Instance

    public List<ItemData> GetItemsForSale()
    {
        // You might have more complex logic here later (e.g., dynamic stock)
        return new List<ItemData>(shopStock);
    }

    public bool PlayerBuysItem(ItemData itemData)
    {
        if (itemData == null || !itemData.canBeBought) return false;
        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
        {
            Debug.LogError("ShopSystem: PlayerWallet or InventoryController not found!");
            return false;
        }

        if (PlayerWallet.Instance.SpendCoins(itemData.buyPrice))
        {
            if (InventoryController.Instance.AddItem(itemData, 1))
            {
                ShopUIManager.Instance?.ShowNotification($"{itemData.itemName} bought!");
                // Optionally: Logic for limited shop stock (remove from shopStock if finite)
                return true;
            }
            else
            {
                // Inventory full, refund coins
                PlayerWallet.Instance.AddCoins(itemData.buyPrice);
                ShopUIManager.Instance?.ShowNotification($"Inventory full! Could not buy {itemData.itemName}.");
                return false;
            }
        }
        else
        {
            ShopUIManager.Instance?.ShowNotification($"Not enough coins for {itemData.itemName}.");
            return false;
        }
    }

    public bool PlayerSellsItem(ItemData itemData, int quantityToSell = 1)
    {
        if (itemData == null || !itemData.canBeSold || quantityToSell <= 0) return false;
        if (PlayerWallet.Instance == null || InventoryController.Instance == null)
        {
            Debug.LogError("ShopSystem: PlayerWallet or InventoryController not found!");
            return false;
        }

        // Check if player actually has enough of the item
        // Note: InventoryController.RemoveItemByData will handle the quantity check and removal.
        // We just need to ensure it was successful before giving coins.

        if (InventoryController.Instance.RemoveItemByData(itemData, quantityToSell))
        {
            PlayerWallet.Instance.AddCoins(itemData.sellPrice * quantityToSell);
            ShopUIManager.Instance?.ShowNotification($"{quantityToSell}x {itemData.itemName} sold for {itemData.sellPrice * quantityToSell} coins!");
            // Optionally: Add sold item to shop's buyable stock if you want that mechanic
            return true;
        }
        else
        {
            ShopUIManager.Instance?.ShowNotification($"Could not sell {itemData.itemName}. Item not found or not enough quantity.");
            return false;
        }
    }
}