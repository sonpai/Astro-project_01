// ItemPickup.cs (Example)
using UnityEngine;
public class ItemPickup : MonoBehaviour
{
    public ItemData itemToGive; // Assign your ItemData SO in the Inspector
    public int quantity = 1;

    private void OnTriggerEnter(Collider other) // Or OnCollisionEnter, or on click
    {
        if (other.CompareTag("Player")) // Assuming player has "Player" tag
        {
            if (InventoryController.Instance != null && itemToGive != null)
            {
                bool added = InventoryController.Instance.AddItem(itemToGive, quantity);
                if (added)
                {
                    Debug.Log($"Picked up {quantity}x {itemToGive.itemName}");
                    Destroy(gameObject); // Remove the item from the world
                }
                else
                {
                    Debug.LogWarning($"Could not pick up {itemToGive.itemName}. Inventory might be full.");
                }
            }
        }
    }
}