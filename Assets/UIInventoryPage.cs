//// UIInventoryPage.cs
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class UIInventoryPage : MonoBehaviour
//{
//    [SerializeField] private UIInventoryItem itemPrefab;
//    [SerializeField] private RectTransform contentPanel; // Parent for UIInventoryItem instances
//    [SerializeField] private UIInventoryDescription itemDescription; // Your existing description panel
//    [SerializeField] private MouseFollower mouseFollower; // Your existing mouse follower

//    private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
//    private InventoryController _inventoryController; // Reference to the backend

//    private int _currentlyDraggedItemIndex = -1;

//    private void Awake()
//    {
//        // Hide initially, InventoryController will call Show()
//        Hide();
//        if (mouseFollower != null) mouseFollower.Toggle(false);
//        if (itemDescription != null) itemDescription.ResetDescription();
//    }

//    public void InitializeInventoryUI(int inventorySize)
//    {
//        _inventoryController = FindAnyObjectByType<InventoryController>(); // Get reference
//           if (_inventoryController == null)
//        {
//            Debug.LogError("UIInventoryPage: InventoryController not found in scene!");
//            return;
//        }

//        // Clear any existing UI items (e.g., if re-initializing)
//        foreach (UIInventoryItem oldItem in _listOfUIItems)
//        {
//            if (oldItem != null) Destroy(oldItem.gameObject);
//        }
//        _listOfUIItems.Clear();

//        for (int i = 0; i < inventorySize; i++)
//        {
//            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
//            uiItem.transform.SetParent(contentPanel, false); // SetParent with worldPositionStays = false
//            uiItem.transform.localScale = Vector3.one; // Ensure correct scale after parenting
//            _listOfUIItems.Add(uiItem);

//            uiItem.SetIndex(i); // IMPORTANT: Give each UI item its slot index

//            // Subscribe to UI item events
//            uiItem.OnItemClicked += HandleItemSelection;
//            uiItem.OnItemBeginDrag += HandleBeginDrag;
//            uiItem.OnItemDroppedOn += HandleDrop; // Renamed for clarity
//            uiItem.OnItemEndDrag += HandleEndDrag;
//            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//        }

//        // Subscribe to InventoryController events to update UI when data changes
//        _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
//        _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;

//        RefreshAllSlotsUI(); // Initial population
//    }

//    private void OnDestroy() // Unsubscribe
//    {
//        if (_inventoryController != null)
//        {
//            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
//            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
//        }
//        foreach (var item in _listOfUIItems)
//        {
//            if (item != null)
//            { // Defensive check
//                item.OnItemClicked -= HandleItemSelection;
//                item.OnItemBeginDrag -= HandleBeginDrag;
//                item.OnItemDroppedOn -= HandleDrop;
//                item.OnItemEndDrag -= HandleEndDrag;
//                item.OnRightMouseBtnClick -= HandleShowItemActions;
//            }
//        }
//    }


//    // Called by InventoryController when a specific slot's data changes
//    private void UpdateSlotUI(int slotIndex, ItemData itemData, int quantity)
//    {
//        if (slotIndex < 0 || slotIndex >= _listOfUIItems.Count) return;

//        if (itemData != null && quantity > 0)
//        {
//            _listOfUIItems[slotIndex].SetData(itemData.itemIcon, quantity);
//        }
//        else
//        {
//            _listOfUIItems[slotIndex].ResetData();
//        }
//    }

//    // Called by InventoryController when the whole inventory might have changed (e.g., after load)
//    private void RefreshAllSlotsUI()
//    {
//        if (_inventoryController == null) return;
//        for (int i = 0; i < _inventoryController.inventorySize; i++)
//        {
//            if (i < _listOfUIItems.Count) // Ensure UI slot exists
//            {
//                ItemData data = _inventoryController.GetItemDataInSlot(i);
//                int qty = _inventoryController.GetQuantityInSlot(i);
//                UpdateSlotUI(i, data, qty);
//            }
//        }
//    }


//    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
//    {
//        if (itemDescription == null || _inventoryController == null) return;

//        int clickedIndex = inventoryItemUI.SlotIndex;
//        ItemData itemData = _inventoryController.GetItemDataInSlot(clickedIndex);

//        if (itemData != null)
//        {
//            itemDescription.SetDescription(itemData.itemIcon, itemData.itemName, itemData.description);
//            // Handle visual selection of the item in UI
//            _listOfUIItems.ForEach(item => item.Deselect()); // Deselect all
//            inventoryItemUI.Select(); // Select clicked one
//        }
//        else
//        {
//            itemDescription.ResetDescription();
//            inventoryItemUI.Deselect();
//        }
//        Debug.Log($"Item selected at index: {clickedIndex}, Name: {itemData?.itemName ?? "Empty"}");
//    }

