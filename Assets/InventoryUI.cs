//// InventoryUI.cs
//using UnityEngine;
//using System.Collections.Generic; // For List

//public class InventoryUI : MonoBehaviour
//{
//    [Header("UI Elements")]
//    [SerializeField] private GameObject inventoryPanel; // The main panel containing all slots
//    [SerializeField] private Transform slotsParent;     // Parent object with a GridLayoutGroup for slots
//    [SerializeField] private GameObject inventorySlotPrefab; // Prefab for a single inventory slot UI

//    private List<InventoryUISlot> uiSlots = new List<InventoryUISlot>();
//    private bool isPanelActive = false; // To track if toggled via "I" or other means

//    private void Awake()
//    {
//        // Create UI slots based on PlayerInventory.TOTAL_SLOTS
//        if (slotsParent != null && inventorySlotPrefab != null)
//        {
//            for (int i = 0; i < PlayerInventory.TOTAL_SLOTS; i++)
//            {
//                GameObject slotGO = Instantiate(inventorySlotPrefab, slotsParent);
//                InventoryUISlot uiSlot = slotGO.GetComponent<InventoryUISlot>();
//                if (uiSlot != null)
//                {
//                    uiSlot.Initialize(i); // Pass slot index
//                    uiSlots.Add(uiSlot);
//                }
//                else
//                {
//                    Debug.LogError("InventorySlotPrefab is missing InventoryUISlot component!");
//                }
//            }
//        }
//        else
//        {
//            Debug.LogError("Slots Parent or Inventory Slot Prefab not assigned in InventoryUI!");
//        }

//        // Initially hide the panel, or set based on PlayerInventory's default visibility
//        if (inventoryPanel != null)
//        {
//            isPanelActive = PlayerInventory.Instance != null ? PlayerInventory.Instance.isFullInventoryVisible : false;
//            inventoryPanel.SetActive(isPanelActive); // Start hidden or based on initial PlayerInventory state
//        }
//    }

//    private void OnEnable()
//    {
//        PlayerInventory.OnInventorySlotChanged += UpdateSlot;
//        PlayerInventory.OnInventoryVisibilityChanged += HandleInventoryVisibilityChange;
//    }

//    private void OnDisable()
//    {
//        PlayerInventory.OnInventorySlotChanged -= UpdateSlot;
//        PlayerInventory.OnInventoryVisibilityChanged -= HandleInventoryVisibilityChange;
//    }

//    private void Start()
//    {
//        // Initial full refresh
//        if (PlayerInventory.Instance != null)
//        {
//            for (int i = 0; i < PlayerInventory.TOTAL_SLOTS; ++i)
//            {
//                UpdateSlot(i, PlayerInventory.Instance.slots[i].itemData, PlayerInventory.Instance.slots[i].quantity);
//            }
//            HandleInventoryVisibilityChange(PlayerInventory.Instance.isFullInventoryVisible);
//        }
//    }


//    private void HandleInventoryVisibilityChange(bool isFullyVisible)
//    {
//        // This method controls which slots are *interactable* or visually distinct if needed.
//        // The actual showing/hiding of the *entire panel* can be handled by a separate toggle function
//        // or tied to this event if `isFullyVisible` means the whole panel becomes visible.

//        // For this implementation, we'll assume the panel's visibility is toggled by "I" key.
//        // This event will ensure the correct number of slots appear active/styled.
//        for (int i = 0; i < uiSlots.Count; i++)
//        {
//            if (i < PlayerInventory.VISIBLE_SLOTS_DEFAULT || isFullyVisible)
//            {
//                uiSlots[i].gameObject.SetActive(true); // Make sure slot UI element is active
//            }
//            else
//            {
//                uiSlots[i].gameObject.SetActive(false); // Hide slots beyond the first row if not fully visible
//            }
//        }

//        // Toggle the main inventory panel's visibility if needed
//        // if (inventoryPanel != null) inventoryPanel.SetActive(isFullyVisible);
//        // The PlayerInventory script handles the 'isFullInventoryVisible' state,
//        // so this UI should listen or have a separate toggle method called by player input.
//    }

//    // Call this method when player presses "I" or an inventory button.
//    public void ToggleInventoryPanel()
//    {
//        isPanelActive = !isPanelActive;
//        if (inventoryPanel != null) inventoryPanel.SetActive(isPanelActive);

//        if (isPanelActive)
//        { // If opening, refresh visibility based on PlayerInventory's state
//            if (PlayerInventory.Instance != null) HandleInventoryVisibilityChange(PlayerInventory.Instance.isFullInventoryVisible);
//        }
//    }


//    private void UpdateSlot(int slotIndex, Item item, int quantity)
//    {
//        if (slotIndex >= 0 && slotIndex < uiSlots.Count)
//        {
//            uiSlots[slotIndex].UpdateSlotDisplay(item, quantity);
//        }
//    }
//}