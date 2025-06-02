
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class UIInventoryPage : MonoBehaviour
//{
//    [SerializeField] private UIInventoryItem itemPrefab;
//    [SerializeField] private RectTransform contentPanel;
//    [SerializeField] private UIInventoryDescription itemDescription;
//    [SerializeField] private MouseFollower mouseFollower; // Assign your MouseFollower GameObject here

//    private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
//    private InventoryController _inventoryController; // Reference to the backend (your new InventoryController)

//    private int _currentlyDraggedItemIndex = -1;

//    private void Awake()
//    {
//        Hide();
//        if (mouseFollower != null) mouseFollower.Toggle(false);
//        if (itemDescription != null) itemDescription.ResetDescription();
//    }

//    private void Start()
//    {
//        _inventoryController = InventoryController.Instance; // Get the singleton instance
//        if (_inventoryController != null)
//        {
//            // The new InventoryController has AssignAndInitializeUI
//            _inventoryController.AssignAndInitializeUI(this);
//        }
//        else
//        {
//            Debug.LogError("UIInventoryPage: InventoryController.Instance not found on Start! UI will not function correctly.");
//        }
//    }

//    // Called by InventoryController.AssignAndInitializeUI
//    public void InitializeInventoryUI(int inventorySize)
//    {
//        if (_inventoryController == null)
//        {
//            // This might happen if AssignAndInitializeUI is called before _inventoryController is set in Start,
//            // or if InventoryController.Instance was null.
//            _inventoryController = InventoryController.Instance; // Try to get it again
//            if (_inventoryController == null)
//            {
//                Debug.LogError("UIInventoryPage: InventoryController is still null in InitializeInventoryUI. Cannot initialize.");
//                return;
//            }
//        }

//        // Clear any existing UI items
//        foreach (Transform child in contentPanel) { Destroy(child.gameObject); }
//        _listOfUIItems.Clear();

//        for (int i = 0; i < inventorySize; i++)
//        {
//            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
//            uiItem.transform.localScale = Vector3.one;
//            _listOfUIItems.Add(uiItem);

//            uiItem.SetIndex(i);

//            uiItem.OnItemClicked += HandleItemSelection;
//            uiItem.OnItemBeginDrag += HandleBeginDrag;
//            uiItem.OnItemDroppedOn += HandleDrop;
//            uiItem.OnItemEndDrag += HandleEndDrag;
//            uiItem.OnRightMouseBtnClick += HandleShowItemActions; // Or HandleUseItem if you prefer
//        }

//        // Subscribe to InventoryController events (your new InventoryController has these)
//        _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
//        _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;

//        // Initial population should be triggered by InventoryController after this via OnInventoryRefreshed
//        // _inventoryController.OnInventoryRefreshed?.Invoke(); // Controller does this.
//    }

//    private void OnDestroy()
//    {
//        if (_inventoryController != null)
//        {
//            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
//            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
//        }
//        // Unsubscribe from UIInventoryItem events if necessary, though if page is destroyed, items usually go with it.
//        foreach (var item in _listOfUIItems)
//        {
//            if (item != null) // Defensive check
//            {
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
//            _listOfUIItems[slotIndex].Deselect(); // Also deselect if it becomes empty
//        }
//    }

//    // Called by InventoryController when the whole inventory might have changed
//    private void RefreshAllSlotsUI()
//    {
//        if (_inventoryController == null)
//        {
//            Debug.LogWarning("UIInventoryPage: Cannot RefreshAllSlotsUI, _inventoryController is null.");
//            return;
//        }
//        // Use _inventoryController.inventorySize as the source of truth for slot count
//        for (int i = 0; i < _inventoryController.inventorySize; i++)
//        {
//            if (i < _listOfUIItems.Count) // Ensure UI slot visual element exists
//            {
//                ItemData data = _inventoryController.GetItemDataInSlot(i);
//                int qty = _inventoryController.GetQuantityInSlot(i);
//                UpdateSlotUI(i, data, qty);
//            }
//            else
//            {
//                Debug.LogWarning($"UIInventoryPage: Mismatch during RefreshAllSlotsUI. Controller wants to update slot {i}, but only {_listOfUIItems.Count} UI items exist. Call InitializeInventoryUI first.");
//                break; // Stop if UI items are not fully initialized
//            }
//        }
//    }

