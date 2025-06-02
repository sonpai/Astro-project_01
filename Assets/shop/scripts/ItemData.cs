//using UnityEngine;

//[CreateAssetMenu(fileName = "New Item", menuName = "Shop System/Item Data")]
//public class ItemData : ScriptableObject
//{
//    [Tooltip("Unique ID for this item (e.g., 'potion_health_01').")]
//    public string itemID;

//    public string itemName;
//    public Sprite itemIcon;
//    [TextArea(3, 5)]
//    public string description;

//    [Header("Shop Details")]
//    public int buyPrice = 10;
//    public int sellPrice = 5;
//    public bool canBeSold = true; // Can this item be sold back by the player?
//    public bool canBeBought = true; // Is this item listed in the shop to buy?

//    [Header("Stacking")]
//    public bool isStackable = false; // Can multiple of this item stack in the same inventory slot?
//    public int maxStackSize = 1;     // Maximum number of items in a stack (if stackable)
//}

// ItemData.cs (ensure these fields exist)
// ItemData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Tooltip("Unique ID for this item (e.g., 'potion_health_01'). Case-sensitive.")]
    public string itemID;
    public string itemName = "New Item";
    public Sprite itemIcon; // Assign this in the Inspector for each ItemData asset
    [TextArea(3, 10)]
    public string description = "Item Description.";

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 99; // Only relevant if isStackable is true

    [Header("Shop Details")]
    public bool canBeBought = true;  // If it can appear in the shop's "buy" section
    public int buyPrice = 10;
    public bool canBeSold = true;   // If the player can sell this item
    public int sellPrice = 5;

    // Optional: Add other game-specific properties
    // public enum ItemType { General, Potion, Weapon, Armor, Quest }
    // public ItemType itemType = ItemType.General;
    // public bool isConsumable = false;
    // public int effectValue = 0; // e.g., health restored, damage bonus
}

