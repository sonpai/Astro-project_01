// LootDrop.cs
using UnityEngine;

[System.Serializable]
public class LootDrop
{
    public Item itemData; // Reference to the Item ScriptableObject
    public int minQuantity = 1;
    public int maxQuantity = 1;
    [Range(0f, 1f)]
    public float dropChance = 0.5f; // 0.0 to 1.0 (0% to 100%)
}