//    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
//    {
//        if (itemDescription == null || _inventoryController == null) return;

//        int clickedIndex = inventoryItemUI.SlotIndex;
//        // Use _inventoryController to get item data
//        ItemData itemData = _inventoryController.GetItemDataInSlot(clickedIndex);

//        _listOfUIItems.ForEach(item => item.Deselect()); // Deselect all first

//        if (itemData != null)
//        {
//            itemDescription.SetDescription(itemData.itemIcon, itemData.itemName, itemData.description);
//            inventoryItemUI.Select();
//        }
//        else
//        {
//            itemDescription.ResetDescription();
//        }
//        // Debug.Log($"Item selected at index: {clickedIndex}, Name: {itemData?.itemName ?? "Empty"}");
//    }

//    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
//    {
//        if (mouseFollower == null || _inventoryController == null) return;

//        _currentlyDraggedItemIndex = inventoryItemUI.SlotIndex;
//        // Use _inventoryController to get item data and quantity
//        ItemData itemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
//        int quantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);

//        if (itemData != null && quantity > 0)
//        {
//            mouseFollower.Toggle(true);
//            mouseFollower.SetData(itemData.itemIcon, quantity);
//            inventoryItemUI.ResetData(); // Visually clear the source slot while dragging
//        }
//        else
//        {
//            _currentlyDraggedItemIndex = -1; // Nothing valid to drag
//        }
//    }

//    private void HandleDrop(UIInventoryItem droppedOnItemUI) // Called on the TARGET item
//    {
//        if (_inventoryController == null || _currentlyDraggedItemIndex == -1) return;

//        int dropTargetIndex = droppedOnItemUI.SlotIndex;
//        if (_currentlyDraggedItemIndex != dropTargetIndex)
//        {
//            // Use _inventoryController to swap items
//            _inventoryController.SwapItems(_currentlyDraggedItemIndex, dropTargetIndex);
//        }
//        // EndDrag will handle mouse follower toggle and resetting _currentlyDraggedItemIndex.
//        // The InventoryController.SwapItems should trigger OnInventorySlotUpdated, which will update the UI.
//    }

//    private void HandleEndDrag(UIInventoryItem draggedItemUI) // Called on the SOURCE item
//    {
//        if (mouseFollower != null) mouseFollower.Toggle(false);

//        // If the item was visually cleared in HandleBeginDrag and not successfully swapped,
//        // the OnInventorySlotUpdated (or OnInventoryRefreshed) from InventoryController
//        // needs to restore the visual of the original slot if the drop was invalid (e.g., outside).
//        // Your InventoryController.SwapItems calls Save and triggers updates for the two slots.
//        // If an item is dragged and dropped outside, SwapItems isn't called.
//        // The slot that was ResetData() in BeginDrag needs to be refreshed.
//        // A simple way: if _currentlyDraggedItemIndex is valid, refresh that specific slot.
//        if (_currentlyDraggedItemIndex != -1)
//        {
//            // This implies the drag started but might not have resulted in a swap.
//            // Refresh the original slot to ensure its visual state is correct.
//            // The InventoryController events *should* handle this if state changed,
//            // but if dragged off-window, no state change occurs, so visual must be reset.
//            ItemData originalItemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
//            int originalQuantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);
//            if (originalItemData != null && _currentlyDraggedItemIndex < _listOfUIItems.Count)
//            {
//                _listOfUIItems[_currentlyDraggedItemIndex].SetData(originalItemData.itemIcon, originalQuantity);
//            }
//            else if (_currentlyDraggedItemIndex < _listOfUIItems.Count)
//            {
//                _listOfUIItems[_currentlyDraggedItemIndex].ResetData(); // if it became empty
//            }
//        }
//        _currentlyDraggedItemIndex = -1; // Reset drag operation
//    }

//    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
//    {
//        if (_inventoryController == null) return;
//        int clickedIndex = inventoryItemUI.SlotIndex;
//        // Use _inventoryController to handle item usage
//        _inventoryController.UseItem(clickedIndex);
//    }

//    public void Show()
//    {
//        gameObject.SetActive(true);
//        if (itemDescription != null) itemDescription.ResetDescription();

