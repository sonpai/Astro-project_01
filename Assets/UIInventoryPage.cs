// UIInventoryPage.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel; // Parent for UIInventoryItem instances
    [SerializeField] private UIInventoryDescription itemDescription; // Your existing description panel
    [SerializeField] private MouseFollower mouseFollower; // Your existing mouse follower

    private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
    private InventoryController _inventoryController; // Reference to the backend

    private int _currentlyDraggedItemIndex = -1;

    private void Awake()
    {
        // Hide initially, InventoryController will call Show()
        Hide();
        if (mouseFollower != null) mouseFollower.Toggle(false);
        if (itemDescription != null) itemDescription.ResetDescription();
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        _inventoryController = FindAnyObjectByType<InventoryController>(); // Get reference
           if (_inventoryController == null)
        {
            Debug.LogError("UIInventoryPage: InventoryController not found in scene!");
            return;
        }

        // Clear any existing UI items (e.g., if re-initializing)
        foreach (UIInventoryItem oldItem in _listOfUIItems)
        {
            if (oldItem != null) Destroy(oldItem.gameObject);
        }
        _listOfUIItems.Clear();

        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel, false); // SetParent with worldPositionStays = false
            uiItem.transform.localScale = Vector3.one; // Ensure correct scale after parenting
            _listOfUIItems.Add(uiItem);

            uiItem.SetIndex(i); // IMPORTANT: Give each UI item its slot index

            // Subscribe to UI item events
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleDrop; // Renamed for clarity
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }

        // Subscribe to InventoryController events to update UI when data changes
        _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
        _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;

        RefreshAllSlotsUI(); // Initial population
    }

    private void OnDestroy() // Unsubscribe
    {
        if (_inventoryController != null)
        {
            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
        }
        foreach (var item in _listOfUIItems)
        {
            if (item != null)
            { // Defensive check
                item.OnItemClicked -= HandleItemSelection;
                item.OnItemBeginDrag -= HandleBeginDrag;
                item.OnItemDroppedOn -= HandleDrop;
                item.OnItemEndDrag -= HandleEndDrag;
                item.OnRightMouseBtnClick -= HandleShowItemActions;
            }
        }
    }


    // Called by InventoryController when a specific slot's data changes
    private void UpdateSlotUI(int slotIndex, ItemData itemData, int quantity)
    {
        if (slotIndex < 0 || slotIndex >= _listOfUIItems.Count) return;

        if (itemData != null && quantity > 0)
        {
            _listOfUIItems[slotIndex].SetData(itemData.itemIcon, quantity);
        }
        else
        {
            _listOfUIItems[slotIndex].ResetData();
        }
    }

    // Called by InventoryController when the whole inventory might have changed (e.g., after load)
    private void RefreshAllSlotsUI()
    {
        if (_inventoryController == null) return;
        for (int i = 0; i < _inventoryController.inventorySize; i++)
        {
            if (i < _listOfUIItems.Count) // Ensure UI slot exists
            {
                ItemData data = _inventoryController.GetItemDataInSlot(i);
                int qty = _inventoryController.GetQuantityInSlot(i);
                UpdateSlotUI(i, data, qty);
            }
        }
    }


    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        if (itemDescription == null || _inventoryController == null) return;

        int clickedIndex = inventoryItemUI.SlotIndex;
        ItemData itemData = _inventoryController.GetItemDataInSlot(clickedIndex);

        if (itemData != null)
        {
            itemDescription.SetDescription(itemData.itemIcon, itemData.itemName, itemData.description);
            // Handle visual selection of the item in UI
            _listOfUIItems.ForEach(item => item.Deselect()); // Deselect all
            inventoryItemUI.Select(); // Select clicked one
        }
        else
        {
            itemDescription.ResetDescription();
            inventoryItemUI.Deselect();
        }
        Debug.Log($"Item selected at index: {clickedIndex}, Name: {itemData?.itemName ?? "Empty"}");
    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        if (mouseFollower == null || _inventoryController == null) return;

        _currentlyDraggedItemIndex = inventoryItemUI.SlotIndex;
        ItemData itemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
        int quantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);

        if (itemData != null && quantity > 0)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(itemData.itemIcon, quantity); // Show icon of dragged item
            // Optionally hide the item in the original slot while dragging
            // inventoryItemUI.ResetData(); // Or set to a "dragging" visual state
        }
        else
        {
            _currentlyDraggedItemIndex = -1; // Nothing to drag
        }
    }

    // This is called when a drag ends ON TOP of another UIInventoryItem
    private void HandleDrop(UIInventoryItem droppedOnItemUI)
    {
        if (_inventoryController == null || _currentlyDraggedItemIndex == -1) return;

        int dropTargetIndex = droppedOnItemUI.SlotIndex;
        if (_currentlyDraggedItemIndex != dropTargetIndex)
        {
            Debug.Log($"Attempting to swap item from slot {_currentlyDraggedItemIndex} to slot {dropTargetIndex}");
            _inventoryController.SwapItems(_currentlyDraggedItemIndex, dropTargetIndex);
        }
        // EndDrag will handle mouse follower toggle
    }

    // This is called when a drag ends, regardless of where it was dropped
    private void HandleEndDrag(UIInventoryItem draggedItemUI) // draggedItemUI is the item where drag started
    {
        if (mouseFollower != null) mouseFollower.Toggle(false);

        // If not dropped on another valid slot, OnItemDroppedOn wouldn't have fired.
        // Here you could implement logic for dropping outside the inventory (e.g., drop item in world),
        // or simply reset the drag operation if it wasn't a successful swap.
        // For now, if it wasn't a swap, the item "returns" visually because its data never changed.

        // If you temporarily hid the item in HandleBeginDrag, restore it here if not swapped:
        // if (_currentlyDraggedItemIndex != -1 && !_wasSwappedThisDrag) {
        //    UpdateSlotUI(_currentlyDraggedItemIndex, _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex), _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex));
        // }

        _currentlyDraggedItemIndex = -1;
    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        if (_inventoryController == null) return;
        int clickedIndex = inventoryItemUI.SlotIndex;
        ItemData itemData = _inventoryController.GetItemDataInSlot(clickedIndex);

        if (itemData != null)
        {
            Debug.Log($"Right-clicked on item: {itemData.itemName} at index {clickedIndex}. Implement item actions (e.g., Use, Drop, Equip).");
            // Example: Show a context menu or directly use the item
            // _inventoryController.UseItem(clickedIndex);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (itemDescription != null) itemDescription.ResetDescription();
        // RefreshAllSlotsUI(); // Already called by InventoryController.OnInventoryRefreshed which is good when Show is called by controller
        // If Show can be called independently, ensure UI is up-to-date:
        if (_inventoryController != null && _listOfUIItems.Count == _inventoryController.inventorySize)
        {
            RefreshAllSlotsUI();
        }
        else if (_inventoryController != null)
        {
            // This might happen if Show is called before Start or if sizes mismatch
            Debug.LogWarning("UIInventoryPage Show called but UI items might not be fully initialized or synced with controller size.");
        }

    }

    // In your UIInventoryPage.cs
    private InventoryController _inventoryControllerInstance; // Cache the instance

    private void Start() // Or OnEnable if your UI panel is activated/deactivated often
    {
        _inventoryControllerInstance = InventoryController.Instance; // Get the singleton
        if (_inventoryControllerInstance != null)
        {
            _inventoryControllerInstance.AssignAndInitializeUI(this); // Register this UI
        }
        else
        {
            Debug.LogError("UIInventoryPage: InventoryController.Instance not found on Start!");
        }

        // Your existing Awake/Start logic for hiding, mouse follower, description reset
        Hide(); // Make sure it's hidden initially as per your original Awake
        if (mouseFollower != null) mouseFollower.Toggle(false);
        if (itemDescription != null) itemDescription.ResetDescription();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (itemDescription != null) itemDescription.ResetDescription();
        if (mouseFollower != null) mouseFollower.Toggle(false); // Ensure mouse follower is hidden
        // Deselect all items visually when hiding
        _listOfUIItems.ForEach(item => item.Deselect());
    }
}