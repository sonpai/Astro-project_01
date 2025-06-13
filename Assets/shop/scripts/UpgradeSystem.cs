//// UpgradeSystem.cs
//using UnityEngine;
//using System.Linq; // Required for Any()

//public class UpgradeSystem : MonoBehaviour
//{
//    public static UpgradeSystem Instance { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null) { Instance = this; }
//        else { Destroy(gameObject); }
//    }

//    public bool CheckUpgradeRequirements(ItemData itemToUpgrade, out string failureReason)
//    {
//        // 1. Check if the player has enough coins
//        if (!PlayerWallet.Instance.HasEnoughCoins(itemToUpgrade.upgradeCost))
//        {
//            failureReason = "Not enough coins!";
//            return false;
//        }

//        // 2. Check if the player has all the required materials
//        foreach (var req in itemToUpgrade.upgradeRequirements)
//        {
//            if (InventoryController.Instance.GetTotalQuantityOfItem(req.requiredItem.itemID) < req.requiredQuantity)
//            {
//                failureReason = $"Missing material: {req.requiredItem.itemName}";
//                return false;
//            }
//        }
//        failureReason = ""; // Success
//        return true;
//    }

//    public bool TryUpgradeItem(int itemSlotIndex)
//    {
//        // --- Get the item from the player's actual inventory ---
//        ItemData itemToUpgrade = InventoryController.Instance.GetItemDataInSlot(itemSlotIndex);

//        // --- Safety checks ---
//        if (itemToUpgrade == null || itemToUpgrade.nextLevelUpgrade == null)
//        {
//            Debug.LogError("Attempted to upgrade an item that is null or not upgradable.");
//            return false;
//        }
//        if (!CheckUpgradeRequirements(itemToUpgrade, out string reason))
//        {
//            Debug.LogWarning($"Upgrade failed: {reason}");
//            // Here you could show a notification to the player
//            return false;
//        }

//        // --- All checks passed, proceed with upgrade ---

//        // 1. Spend Coins
//        PlayerWallet.Instance.SpendCoins(itemToUpgrade.upgradeCost);

//        // 2. Remove Materials
//        foreach (var req in itemToUpgrade.upgradeRequirements)
//        {
//            InventoryController.Instance.RemoveItemByData(req.requiredItem, req.requiredQuantity);
//        }

//        // 3. Remove the old item
//        InventoryController.Instance.RemoveItemFromSlot(itemSlotIndex, 1);

//        // 4. Add the new, upgraded item
//        InventoryController.Instance.AddItem(itemToUpgrade.nextLevelUpgrade, 1);

//        Debug.Log($"Successfully upgraded {itemToUpgrade.itemName} to {itemToUpgrade.nextLevelUpgrade.itemName}!");
//        // Here you can show a success notification
//        return true;
//    }
//}