//        // REMOVE OR COMMENT OUT THE FOLLOWING LINE:
//        // if(_inventoryController != null) _inventoryController.OnInventoryRefreshed?.Invoke(); 
//        // Your InventoryController.Update() method already calls OnInventoryRefreshed before showing.
//        // If you absolutely need UIInventoryPage to request a refresh at other times,
//        // add a public method to InventoryController like public void RequestRefresh() { OnInventoryRefreshed?.Invoke(); }
//        // and call that instead. But for this Show() case, it's handled by the controller.
//    }

//    public void Hide()
//    {
//        gameObject.SetActive(false);
//        if (itemDescription != null) itemDescription.ResetDescription();
//        if (mouseFollower != null) mouseFollower.Toggle(false);
//        _listOfUIItems.ForEach(item => item.Deselect());
//        if (_currentlyDraggedItemIndex != -1 && _inventoryController != null && _currentlyDraggedItemIndex < _listOfUIItems.Count)
//        {
//            // If hiding mid-drag, restore the appearance of the dragged item's original slot
//            ItemData originalItemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
//            int originalQuantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);
//            if (originalItemData != null)
//            {
//                _listOfUIItems[_currentlyDraggedItemIndex].SetData(originalItemData.itemIcon, originalQuantity);
//            }
//            else
//            {
//                _listOfUIItems[_currentlyDraggedItemIndex].ResetData();
//            }
//        }
//        _currentlyDraggedItemIndex = -1;
//    }
//}

