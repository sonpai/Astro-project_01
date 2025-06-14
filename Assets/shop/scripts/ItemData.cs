//// ItemData.cs (Corrected and with Upgrade Fields)
//using UnityEngine;
//using System.Collections.Generic; // <<< THIS LINE FIXES THE ERROR

//[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]

//public class ItemData : ScriptableObject
//{
//    public string itemID;
//    public string itemName;
//    public Sprite itemIcon;
//    [TextArea(3, 5)]
//    public string description;

//    [Header("Stacking")]
//    public bool isStackable = true;
//    public int maxStackSize = 99;

//    [Header("Shop Details")]
//    public bool canBeBought = true;
//    public int buyPrice = 10;
//    public bool canBeSold = true;
//    public int sellPrice = 5;

//    [Header("Upgrade Info")]
//    [Tooltip("The item this one will turn into when upgraded. Leave empty if not upgradable.")]
//    public ItemData nextLevelUpgrade;

//    //[Tooltip("The cost in coins to perform the upgrade.")]
//    public int upgradeCost;

//    [Tooltip("A list of all materials and quantities needed for the upgrade.")]
//    public List<UpgradeRequirement> upgradeRequirements; // This now works because of the "using" line

//    [Header("Item Type")]
//    public ItemType itemType;

//    //[Header("Item Stats")]
//    //public List<StatModifier> statModifiers;
//}

// ItemData.cs
using UnityEngine;
using System.Collections.Generic;

// These enums can be used by any script
//public enum ItemType { Weapon, Armor, Consumable, Material }
//public enum StatType { Damage, Armor }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string description;
    public ItemType itemType;

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Shop & Selling")]
    public bool canBeBought = true;
    public int buyPrice = 10;
    public bool canBeSold = true;
    public int sellPrice = 5;
}

