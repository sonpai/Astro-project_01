using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shop System/Item Data")]
public class ItemData : ScriptableObject
{
    [Tooltip("Unique ID for this item (e.g., 'potion_health_01').")]
    public string itemID;

    public string itemName;
    public Sprite itemIcon;
    [TextArea(3, 5)]
    public string description;

    [Header("Shop Details")]
    public int buyPrice = 10;
    public int sellPrice = 5;
    public bool canBeSold = true; // Can this item be sold back by the player?
    public bool canBeBought = true; // Is this item listed in the shop to buy?

    [Header("Stacking")]
    public bool isStackable = false; // Can multiple of this item stack in the same inventory slot?
    public int maxStackSize = 1;     // Maximum number of items in a stack (if stackable)
}