// UIInventoryPage.cs (Modified to work with the unified InventoryController)
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab; // Assign your UIInventoryItem prefab
    [SerializeField] private RectTransform contentPanel; // Assign the parent for item slots (e.g., with a GridLayoutGroup)
    [SerializeField] private UIInventoryDescription itemDescription; // Assign your description panel
    [SerializeField] private MouseFollower mouseFollower; // Assign your MouseFollower GameObject/script

    private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
    private InventoryController _inventoryController; // Will hold InventoryController.Instance

    private int _currentlyDraggedItemIndex = -1;

    private void Awake()
    {
        // Initial state setup
        Hide(); // Start hidden
        if (mouseFollower != null) mouseFollower.Toggle(false);
        if (itemDescription != null) itemDescription.ResetDescription();
    }

    private void Start()
    {
        _inventoryController = InventoryController.Instance; // Get the singleton instance
        if (_inventoryController != null)
        {
            _inventoryController.AssignAndInitializeUI(this); // Register this UI
        }
        else
        {
            Debug.LogError($"UIInventoryPage ({gameObject.name}): InventoryController.Instance not found on Start! UI will not function correctly.");
        }
    }

    // This method is now primarily called by InventoryController.AssignAndInitializeUI
    public void InitializeInventoryUI(int inventorySize)
    {
        if (_inventoryController == null) _inventoryController = InventoryController.Instance; // Failsafe
        if (_inventoryController == null)
        {
            Debug.LogError("UIInventoryPage: InventoryController is null. Cannot initialize UI.");
            return;
        }

        // Clear any existing UI items before recreating
        foreach (Transform child in contentPanel) { Destroy(child.gameObject); }
        _listOfUIItems.Clear();

        for (int i = 0; i < inventorySize; i++)
        {
            if (itemPrefab == null) { Debug.LogError("UIInventoryPage: itemPrefab is not assigned!"); return; }
            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
            uiItem.transform.localScale = Vector3.one; // Ensure correct scale after parenting under layout group
            _listOfUIItems.Add(uiItem);
            uiItem.SetIndex(i);

            // Subscribe to events from this specific UI item
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleDrop; // Make sure UIInventoryItem implements IDropHandler and invokes this
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }

        // Subscribe to broader InventoryController events for data changes
        _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI; // Unsubscribe first to prevent duplicates
        _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
        _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI; // Unsubscribe first
        _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;

        // InventoryController will call OnInventoryRefreshed after AssignAndInitializeUI,
        // which will trigger RefreshAllSlotsUI() to populate data.
    }

    private void OnDestroy()
    {
        if (_inventoryController != null)
        {
            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
        }
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
            //if (itemDescription != null && _listOfUIItems[slotIndex].isActiveAndEnabled && _listOfUIItems[slotIndex].CompareTag("Selected"))
            //{ // Example check if it was selected
            //    itemDescription.ResetDescription(); // If the cleared item was selected, reset description
            //}
            //_listOfUIItems[slotIndex].Deselect();
        }
    }

    private void RefreshAllSlotsUI()
    {
        if (_inventoryController == null) { Debug.LogWarning("UIIP: RefreshAllSlotsUI - No InventoryController."); return; }
        if (_listOfUIItems.Count != _inventoryController.inventorySize)
        {
            Debug.LogWarning($"UIIP: Mismatch in UI items ({_listOfUIItems.Count}) and controller size ({_inventoryController.inventorySize}). Re-initializing UI visuals.");
            InitializeInventoryUI(_inventoryController.inventorySize); // Rebuild UI if counts don't match
                                                                       // This call to InitializeInventoryUI will re-subscribe events, so careful about call order.
        }

        for (int i = 0; i < _inventoryController.inventorySize; i++)
        {
            if (i < _listOfUIItems.Count)
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

        _listOfUIItems.ForEach(item => item.Deselect()); // Deselect all

        if (itemData != null)
        {
            itemDescription.SetDescription(itemData.itemIcon, itemData.itemName, itemData.description);
            inventoryItemUI.Select();
        }
        else
        {
            itemDescription.ResetDescription();
        }
    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        if (mouseFollower == null || _inventoryController == null) return;
        int draggedIndex = inventoryItemUI.SlotIndex;
        ItemData itemData = _inventoryController.GetItemDataInSlot(draggedIndex);
        int quantity = _inventoryController.GetQuantityInSlot(draggedIndex);

        if (itemData != null && quantity > 0)
        {
            _currentlyDraggedItemIndex = draggedIndex;
            mouseFollower.Toggle(true);
            mouseFollower.SetData(itemData.itemIcon, quantity); // Use item's actual icon and quantity
            inventoryItemUI.ResetData(); // Visually "pick up" the item
        }
        else
        {
            _currentlyDraggedItemIndex = -1;
        }
    }

    private void HandleDrop(UIInventoryItem droppedOnItemUI) // This is the TARGET slot
    {
        if (_inventoryController == null || _currentlyDraggedItemIndex == -1) return; // Nothing being dragged
        int targetIndex = droppedOnItemUI.SlotIndex;
        if (_currentlyDraggedItemIndex != targetIndex)
        {
            _inventoryController.SwapItems(_currentlyDraggedItemIndex, targetIndex);
        }
        // Eventual UI update will be handled by OnInventorySlotUpdated from InventoryController
        // No need to manually call EndDrag logic here, it's for the dragged item itself.
    }

    private void HandleEndDrag(UIInventoryItem draggedItemUI) // Called on the SOURCE item
    {
        if (mouseFollower != null) mouseFollower.Toggle(false);

        // If a drag was in progress and might not have resulted in a successful swap,
        // refresh the UI of the original slot to reflect its current state in the InventoryController.
        if (_currentlyDraggedItemIndex != -1 && _inventoryController != null)
        {
            ItemData currentData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
            int currentQty = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);
            // Directly update the UI for the slot from which the item was dragged.
            // This ensures its visual state is restored if no swap occurred.
            UpdateSlotUI(_currentlyDraggedItemIndex, currentData, currentQty);
        }
        _currentlyDraggedItemIndex = -1; // Reset drag state
    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        if (_inventoryController == null) return;
        int clickedIndex = inventoryItemUI.SlotIndex;
        // Let InventoryController handle the logic of using an item
        _inventoryController.UseItem(clickedIndex);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (itemDescription != null) itemDescription.ResetDescription();
        // Data refresh should be handled by InventoryController.OnInventoryRefreshed
        // which is called when inventoryUI.Show() is called from InventoryController's Update method.
        // Or when AssignAndInitializeUI is called.
        //if (_inventoryController != null) _inventoryController.OnInventoryRefreshed?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (itemDescription != null) itemDescription.ResetDescription();
        if (mouseFollower != null) mouseFollower.Toggle(false);
        _listOfUIItems.ForEach(item => { if (item != null) item.Deselect(); });
        // If hiding mid-drag, ensure the dragged item's original slot visual is restored
        if (_currentlyDraggedItemIndex != -1 && _inventoryController != null && _currentlyDraggedItemIndex < _listOfUIItems.Count)
        {
            ItemData originalItemData = _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex);
            int originalQuantity = _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex);
            UpdateSlotUI(_currentlyDraggedItemIndex, originalItemData, originalQuantity);
        }
        _currentlyDraggedItemIndex = -1;
    }
}