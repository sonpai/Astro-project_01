// ItemPickup.cs
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [Header("Item Configuration")]
    [Tooltip("The unique ID of the item this pickup represents. Must match an itemID in ItemDataManager.")]
    public string itemIDToGive;
    public int quantityToGive = 1;

    [Header("Visuals & Interaction")]
    public SpriteRenderer spriteRenderer; // Optional: To display the item in the world
    // public Collider2D pickupCollider; // To detect player interaction

    void Start()
    {
        // Optional: Initialize the visual if you have a spriteRenderer
        if (spriteRenderer != null && ItemDataManager.Instance != null)
        {
            ItemData data = ItemDataManager.Instance.GetItemByID(itemIDToGive);
            if (data != null)
            {
                spriteRenderer.sprite = data.itemIcon;
            }
            else
            {
                Debug.LogWarning($"ItemPickup on {gameObject.name}: ItemID '{itemIDToGive}' not found in ItemDataManager for visual setup.", this);
            }
        }
    }

    // Example: Using OnTriggerEnter2D for pickup
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Assuming your player GameObject has a "Player" tag
        if (other.CompareTag("Player"))
        {
            TryGiveItemToPlayer();
        }
    }

    // Add this new method inside your ItemPickUp.cs class

    public void Initialize(ItemData itemData, int qty)
    {
        // Set the data for this pickup
        itemIDToGive = itemData.itemID;
        quantityToGive = qty;

        // Update the visual sprite to match the item
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.itemIcon;
        }
    }

    // Or, you might have an interaction system that calls this method
    public void TryGiveItemToPlayer()
    {
        if (string.IsNullOrEmpty(itemIDToGive))
        {
            Debug.LogError($"ItemPickup on {gameObject.name}: itemIDToGive is not set!", this);
            return;
        }

        if (ItemDataManager.Instance == null)
        {
            Debug.LogError("ItemPickup: ItemDataManager.Instance is not available. Cannot give item.", this);
            return;
        }

        ItemData itemData = ItemDataManager.Instance.GetItemByID(itemIDToGive);

        if (itemData == null)
        {
            Debug.LogError($"ItemPickup on {gameObject.name}: ItemData for ID '{itemIDToGive}' not found in ItemDataManager.", this);
            return;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogError("ItemPickup: InventoryController.Instance is not available. Cannot add item to inventory.", this);
            return;
        }

        // --- THE CORE LOGIC TO ADD THE ITEM ---
        bool itemWasAdded = InventoryController.Instance.AddItem(itemData, quantityToGive);
        // ----------------------------------------

        if (itemWasAdded)
        {
            Debug.Log($"Player picked up {quantityToGive}x {itemData.itemName}.");
            // Optional: Destroy the pickup GameObject from the world
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"Could not add {itemData.itemName} to inventory (e.g., inventory full). Pickup remains.");
            // Optionally, provide feedback to the player (e.g., "Inventory Full" message)
        }
    }
}