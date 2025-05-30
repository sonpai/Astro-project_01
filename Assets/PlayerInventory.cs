//// PlayerInventory.cs
//using UnityEngine;
//using System;
//using System.Collections.Generic;
//using System.Linq;


//[System.Serializable]
//public class InventorySlot
//{
//    public Item itemData;
//    public int quantity;

//    public InventorySlot()
//    {
//        itemData = null;
//        quantity = 0;
//    }

//    public void Clear()
//    {
//        itemData = null;
//        quantity = 0;
//    }

//    public void AddQuantity(int amount)
//    {
//        quantity += amount;
//    }

//    public void Set(Item item, int amount)
//    {
//        itemData = item;
//        quantity = amount;
//    }
//}

//public class PlayerInventory : MonoBehaviour
//{
//    public const int TOTAL_SLOTS = 40;
//    public const int VISIBLE_SLOTS_DEFAULT = 10; // First row

//    public InventorySlot[] slots = new InventorySlot[TOTAL_SLOTS];

//    public bool isFullInventoryVisible { get; private set; } = false;

//    public static event Action<int, Item, int> OnInventorySlotChanged; // slotIndex, item, quantity
//    public static event Action<bool> OnInventoryVisibilityChanged; // isFullyVisible

//    // Singleton or accessible reference
//    public static PlayerInventory Instance { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            // DontDestroyOnLoad(gameObject); // If player persists
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Initialize slots
//        for (int i = 0; i < TOTAL_SLOTS; i++)
//        {
//            slots[i] = new InventorySlot();
//        }
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            ToggleFullInventoryView();
//        }

//        // Example: Use item from slot 1 (hotkey '1')
//        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
//        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
//        // ... up to Alpha0 for slot 9 (or 10th slot)
//    }

//    public void ToggleFullInventoryView()
//    {
//        isFullInventoryVisible = !isFullInventoryVisible;
//        Debug.Log($"Inventory full view: {isFullInventoryVisible}");
//        OnInventoryVisibilityChanged?.Invoke(isFullInventoryVisible);
//    }

//    public bool AddItem(Item itemToAdd, int quantity = 1)
//    {
//        if (itemToAdd == null || quantity <= 0) return false;

//        // 1. Try to stack with existing items
//        if (itemToAdd.isStackable)
//        {
//            for (int i = 0; i < TOTAL_SLOTS; i++)
//            {
//                if (slots[i].itemData == itemToAdd && slots[i].quantity < itemToAdd.maxStackSize)
//                {
//                    int canAdd = itemToAdd.maxStackSize - slots[i].quantity;
//                    int toAdd = Mathf.Min(quantity, canAdd);
//                    slots[i].AddQuantity(toAdd);
//                    OnInventorySlotChanged?.Invoke(i, slots[i].itemData, slots[i].quantity);
//                    quantity -= toAdd;
//                    if (quantity <= 0)
//                    {
//                        Debug.Log($"Stacked {itemToAdd.itemName} in slot {i}. New quantity: {slots[i].quantity}");
//                        return true;
//                    }
//                }
//            }
//        }

//        // 2. Try to find an empty slot for remaining quantity or non-stackable item
//        for (int i = 0; i < TOTAL_SLOTS; i++)
//        {
//            if (slots[i].itemData == null)
//            {
//                if (itemToAdd.isStackable)
//                {
//                    int toAdd = Mathf.Min(quantity, itemToAdd.maxStackSize);
//                    slots[i].Set(itemToAdd, toAdd);
//                    OnInventorySlotChanged?.Invoke(i, slots[i].itemData, slots[i].quantity);
//                    quantity -= toAdd;
//                    Debug.Log($"Added {itemToAdd.itemName} (x{toAdd}) to empty slot {i}.");
//                    if (quantity <= 0) return true;
//                    // Continue if more quantity needs new slots
//                }
//                else // Non-stackable, add one and done if quantity was 1
//                {
//                    slots[i].Set(itemToAdd, 1);
//                    OnInventorySlotChanged?.Invoke(i, slots[i].itemData, slots[i].quantity);
//                    quantity--; // Should be 0 if only one non-stackable was being added
//                    Debug.Log($"Added non-stackable {itemToAdd.itemName} to empty slot {i}.");
//                    if (quantity <= 0) return true;
//                }
//            }
//        }

//        Debug.LogWarning($"Inventory full or could not add all {itemToAdd.itemName}. Remaining quantity: {quantity}");
//        return quantity <= 0; // Return true if all items were added
//    }

//    public void RemoveItem(int slotIndex, int quantity = 1)
//    {
//        if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS || slots[slotIndex].itemData == null || quantity <= 0)
//        {
//            return;
//        }

//        slots[slotIndex].quantity -= quantity;
//        if (slots[slotIndex].quantity <= 0)
//        {
//            Debug.Log($"Removed all {slots[slotIndex].itemData.itemName} from slot {slotIndex}.");
//            slots[slotIndex].Clear();
//            OnInventorySlotChanged?.Invoke(slotIndex, null, 0);
//        }
//        else
//        {
//            Debug.Log($"Removed {quantity} of {slots[slotIndex].itemData.itemName} from slot {slotIndex}. New quantity: {slots[slotIndex].quantity}");
//            OnInventorySlotChanged?.Invoke(slotIndex, slots[slotIndex].itemData, slots[slotIndex].quantity);
//        }
//    }

//    public bool HasItem(Item itemToCheck, int quantity = 1)
//    {
//        if (itemToCheck == null || quantity <= 0) return false;
//        int count = 0;
//        foreach (var slot in slots)
//        {
//            if (slot.itemData == itemToCheck)
//            {
//                count += slot.quantity;
//                if (count >= quantity) return true;
//            }
//        }
//        return false;
//    }


//    public void UseItem(int slotIndex)
//    {
//        if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS || slots[slotIndex].itemData == null)
//        {
//            Debug.LogWarning($"Cannot use item: Slot {slotIndex} is empty or invalid.");
//            return;
//        }

//        Item itemToUse = slots[slotIndex].itemData;
//        Debug.Log($"Attempting to use item: {itemToUse.itemName} from slot {slotIndex}");

//        // Specific logic for potion types
//        if (itemToUse.itemType == ItemType.Potion)
//        {
//            PlayerCombat playerCombat = GetComponent<PlayerCombat>(); // Assuming PlayerCombat is on the same GameObject
//            PlayerHealth playerHealth = GetComponent<PlayerHealth>(); // Assuming PlayerHealth is on the same GameObject

//            bool consumed = false;
//            switch (itemToUse.potionEffect)
//            {
//                case PotionEffect.DamageBoost:
//                    if (playerCombat != null)
//                    {
//                        playerCombat.ApplyDamagePotion(itemToUse);
//                        consumed = true;
//                    }
//                    break;
//                case PotionEffect.Heal:
//                    if (playerHealth != null)
//                    {
//                        playerHealth.Heal(itemToUse.effectValue);
//                        consumed = true;
//                    }
//                    break;
//                    // Add more potion effects here
//            }

//            if (consumed)
//            {
//                RemoveItem(slotIndex, 1); // Consume one item
//            }
//        }
//        else
//        {
//            // Generic item use call (could be for equipment, etc.)
//            itemToUse.Use(gameObject); // 'this.gameObject' is the player
//        }
//    }
//}