//    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
//    {
//        if (mouseFollower == null || _inventoryController == null) return;

//        _currentlyDraggedItemIndex = inventoryItemUI.SlotIndex;
//        ItemData itemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
//        int quantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);

//        if (itemData != null && quantity > 0)
//        {
//            mouseFollower.Toggle(true);
//            mouseFollower.SetData(itemData.itemIcon, quantity); // Show icon of dragged item
//            // Optionally hide the item in the original slot while dragging
//            // inventoryItemUI.ResetData(); // Or set to a "dragging" visual state
//        }
//        else
//        {
//            _currentlyDraggedItemIndex = -1; // Nothing to drag
//        }
//    }

//    // This is called when a drag ends ON TOP of another UIInventoryItem
//    private void HandleDrop(UIInventoryItem droppedOnItemUI)
//    {
//        if (_inventoryController == null || _currentlyDraggedItemIndex == -1) return;

//        int dropTargetIndex = droppedOnItemUI.SlotIndex;
//        if (_currentlyDraggedItemIndex != dropTargetIndex)
//        {
//            Debug.Log($"Attempting to swap item from slot {_currentlyDraggedItemIndex} to slot {dropTargetIndex}");
//            _inventoryController.SwapItems(_currentlyDraggedItemIndex, dropTargetIndex);
//        }
//        // EndDrag will handle mouse follower toggle
//    }

//    // This is called when a drag ends, regardless of where it was dropped
//    private void HandleEndDrag(UIInventoryItem draggedItemUI) // draggedItemUI is the item where drag started
//    {
//        if (mouseFollower != null) mouseFollower.Toggle(false);

//        // If not dropped on another valid slot, OnItemDroppedOn wouldn't have fired.
//        // Here you could implement logic for dropping outside the inventory (e.g., drop item in world),
//        // or simply reset the drag operation if it wasn't a successful swap.
//        // For now, if it wasn't a swap, the item "returns" visually because its data never changed.

//        // If you temporarily hid the item in HandleBeginDrag, restore it here if not swapped:
//        // if (_currentlyDraggedItemIndex != -1 && !_wasSwappedThisDrag) {
//        //    UpdateSlotUI(_currentlyDraggedItemIndex, _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex), _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex));
//        // }

//        _currentlyDraggedItemIndex = -1;
//    }

//    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
//    {
//        if (_inventoryController == null) return;
//        int clickedIndex = inventoryItemUI.SlotIndex;
//        ItemData itemData = _inventoryController.GetItemDataInSlot(clickedIndex);

//        if (itemData != null)
//        {
//            Debug.Log($"Right-clicked on item: {itemData.itemName} at index {clickedIndex}. Implement item actions (e.g., Use, Drop, Equip).");
//            // Example: Show a context menu or directly use the item
//            // _inventoryController.UseItem(clickedIndex);
//        }
//    }

//    public void Show()
//    {
//        gameObject.SetActive(true);
//        if (itemDescription != null) itemDescription.ResetDescription();
//        // RefreshAllSlotsUI(); // Already called by InventoryController.OnInventoryRefreshed which is good when Show is called by controller
//        // If Show can be called independently, ensure UI is up-to-date:
//        if (_inventoryController != null && _listOfUIItems.Count == _inventoryController.inventorySize)
//        {
//            RefreshAllSlotsUI();
//        }
//        else if (_inventoryController != null)
//        {
//            // This might happen if Show is called before Start or if sizes mismatch
//            Debug.LogWarning("UIInventoryPage Show called but UI items might not be fully initialized or synced with controller size.");
//        }

//    }

//    // In your UIInventoryPage.cs
//    private InventoryController _inventoryControllerInstance; // Cache the instance

//    private void Start() // Or OnEnable if your UI panel is activated/deactivated often
//    {
//        _inventoryControllerInstance = InventoryController.Instance; // Get the singleton
//        if (_inventoryControllerInstance != null)
//        {
//            _inventoryControllerInstance.AssignAndInitializeUI(this); // Register this UI
//        }
//        else
//        {
//            Debug.LogError("UIInventoryPage: InventoryController.Instance not found on Start!");
//        }

//        // Your existing Awake/Start logic for hiding, mouse follower, description reset
//        Hide(); // Make sure it's hidden initially as per your original Awake
//        if (mouseFollower != null) mouseFollower.Toggle(false);
//        if (itemDescription != null) itemDescription.ResetDescription();
//    }

