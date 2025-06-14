// --- The ONE AND ONLY ItemType.cs file in your project ---

using UnityEngine;

// This enum contains ALL possible item types from your different systems.
public enum ItemTypes
{
    Weapon,
    Armor,
    Consumable,
    Material
    // Add any other types you might need from other scripts here
}

// You can keep other related enums here too.
public enum StatType
{
    Damage,
    Armor
}

public class EnumScript : MonoBehaviour
{
    // This class can remain empty.
}