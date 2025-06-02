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

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemID;         // Unique ID for the item
    public string itemName;       // Display name
    public Sprite itemIcon;       // Icon for UI
    [TextArea]
    public string description;    // Description for UI

    public bool isStackable;
    public int maxStackSize = 99;

    // --- SHOP RELATED ---
    public bool canBeBought;      // << ADD THIS
    public int buyPrice;          // << ADD THIS

    // --- SELLING RELATED (already likely present from previous steps) ---
    public bool canBeSold;
    public int sellPrice;

    // public bool isConsumable; // Optional: for item usage logic
}
