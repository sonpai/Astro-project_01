// UpgradeSystem.cs
using UnityEngine;
using System;

public class UpgradeSystem : MonoBehaviour
{
    public static event Action<string> OnUpgradeStatus;

    public void TryUpgradeItem(int inventorySlotIndex)
    {
        ItemData itemToUpgrade = InventoryController.Instance.GetItemDataInSlot(inventorySlotIndex);

        // 1. Check if item is upgradable
        if (itemToUpgrade == null || itemToUpgrade.nextLevelUpgrade == null)
        {
            OnUpgradeStatus?.Invoke("This item cannot be upgraded.");
            return;
        }

        // 2. Check coin cost
        if (!PlayerWallet.Instance.SpendCoins(itemToUpgrade.upgradeCost))
        {
            OnUpgradeStatus?.Invoke("Not enough coins!");
            return;
        }

        // 3. Check material requirements
        if (!InventoryController.Instance.HasItems(itemToUpgrade.upgradeRequirements))
        {
            OnUpgradeStatus?.Invoke("Missing required materials!");
            PlayerWallet.Instance.AddCoins(itemToUpgrade.upgradeCost); // Refund coins
            return;
        }

        // 4. All checks passed, perform the upgrade
        // Spend materials
        foreach (var req in itemToUpgrade.upgradeRequirements)
        {
            InventoryController.Instance.RemoveItemByData(req.requiredItem, req.requiredQuantity);
        }

        // Remove the old item
        InventoryController.Instance.RemoveItemFromSlot(inventorySlotIndex, 1);

        // Add the new, upgraded item
        InventoryController.Instance.AddItem(itemToUpgrade.nextLevelUpgrade, 1);

        OnUpgradeStatus?.Invoke($"Success! Upgraded to {itemToUpgrade.nextLevelUpgrade.itemName}.");
    }
}