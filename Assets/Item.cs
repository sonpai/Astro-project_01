// Item.cs
using UnityEngine;

public enum ItemType
{
    Potion,
    Equipment, // Example
    Material,  // Example
    QuestItem  // Example
}

public enum PotionEffect
{
    None,
    Heal,
    DamageBoost
}

[CreateAssetMenu(fileName = "New Item", menuName = "RPG System/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    public ItemType itemType = ItemType.Material;
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Potion Specific")]
    public PotionEffect potionEffect = PotionEffect.None;
    public float effectDuration = 10f; // For temporary effects like DamageBoost
    public int effectValue = 0;       // e.g., +1 damage for DamageBoost, or heal amount

    public virtual void Use(GameObject user)
    {
        Debug.Log($"Using {itemName} on {user.name}. Base Use method, override for specific effects.");
        // Specific logic for potion effects will be handled in PlayerCombat or PlayerHealth
    }
}