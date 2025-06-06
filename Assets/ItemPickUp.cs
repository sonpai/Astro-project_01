// ItemPickup.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Ensures there is a collider
public class ItemPickup : MonoBehaviour
{
    public ItemData itemDataToGive;
    public int quantity = 1;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool _canBePickedUp = true;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    // Called by InventoryController when dropping an item
    public void Initialize(ItemData itemData, int qty)
    {
        itemDataToGive = itemData;
        quantity = qty;
        if (spriteRenderer != null && itemData != null)
        {
            spriteRenderer.sprite = itemData.itemIcon;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canBePickedUp) return;

        if (other.CompareTag("Player")) // Make sure your player GameObject has the "Player" tag
        {
            if (InventoryController.Instance != null && itemDataToGive != null)
            {
                bool addedSuccessfully = InventoryController.Instance.AddItem(itemDataToGive, quantity);
                if (addedSuccessfully)
                {
                    Destroy(gameObject); // Item picked up, remove from world
                }
            }
        }
    }
}