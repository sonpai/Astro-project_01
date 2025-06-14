
//// ItemPickup.cs
//using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
//public class ItemPickup : MonoBehaviour
//{
//    [Tooltip("The ItemData for the item on the ground.")]
//    public ItemData itemData;

//    [Tooltip("How many of the item to give.")]
//    public int quantity = 1;

//    private SpriteRenderer spriteRenderer;

//    private void Awake()
//    {
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        // Automatically set the visual sprite to match the item data
//        if (itemData != null)
//        {
//            spriteRenderer.sprite = itemData.itemIcon;
//        }
//    }

//    // Called by the InventoryController when dropping items
//    public void Initialize(ItemData data, int qty)
//    {
//        itemData = data;
//        quantity = qty;
//        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
//        spriteRenderer.sprite = itemData.itemIcon;
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        // Check if the object that entered the trigger has the "Player" tag
//        if (other.CompareTag("Player"))
//        {
//            // Try to add the item to the inventory
//            if (InventoryController.Instance.AddItem(itemData, quantity))
//            {
//                // If the item was added successfully, destroy the pickup object
//                Destroy(gameObject);
//            }
//            // If AddItem returns false, it means the inventory is full, so the pickup stays.
//        }
//    }
//}

// PASTE THIS ENTIRE CODE INTO YOUR 'ItemPickUp.cs' FILE

using UnityEngine;
using System.Collections; // Required for using coroutines

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Tooltip("The ItemData for the item on the ground.")]
    public ItemData itemData;

    [Tooltip("How many of the item to give.")]
    public int quantity = 1;

    private SpriteRenderer spriteRenderer;
    private Collider2D col; // Reference to the item's collider

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get the collider component to be disabled/enabled
        col = GetComponent<Collider2D>();
    }

    // This is now called by the InventoryController when an item is dropped
    public void Initialize(ItemData data, int qty)
    {
        itemData = data;
        quantity = qty;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemData.itemIcon;

        // Start the delay coroutine as soon as the item is created on the ground
        StartCoroutine(EnableColliderAfterDelay(0.5f));
    }

    // This coroutine temporarily disables the collider to prevent instant pickup
    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        // Disable the collider immediately
        col.enabled = false;
        // Wait for the specified delay time (e.g., 0.5 seconds)
        yield return new WaitForSeconds(delay);
        // Re-enable the collider, allowing the item to be picked up now
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventoryController.Instance.AddItem(itemData, quantity))
            {
                Destroy(gameObject);
            }
        }
    }
}