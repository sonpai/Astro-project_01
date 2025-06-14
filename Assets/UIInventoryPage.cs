////using System;
////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;


////public class UIInventoryPage : MonoBehaviour
////{
////    [SerializeField]
////    private UIInventoryItem itemPrefab;

////    [SerializeField]
////    private RectTransform contentPanel;

////    [SerializeField]
////    private UIInventoryDescription itemDescription;

////    [SerializeField]
////    private MouseFollower e;

////    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

////    public event Action<int> OnDescriptionRequested,
////                OnItemActionRequested,
////                OnStartDragging;

////    public event Action<int, int> OnSwapItems;

////    private int currentlyDraggedItemIndex = -1;

////    public void InitializeInventoryUI(int inventorySize)
////    {
////        for (int i = 0; i < inventorySize; i++)
////        {
////            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
////            uiItem.transform.SetParent(contentPanel);
////            listOfUIItems.Add(uiItem);
////            uiItem.OnItemClicked += HandleItemSelection;
////            uiItem.OnItemBeginDrag += HandleBeginDrag;
////            uiItem.OnItemDroppedOn += HandleSwap;
////            uiItem.OnItemEndDrag += HandleEndDrag;
////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
////            Debug.Log("Subscribing HandleShowItemActions for " + uiItem.name);
////        }
////    }

////    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
////    {
////        Debug.Log(inventoryItemUI.name);
////        int index = listOfUIItems.IndexOf(inventoryItemUI);
////        if (index == -1)
////            return;
////        OnDescriptionRequested?.Invoke(index);

////    }

////    private void ResetDraggedItem()
////    {
////        e.Toggle(false);
////        currentlyDraggedItemIndex = -1;
////    }

////    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
////    {

////        int index = listOfUIItems.IndexOf(inventoryItemUI);
////        if (index == -1)
////            return;

////        currentlyDraggedItemIndex = index;
////        HandleItemSelection(inventoryItemUI);
////        OnStartDragging?.Invoke(index);

////    }

////    public void CreateDraggedItem(Sprite sprite, int quantity)
////    {
////        e.Toggle(true);
////        e.SetData(sprite, quantity);
////    }

////    private void HandleSwap(UIInventoryItem inventoryItemUI)
////    {
////        int index = listOfUIItems.IndexOf(inventoryItemUI);
////        if (index == -1)
////        {
////            return;
////        }
////        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
////    }

////    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
////    {
////        ResetDraggedItem();
////    }

////    public void UpdateData(int itemIndex,
////            Sprite itemImage, int itemQuantity)
////    {
////        if (listOfUIItems.Count > itemIndex)
////        {
////            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
////        }
////    }

////    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
////    {
////        Debug.Log("HandleShowItemActions called by " + inventoryItemUI.name);

////    }

////    private void Awake()
////    {
////        Hide();
////        e.Toggle(false);
////        itemDescription.ResetDescription();
////    }

////    public void Show()
////    {
////        gameObject.SetActive(true);
////        itemDescription.ResetDescription();
////        ResetSelection();

////    }

////    public void ResetSelection()
////    {
////        itemDescription.ResetDescription();
////        DeselectAllItems();
////    }

////    private void DeselectAllItems()
////    {
////        foreach (UIInventoryItem item in listOfUIItems)
////        {
////            item.Deselect();
////        }
////    }
////    public void Hide()
////    {
////        gameObject.SetActive(false);
////        ResetDraggedItem();
////    }
////}




//// UIInventoryPage.cs (Corrected and Unified Version)
//using UnityEngine;
//using System.Collections.Generic;

//public class UIInventoryPage : MonoBehaviour
//{
//    [Header("Component References")]
//    [SerializeField]
//    private UIInventoryItem itemPrefab; // This MUST be assigned in the inspector

//    [SerializeField]
//    private RectTransform contentPanel;

//    [SerializeField]
//    private UIInventoryDescription itemDescription;

//    [SerializeField]
//    private MouseFollower mouseFollower;

//    // --- Private State ---
//    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
//    private InventoryController _inventoryController; // The backend controller
//    private int _currentlyDraggedItemIndex = -1;

//    // This method is now called by InventoryController when it's ready
//    public void InitializeInventoryUI(int inventorySize)
//    {
//        if (itemPrefab == null)
//        {
//            Debug.LogError("UIInventoryPage: 'Item Prefab' is not assigned in the Inspector!", this);
//            return;
//        }

//        // Clear old items before creating new ones
//        foreach (Transform child in contentPanel)
//        {
//            Destroy(child.gameObject);
//        }
//        listOfUIItems.Clear();

//        for (int i = 0; i < inventorySize; i++)
//        {
//            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
//            listOfUIItems.Add(uiItem);
//            uiItem.SetIndex(i);

//            // Subscribe to events from this specific UI item instance
//            uiItem.OnItemClicked += HandleItemSelection;
//            uiItem.OnItemBeginDrag += HandleBeginDrag;
//            uiItem.OnItemDroppedOn += HandleDrop;
//            //uiItem.OnItemEndDrag += HandleEndDrag;
//            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//        }
//    }

//    // Subscribe to backend events when this UI is enabled
//    private void OnEnable()
//    {
//        if (_inventoryController == null) _inventoryController = InventoryController.Instance;

//        if (_inventoryController != null)
//        {
//            _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
//            _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;
//        }
//    }

