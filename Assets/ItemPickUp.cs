////// ItemPickup.cs
////using UnityEngine;

////public class ItemPickUp : MonoBehaviour
////{
////    [Header("Item Configuration")]
////    [Tooltip("The unique ID of the item this pickup represents. Must match an itemID in ItemDataManager.")]
////    public string itemIDToGive;
////    public int quantityToGive = 1;

////    [Header("Visuals & Interaction")]
////    public SpriteRenderer spriteRenderer; // Optional: To display the item in the world
////    // public Collider2D pickupCollider; // To detect player interaction

////    void Start()
////    {
////        // Optional: Initialize the visual if you have a spriteRenderer
////        if (spriteRenderer != null && ItemDataManager.Instance != null)
////        {
////            ItemData data = ItemDataManager.Instance.GetItemByID(itemIDToGive);
////            if (data != null)
////            {
////                spriteRenderer.sprite = data.itemIcon;
////            }
////            else
////            {
////                Debug.LogWarning($"ItemPickup on {gameObject.name}: ItemID '{itemIDToGive}' not found in ItemDataManager for visual setup.", this);
////            }
////        }
////    }

////    // Example: Using OnTriggerEnter2D for pickup
////    private void OnTriggerEnter2D(Collider2D other)
////    {
////        // Assuming your player GameObject has a "Player" tag
////        if (other.CompareTag("Player"))
////        {
////            TryGiveItemToPlayer();
////        }
////    }

////    // Add this new method inside your ItemPickUp.cs class

////    public void Initialize(ItemData itemData, int qty)
////    {
////        // Set the data for this pickup
////        itemIDToGive = itemData.itemID;
////        quantityToGive = qty;

////        // Update the visual sprite to match the item
////        if (spriteRenderer != null)
////        {
////            spriteRenderer.sprite = itemData.itemIcon;
////        }
////    }

////    // Or, you might have an interaction system that calls this method
////    public void TryGiveItemToPlayer()
////    {
////        if (string.IsNullOrEmpty(itemIDToGive))
////        {
////            Debug.LogError($"ItemPickup on {gameObject.name}: itemIDToGive is not set!", this);
////            return;
////        }

////        if (ItemDataManager.Instance == null)
////        {
////            Debug.LogError("ItemPickup: ItemDataManager.Instance is not available. Cannot give item.", this);
////            return;
////        }

////        ItemData itemData = ItemDataManager.Instance.GetItemByID(itemIDToGive);

////        if (itemData == null)
////        {
////            Debug.LogError($"ItemPickup on {gameObject.name}: ItemData for ID '{itemIDToGive}' not found in ItemDataManager.", this);
////            return;
////        }

////        if (InventoryController.Instance == null)
////        {
////            Debug.LogError("ItemPickup: InventoryController.Instance is not available. Cannot add item to inventory.", this);
////            return;
////        }

////        // --- THE CORE LOGIC TO ADD THE ITEM ---
////        bool itemWasAdded = InventoryController.Instance.AddItem(itemData, quantityToGive);
////        // ----------------------------------------

////        if (itemWasAdded)
////        {
////            Debug.Log($"Player picked up {quantityToGive}x {itemData.itemName}.");
////            // Optional: Destroy the pickup GameObject from the world
////            Destroy(gameObject);
////        }
////        else
////        {
////            Debug.LogWarning($"Could not add {itemData.itemName} to inventory (e.g., inventory full). Pickup remains.");
////            // Optionally, provide feedback to the player (e.g., "Inventory Full" message)
////        }
////    }
////}

//// ItemPickup.cs
//using UnityEngine;

//public class ItemPickup : MonoBehaviour
//{
//    public ItemData itemData;
//    public int quantity = 1;
//    private SpriteRenderer _spriteRenderer;

//    public void Initialize(ItemData data, int qty)
//    {
//        itemData = data;
//        quantity = qty;
//        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
//        if (_spriteRenderer != null) _spriteRenderer.sprite = data.itemIcon;
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            if (InventoryController.Instance.AddItem(itemData, quantity))
//            {
//                Destroy(gameObject);
//            }
//        }
//    }
//}

// ItemPickup.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Tooltip("The ItemData for the item on the ground.")]
    public ItemData itemData;

    [Tooltip("How many of the item to give.")]
    public int quantity = 1;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Automatically set the visual sprite to match the item data
        if (itemData != null)
        {
            spriteRenderer.sprite = itemData.itemIcon;
        }
    }

    // Called by the InventoryController when dropping items
    public void Initialize(ItemData data, int qty)
    {
        itemData = data;
        quantity = qty;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemData.itemIcon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Try to add the item to the inventory
            if (InventoryController.Instance.AddItem(itemData, quantity))
            {
                // If the item was added successfully, destroy the pickup object
                Destroy(gameObject);
            }
            // If AddItem returns false, it means the inventory is full, so the pickup stays.
        }
    }
}