//    public void Hide()
//    {
//        gameObject.SetActive(false);
//        if (itemDescription != null) itemDescription.ResetDescription();
//        if (mouseFollower != null) mouseFollower.Toggle(false); // Ensure mouse follower is hidden
//        // Deselect all items visually when hiding
//        _listOfUIItems.ForEach(item => item.Deselect());
//    }
//}

// UIInventoryPage.cs (ensure it uses the correct InventoryController instance)
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private UIInventoryDescription itemDescription;
    [SerializeField] private MouseFollower mouseFollower; // Assign your MouseFollower GameObject here

    private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
    private InventoryController _inventoryController; // Reference to the backend (your new InventoryController)

    private int _currentlyDraggedItemIndex = -1;

    private void Awake()
    {
        Hide();
        if (mouseFollower != null) mouseFollower.Toggle(false);
        if (itemDescription != null) itemDescription.ResetDescription();
    }

    private void Start()
    {
        _inventoryController = InventoryController.Instance; // Get the singleton instance
        if (_inventoryController != null)
        {
            // The new InventoryController has AssignAndInitializeUI
            _inventoryController.AssignAndInitializeUI(this);
        }
        else
        {
            Debug.LogError("UIInventoryPage: InventoryController.Instance not found on Start! UI will not function correctly.");
        }
    }

    // Called by InventoryController.AssignAndInitializeUI
    public void InitializeInventoryUI(int inventorySize)
    {
        if (_inventoryController == null)
        {
            // This might happen if AssignAndInitializeUI is called before _inventoryController is set in Start,
            // or if InventoryController.Instance was null.
            _inventoryController = InventoryController.Instance; // Try to get it again
            if (_inventoryController == null)
            {
                Debug.LogError("UIInventoryPage: InventoryController is still null in InitializeInventoryUI. Cannot initialize.");
                return;
            }
        }

        // Clear any existing UI items
        foreach (Transform child in contentPanel) { Destroy(child.gameObject); }
        _listOfUIItems.Clear();

        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
            uiItem.transform.localScale = Vector3.one;
            _listOfUIItems.Add(uiItem);

            uiItem.SetIndex(i);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleDrop;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions; // Or HandleUseItem if you prefer
        }

        // Subscribe to InventoryController events (your new InventoryController has these)
        _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
        _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;

        // Initial population should be triggered by InventoryController after this via OnInventoryRefreshed
        // _inventoryController.OnInventoryRefreshed?.Invoke(); // Controller does this.
    }

    private void OnDestroy()
    {
        if (_inventoryController != null)
        {
            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
        }
        // Unsubscribe from UIInventoryItem events if necessary, though if page is destroyed, items usually go with it.
        foreach (var item in _listOfUIItems)
        {
            if (item != null) // Defensive check
            {
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
            _listOfUIItems[slotIndex].Deselect(); // Also deselect if it becomes empty
        }
    }

    // Called by InventoryController when the whole inventory might have changed
    private void RefreshAllSlotsUI()
    {
        if (_inventoryController == null)
        {
            Debug.LogWarning("UIInventoryPage: Cannot RefreshAllSlotsUI, _inventoryController is null.");
            return;
        }
        // Use _inventoryController.inventorySize as the source of truth for slot count
        for (int i = 0; i < _inventoryController.inventorySize; i++)
        {
            if (i < _listOfUIItems.Count) // Ensure UI slot visual element exists
            {
                ItemData data = _inventoryController.GetItemDataInSlot(i);
                int qty = _inventoryController.GetQuantityInSlot(i);
                UpdateSlotUI(i, data, qty);
            }
            else
            {
                Debug.LogWarning($"UIInventoryPage: Mismatch during RefreshAllSlotsUI. Controller wants to update slot {i}, but only {_listOfUIItems.Count} UI items exist. Call InitializeInventoryUI first.");
                break; // Stop if UI items are not fully initialized
            }
        }
    }

    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        if (itemDescription == null || _inventoryController == null) return;

        int clickedIndex = inventoryItemUI.SlotIndex;
        // Use _inventoryController to get item data
        ItemData itemData = _inventoryController.GetItemDataInSlot(clickedIndex);

        _listOfUIItems.ForEach(item => item.Deselect()); // Deselect all first

        if (itemData != null)
        {
            itemDescription.SetDescription(itemData.itemIcon, itemData.itemName, itemData.description);
            inventoryItemUI.Select();
        }
        else
        {
            itemDescription.ResetDescription();
        }
        // Debug.Log($"Item selected at index: {clickedIndex}, Name: {itemData?.itemName ?? "Empty"}");
    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        if (mouseFollower == null || _inventoryController == null) return;

        _currentlyDraggedItemIndex = inventoryItemUI.SlotIndex;
        // Use _inventoryController to get item data and quantity
        ItemData itemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
        int quantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);

        if (itemData != null && quantity > 0)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(itemData.itemIcon, quantity);
            inventoryItemUI.ResetData(); // Visually clear the source slot while dragging
        }
        else
        {
            _currentlyDraggedItemIndex = -1; // Nothing valid to drag
        }
    }

    private void HandleDrop(UIInventoryItem droppedOnItemUI) // Called on the TARGET item
    {
        if (_inventoryController == null || _currentlyDraggedItemIndex == -1) return;

        int dropTargetIndex = droppedOnItemUI.SlotIndex;
        if (_currentlyDraggedItemIndex != dropTargetIndex)
        {
            // Use _inventoryController to swap items
            _inventoryController.SwapItems(_currentlyDraggedItemIndex, dropTargetIndex);
        }
        // EndDrag will handle mouse follower toggle and resetting _currentlyDraggedItemIndex.
        // The InventoryController.SwapItems should trigger OnInventorySlotUpdated, which will update the UI.
    }

    private void HandleEndDrag(UIInventoryItem draggedItemUI) // Called on the SOURCE item
    {
        if (mouseFollower != null) mouseFollower.Toggle(false);

        // If the item was visually cleared in HandleBeginDrag and not successfully swapped,
        // the OnInventorySlotUpdated (or OnInventoryRefreshed) from InventoryController
        // needs to restore the visual of the original slot if the drop was invalid (e.g., outside).
        // Your InventoryController.SwapItems calls Save and triggers updates for the two slots.
        // If an item is dragged and dropped outside, SwapItems isn't called.
        // The slot that was ResetData() in BeginDrag needs to be refreshed.
        // A simple way: if _currentlyDraggedItemIndex is valid, refresh that specific slot.
        if (_currentlyDraggedItemIndex != -1)
        {
            // This implies the drag started but might not have resulted in a swap.
            // Refresh the original slot to ensure its visual state is correct.
            // The InventoryController events *should* handle this if state changed,
            // but if dragged off-window, no state change occurs, so visual must be reset.
            ItemData originalItemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
            int originalQuantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);
            if (originalItemData != null && _currentlyDraggedItemIndex < _listOfUIItems.Count)
            {
                _listOfUIItems[_currentlyDraggedItemIndex].SetData(originalItemData.itemIcon, originalQuantity);
            }
            else if (_currentlyDraggedItemIndex < _listOfUIItems.Count)
            {
                _listOfUIItems[_currentlyDraggedItemIndex].ResetData(); // if it became empty
            }
        }
        _currentlyDraggedItemIndex = -1; // Reset drag operation
    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        if (_inventoryController == null) return;
        int clickedIndex = inventoryItemUI.SlotIndex;
        // Use _inventoryController to handle item usage
        _inventoryController.UseItem(clickedIndex);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (itemDescription != null) itemDescription.ResetDescription();

        // REMOVE OR COMMENT OUT THE FOLLOWING LINE:
        // if(_inventoryController != null) _inventoryController.OnInventoryRefreshed?.Invoke(); 
        // Your InventoryController.Update() method already calls OnInventoryRefreshed before showing.
        // If you absolutely need UIInventoryPage to request a refresh at other times,
        // add a public method to InventoryController like public void RequestRefresh() { OnInventoryRefreshed?.Invoke(); }
        // and call that instead. But for this Show() case, it's handled by the controller.
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (itemDescription != null) itemDescription.ResetDescription();
        if (mouseFollower != null) mouseFollower.Toggle(false);
        _listOfUIItems.ForEach(item => item.Deselect());
        if (_currentlyDraggedItemIndex != -1 && _inventoryController != null && _currentlyDraggedItemIndex < _listOfUIItems.Count)
        {
            // If hiding mid-drag, restore the appearance of the dragged item's original slot
            ItemData originalItemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
            int originalQuantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);
            if (originalItemData != null)
            {
                _listOfUIItems[_currentlyDraggedItemIndex].SetData(originalItemData.itemIcon, originalQuantity);
            }
            else
            {
                _listOfUIItems[_currentlyDraggedItemIndex].ResetData();
            }
        }
        _currentlyDraggedItemIndex = -1;
    }
}