//    // Unsubscribe when disabled to prevent errors
//    private void OnDisable()
//    {
//        if (_inventoryController != null)
//        {
//            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
//            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
//        }
//    }

//    // --- UI Update Methods ---

//    private void UpdateSlotUI(int index, ItemData data, int quantity)
//    {
//        if (index < listOfUIItems.Count)
//        {
//            if (data != null)
//                listOfUIItems[index].SetData(data.itemIcon, quantity);
//            else
//                listOfUIItems[index].ResetData();
//        }
//    }

//    private void RefreshAllSlotsUI()
//    {
//        if (_inventoryController == null) return;
//        DeselectAllItems();
//        for (int i = 0; i < listOfUIItems.Count; i++)
//        {
//            UpdateSlotUI(i, _inventoryController.GetItemDataInSlot(i), _inventoryController.GetQuantityInSlot(i));
//        }
//    }

//    // --- Event Handlers for UI Actions ---

//    private void HandleItemSelection(UIInventoryItem item)
//    {
//        DeselectAllItems();
//        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;

//        item.Select();
//        ItemData data = _inventoryController.GetItemDataInSlot(item.SlotIndex);
//        if (data != null)
//        {
//            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
//        }
//    }

//    private void HandleBeginDrag(UIInventoryItem item)
//    {
//        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;

//        _currentlyDraggedItemIndex = item.SlotIndex;
//        mouseFollower.Toggle(true);
//        mouseFollower.SetData(
//            _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon,
//            _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex)
//        );
//        item.ResetData(); // Hide the item in the original slot while dragging
//    }

//    private void HandleDrop(UIInventoryItem dropTargetItem)
//    {
//        if (_currentlyDraggedItemIndex == -1) return;

//        _inventoryController.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
//        //HandleEndDrag(null); // End the drag operation
//    }

//    //private void HandleEndDrag(UIInventoryItem item)
//    //{
//    //    mouseFollower.Toggle(false);

//    //    // If the drag ended without a successful drop, OnInventoryRefreshed will fix the visuals
//    //    if (_currentlyDraggedItemIndex != -1)
//    //    {
//    //        _inventoryController.OnInventoryRefreshed?.Invoke();
//    //    }
//    //    _currentlyDraggedItemIndex = -1;
//    //}

//    private void HandleShowItemActions(UIInventoryItem item)
//    {
//        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;
//        // This is where you would implement logic for right-click (e.g., use, drop)
//        Debug.Log("Right-clicked on: " + _inventoryController.GetItemDataInSlot(item.SlotIndex).itemName);
//    }

//    // --- General UI Control ---

//    public void Show() => gameObject.SetActive(true);
//    public void Hide() => gameObject.SetActive(false);

//    private void DeselectAllItems()
//    {
//        foreach (UIInventoryItem item in listOfUIItems)
//        {
//            item.Deselect();
//        }
//        itemDescription.ResetDescription();
//    }
//}


// UIInventoryPage.cs
using UnityEngine;
using System.Collections.Generic;

public class UIInventoryPage : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIInventoryDescription itemDescription;

    // Note: MouseFollower reference removed, as it's not in the provided code.
    // If you add it back, handle its logic here.

    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
    private int currentlyDraggedItemIndex = -1;

    private void Start()
    {
        // Find the controller and tell it this UI exists.
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.AssignAndInitializeUI(this);
        }
        Hide();
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
            listOfUIItems.Add(uiItem);
            uiItem.SetIndex(i); // Give each slot its index

            // Subscribe to events from this specific UI item instance
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleDrop;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    // Subscribe to backend events when this UI is enabled
    private void OnEnable()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
        }
    }

    // Unsubscribe when disabled
    private void OnDisable()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
        }
    }

    // ---- UI UPDATE METHODS (Called by Controller's Events) ----
    private void UpdateSlotUI(int index, ItemData data, int quantity)
    {
        if (index < 0 || index >= listOfUIItems.Count) return;

        if (data != null)
            listOfUIItems[index].SetData(data.itemIcon, quantity);
        else
            listOfUIItems[index].ResetData();
    }

    private void RefreshAllSlotsUI()
    {
        DeselectAllItems();
        for (int i = 0; i < listOfUIItems.Count; i++)
        {
            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
        }
    }

    // ---- EVENT HANDLERS (From UI Clicks/Drags) ----
    private void HandleItemSelection(UIInventoryItem item)
    {
        DeselectAllItems();
        item.Select();

        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
        if (data != null)
        {
            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
        }
    }

    private void HandleBeginDrag(UIInventoryItem item)
    {
        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
        currentlyDraggedItemIndex = item.SlotIndex;
        // Optionally show mouse follower here
    }

    private void HandleDrop(UIInventoryItem dropTargetItem)
    {
        if (currentlyDraggedItemIndex == -1) return;
        InventoryController.Instance.SwapItems(currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
        HandleEndDrag(null);
    }

    private void HandleEndDrag(UIInventoryItem item)
    {
        currentlyDraggedItemIndex = -1;
        // Optionally hide mouse follower here
        RefreshAllSlotsUI(); // Refresh to fix visuals if drag was cancelled
    }

    private void HandleShowItemActions(UIInventoryItem item) { /* Implement right-click menu here */ }

    // ---- GENERAL UI CONTROL ----
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void DeselectAllItems()
    {
        foreach (UIInventoryItem item in listOfUIItems)
        {
            item.Deselect();
        }
        itemDescription.ResetDescription();
    }
}