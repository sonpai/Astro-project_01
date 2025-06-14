//////////using System;
//////////using System.Collections;
//////////using System.Collections.Generic;
//////////using UnityEngine;


//////////public class UIInventoryPage : MonoBehaviour
//////////{
//////////    [SerializeField]
//////////    private UIInventoryItem itemPrefab;

//////////    [SerializeField]
//////////    private RectTransform contentPanel;

//////////    [SerializeField]
//////////    private UIInventoryDescription itemDescription;

//////////    [SerializeField]
//////////    private MouseFollower e;

//////////    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

//////////    public event Action<int> OnDescriptionRequested,
//////////                OnItemActionRequested,
//////////                OnStartDragging;

//////////    public event Action<int, int> OnSwapItems;

//////////    private int currentlyDraggedItemIndex = -1;

//////////    public void InitializeInventoryUI(int inventorySize)
//////////    {
//////////        for (int i = 0; i < inventorySize; i++)
//////////        {
//////////            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
//////////            uiItem.transform.SetParent(contentPanel);
//////////            listOfUIItems.Add(uiItem);
//////////            uiItem.OnItemClicked += HandleItemSelection;
//////////            uiItem.OnItemBeginDrag += HandleBeginDrag;
//////////            uiItem.OnItemDroppedOn += HandleSwap;
//////////            uiItem.OnItemEndDrag += HandleEndDrag;
//////////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//////////            Debug.Log("Subscribing HandleShowItemActions for " + uiItem.name);
//////////        }
//////////    }

//////////    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
//////////    {
//////////        Debug.Log(inventoryItemUI.name);
//////////        int index = listOfUIItems.IndexOf(inventoryItemUI);
//////////        if (index == -1)
//////////            return;
//////////        OnDescriptionRequested?.Invoke(index);

//////////    }

//////////    private void ResetDraggedItem()
//////////    {
//////////        e.Toggle(false);
//////////        currentlyDraggedItemIndex = -1;
//////////    }

//////////    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
//////////    {

//////////        int index = listOfUIItems.IndexOf(inventoryItemUI);
//////////        if (index == -1)
//////////            return;

//////////        currentlyDraggedItemIndex = index;
//////////        HandleItemSelection(inventoryItemUI);
//////////        OnStartDragging?.Invoke(index);

//////////    }

//////////    public void CreateDraggedItem(Sprite sprite, int quantity)
//////////    {
//////////        e.Toggle(true);
//////////        e.SetData(sprite, quantity);
//////////    }

//////////    private void HandleSwap(UIInventoryItem inventoryItemUI)
//////////    {
//////////        int index = listOfUIItems.IndexOf(inventoryItemUI);
//////////        if (index == -1)
//////////        {
//////////            return;
//////////        }
//////////        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
//////////    }

//////////    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
//////////    {
//////////        ResetDraggedItem();
//////////    }

//////////    public void UpdateData(int itemIndex,
//////////            Sprite itemImage, int itemQuantity)
//////////    {
//////////        if (listOfUIItems.Count > itemIndex)
//////////        {
//////////            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
//////////        }
//////////    }

//////////    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
//////////    {
//////////        Debug.Log("HandleShowItemActions called by " + inventoryItemUI.name);

//////////    }

//////////    private void Awake()
//////////    {
//////////        Hide();
//////////        e.Toggle(false);
//////////        itemDescription.ResetDescription();
//////////    }

//////////    public void Show()
//////////    {
//////////        gameObject.SetActive(true);
//////////        itemDescription.ResetDescription();
//////////        ResetSelection();

//////////    }

//////////    public void ResetSelection()
//////////    {
//////////        itemDescription.ResetDescription();
//////////        DeselectAllItems();
//////////    }

//////////    private void DeselectAllItems()
//////////    {
//////////        foreach (UIInventoryItem item in listOfUIItems)
//////////        {
//////////            item.Deselect();
//////////        }
//////////    }
//////////    public void Hide()
//////////    {
//////////        gameObject.SetActive(false);
//////////        ResetDraggedItem();
//////////    }
//////////}




////////// UIInventoryPage.cs (Corrected and Unified Version)
////////using UnityEngine;
////////using System.Collections.Generic;

////////public class UIInventoryPage : MonoBehaviour
////////{
////////    [Header("Component References")]
////////    [SerializeField]
////////    private UIInventoryItem itemPrefab; // This MUST be assigned in the inspector

////////    [SerializeField]
////////    private RectTransform contentPanel;

////////    [SerializeField]
////////    private UIInventoryDescription itemDescription;

////////    [SerializeField]
////////    private MouseFollower mouseFollower;

////////    // --- Private State ---
////////    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
////////    private InventoryController _inventoryController; // The backend controller
////////    private int _currentlyDraggedItemIndex = -1;

////////    // This method is now called by InventoryController when it's ready
////////    public void InitializeInventoryUI(int inventorySize)
////////    {
////////        if (itemPrefab == null)
////////        {
////////            Debug.LogError("UIInventoryPage: 'Item Prefab' is not assigned in the Inspector!", this);
////////            return;
////////        }

////////        // Clear old items before creating new ones
////////        foreach (Transform child in contentPanel)
////////        {
////////            Destroy(child.gameObject);
////////        }
////////        listOfUIItems.Clear();

////////        for (int i = 0; i < inventorySize; i++)
////////        {
////////            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
////////            listOfUIItems.Add(uiItem);
////////            uiItem.SetIndex(i);

////////            // Subscribe to events from this specific UI item instance
////////            uiItem.OnItemClicked += HandleItemSelection;
////////            uiItem.OnItemBeginDrag += HandleBeginDrag;
////////            uiItem.OnItemDroppedOn += HandleDrop;
////////            //uiItem.OnItemEndDrag += HandleEndDrag;
////////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
////////        }
////////    }

////////    // Subscribe to backend events when this UI is enabled
////////    private void OnEnable()
////////    {
////////        if (_inventoryController == null) _inventoryController = InventoryController.Instance;

////////        if (_inventoryController != null)
////////        {
////////            _inventoryController.OnInventorySlotUpdated += UpdateSlotUI;
////////            _inventoryController.OnInventoryRefreshed += RefreshAllSlotsUI;
////////        }
////////    }

////////    // Unsubscribe when disabled to prevent errors
////////    private void OnDisable()
////////    {
////////        if (_inventoryController != null)
////////        {
////////            _inventoryController.OnInventorySlotUpdated -= UpdateSlotUI;
////////            _inventoryController.OnInventoryRefreshed -= RefreshAllSlotsUI;
////////        }
////////    }

////////    // --- UI Update Methods ---

////////    private void UpdateSlotUI(int index, ItemData data, int quantity)
////////    {
////////        if (index < listOfUIItems.Count)
////////        {
////////            if (data != null)
////////                listOfUIItems[index].SetData(data.itemIcon, quantity);
////////            else
////////                listOfUIItems[index].ResetData();
////////        }
////////    }

////////    private void RefreshAllSlotsUI()
////////    {
////////        if (_inventoryController == null) return;
////////        DeselectAllItems();
////////        for (int i = 0; i < listOfUIItems.Count; i++)
////////        {
////////            UpdateSlotUI(i, _inventoryController.GetItemDataInSlot(i), _inventoryController.GetQuantityInSlot(i));
////////        }
////////    }

////////    // --- Event Handlers for UI Actions ---

////////    private void HandleItemSelection(UIInventoryItem item)
////////    {
////////        DeselectAllItems();
////////        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;

////////        item.Select();
////////        ItemData data = _inventoryController.GetItemDataInSlot(item.SlotIndex);
////////        if (data != null)
////////        {
////////            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
////////        }
////////    }

////////    private void HandleBeginDrag(UIInventoryItem item)
////////    {
////////        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;

////////        _currentlyDraggedItemIndex = item.SlotIndex;
////////        mouseFollower.Toggle(true);
////////        mouseFollower.SetData(
////////            _inventoryController.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon,
////////            _inventoryController.GetQuantityInSlot(_currentlyDraggedItemIndex)
////////        );
////////        item.ResetData(); // Hide the item in the original slot while dragging
////////    }

////////    private void HandleDrop(UIInventoryItem dropTargetItem)
////////    {
////////        if (_currentlyDraggedItemIndex == -1) return;

////////        _inventoryController.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
////////        //HandleEndDrag(null); // End the drag operation
////////    }

////////    //private void HandleEndDrag(UIInventoryItem item)
////////    //{
////////    //    mouseFollower.Toggle(false);

////////    //    // If the drag ended without a successful drop, OnInventoryRefreshed will fix the visuals
////////    //    if (_currentlyDraggedItemIndex != -1)
////////    //    {
////////    //        _inventoryController.OnInventoryRefreshed?.Invoke();
////////    //    }
////////    //    _currentlyDraggedItemIndex = -1;
////////    //}

////////    private void HandleShowItemActions(UIInventoryItem item)
////////    {
////////        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;
////////        // This is where you would implement logic for right-click (e.g., use, drop)
////////        Debug.Log("Right-clicked on: " + _inventoryController.GetItemDataInSlot(item.SlotIndex).itemName);
////////    }

////////    // --- General UI Control ---

////////    public void Show() => gameObject.SetActive(true);
////////    public void Hide() => gameObject.SetActive(false);

////////    private void DeselectAllItems()
////////    {
////////        foreach (UIInventoryItem item in listOfUIItems)
////////        {
////////            item.Deselect();
////////        }
////////        itemDescription.ResetDescription();
////////    }
////////}


//////// UIInventoryPage.cs
//////using UnityEngine;
//////using System.Collections.Generic;

//////public class UIInventoryPage : MonoBehaviour
//////{
//////    [Header("Component References")]
//////    [SerializeField]
//////    private UIInventoryItem itemPrefab;

//////    [SerializeField]
//////    private RectTransform contentPanel;

//////    [SerializeField]
//////    private UIInventoryDescription itemDescription;

//////    // Note: MouseFollower reference removed, as it's not in the provided code.
//////    // If you add it back, handle its logic here.

//////    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
//////    private int currentlyDraggedItemIndex = -1;

//////    private void Start()
//////    {
//////        // Find the controller and tell it this UI exists.
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.AssignAndInitializeUI(this);
//////        }
//////        Hide();
//////    }

//////    public void InitializeInventoryUI(int inventorySize)
//////    {
//////        for (int i = 0; i < inventorySize; i++)
//////        {
//////            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
//////            listOfUIItems.Add(uiItem);
//////            uiItem.SetIndex(i); // Give each slot its index

//////            // Subscribe to events from this specific UI item instance
//////            uiItem.OnItemClicked += HandleItemSelection;
//////            uiItem.OnItemBeginDrag += HandleBeginDrag;
//////            uiItem.OnItemDroppedOn += HandleDrop;
//////            uiItem.OnItemEndDrag += HandleEndDrag;
//////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//////        }
//////    }

//////    // Subscribe to backend events when this UI is enabled
//////    private void OnEnable()
//////    {
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
//////            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
//////        }
//////    }

//////    // Unsubscribe when disabled
//////    private void OnDisable()
//////    {
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
//////            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
//////        }
//////    }

//////    // ---- UI UPDATE METHODS (Called by Controller's Events) ----
//////    private void UpdateSlotUI(int index, ItemData data, int quantity)
//////    {
//////        if (index < 0 || index >= listOfUIItems.Count) return;

//////        if (data != null)
//////            listOfUIItems[index].SetData(data.itemIcon, quantity);
//////        else
//////            listOfUIItems[index].ResetData();
//////    }

//////    private void RefreshAllSlotsUI()
//////    {
//////        DeselectAllItems();
//////        for (int i = 0; i < listOfUIItems.Count; i++)
//////        {
//////            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
//////        }
//////    }

//////    // ---- EVENT HANDLERS (From UI Clicks/Drags) ----
//////    private void HandleItemSelection(UIInventoryItem item)
//////    {
//////        DeselectAllItems();
//////        item.Select();

//////        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
//////        if (data != null)
//////        {
//////            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
//////        }
//////    }

//////    private void HandleBeginDrag(UIInventoryItem item)
//////    {
//////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
//////        currentlyDraggedItemIndex = item.SlotIndex;
//////        // Optionally show mouse follower here
//////    }

//////    private void HandleDrop(UIInventoryItem dropTargetItem)
//////    {
//////        if (currentlyDraggedItemIndex == -1) return;
//////        InventoryController.Instance.SwapItems(currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
//////        HandleEndDrag(null);
//////    }

//////    private void HandleEndDrag(UIInventoryItem item)
//////    {
//////        currentlyDraggedItemIndex = -1;
//////        // Optionally hide mouse follower here
//////        RefreshAllSlotsUI(); // Refresh to fix visuals if drag was cancelled
//////    }

//////    private void HandleShowItemActions(UIInventoryItem item) { /* Implement right-click menu here */ }

//////    // ---- GENERAL UI CONTROL ----
//////    public void Show() => gameObject.SetActive(true);
//////    public void Hide() => gameObject.SetActive(false);

//////    private void DeselectAllItems()
//////    {
//////        foreach (UIInventoryItem item in listOfUIItems)
//////        {
//////            item.Deselect();
//////        }
//////        itemDescription.ResetDescription();
//////    }
//////}

////// UIInventoryPage.cs
////using UnityEngine;
////using System.Collections.Generic;

////public class UIInventoryPage : MonoBehaviour
////{
////    [Header("Component References")]
////    [SerializeField]
////    private UIInventoryItem itemPrefab;

////    [SerializeField]
////    private RectTransform contentPanel;

////    [SerializeField]
////    private UIInventoryDescription itemDescription;

////    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
////    private int currentlyDraggedItemIndex = -1;

////    private void Start()
////    {
////        // Find the controller and tell it this UI exists.
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.AssignAndInitializeUI(this);
////        }
////        Hide();
////    }

////    public void InitializeInventoryUI(int inventorySize)
////    {
////        // Clear old items before creating new ones
////        foreach (Transform child in contentPanel)
////        {
////            Destroy(child.gameObject);
////        }
////        listOfUIItems.Clear();

////        for (int i = 0; i < inventorySize; i++)
////        {
////            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
////            listOfUIItems.Add(uiItem);
////            uiItem.SetIndex(i);

////            // Subscribe to events from this specific UI item instance
////            uiItem.OnItemClicked += HandleItemSelection;
////            uiItem.OnItemBeginDrag += HandleBeginDrag;
////            uiItem.OnItemDroppedOn += HandleDrop;
////            uiItem.OnItemEndDrag += HandleEndDrag;
////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
////        }
////    }

////    // --- LIFECYCLE METHODS FOR RELIABLE UPDATES ---

////    private void OnEnable()
////    {
////        // Subscribe to controller events
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
////            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
////        }
////        // **THE KEY FIX**: Always refresh the entire UI when it becomes visible.
////        // This ensures it's always in sync with the backend data.
////        RefreshAllSlotsUI();
////    }

////    private void OnDisable()
////    {
////        // Unsubscribe to prevent errors when the object is inactive
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
////            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
////        }
////    }

////    // ---- UI UPDATE METHODS ----
////    private void UpdateSlotUI(int index, ItemData data, int quantity)
////    {
////        if (index < 0 || index >= listOfUIItems.Count) return;

////        if (data != null)
////            listOfUIItems[index].SetData(data.itemIcon, quantity);
////        else
////            listOfUIItems[index].ResetData();
////    }

////    private void RefreshAllSlotsUI()
////    {
////        if (InventoryController.Instance == null || listOfUIItems.Count == 0) return;

////        DeselectAllItems();
////        for (int i = 0; i < listOfUIItems.Count; i++)
////        {
////            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
////        }
////    }

////    // ---- EVENT HANDLERS (From UI Clicks/Drags) ----
////    private void HandleItemSelection(UIInventoryItem item)
////    {
////        DeselectAllItems();
////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

////        item.Select();

////        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
////        if (data != null)
////        {
////            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
////        }
////    }

////    private void HandleBeginDrag(UIInventoryItem item)
////    {
////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
////        currentlyDraggedItemIndex = item.SlotIndex;
////        // Logic for mouse follower would go here
////    }

////    private void HandleDrop(UIInventoryItem dropTargetItem)
////    {
////        if (currentlyDraggedItemIndex == -1) return;
////        InventoryController.Instance.SwapItems(currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
////    }

////    private void HandleEndDrag(UIInventoryItem item)
////    {
////        currentlyDraggedItemIndex = -1;
////    }

////    private void HandleShowItemActions(UIInventoryItem item) { /* Implement right-click menu here */ }

////    // ---- GENERAL UI CONTROL ----
////    public void Show() => gameObject.SetActive(true);
////    public void Hide() => gameObject.SetActive(false);

////    private void DeselectAllItems()
////    {
////        foreach (UIInventoryItem item in listOfUIItems)
////        {
////            item.Deselect();
////        }
////        itemDescription.ResetDescription();
////    }
////}


//// UIInventoryPage.cs
//using UnityEngine;
//using System.Collections.Generic;

//public class UIInventoryPage : MonoBehaviour
//{
//    [Header("Component References")]
//    [SerializeField]
//    private UIInventoryItem itemPrefab;

//    [SerializeField]
//    private RectTransform contentPanel;

//    [SerializeField]
//    private UIInventoryDescription itemDescription;

//    [SerializeField]
//    private MouseFollower mouseFollower; // You need this for drag-and-drop visuals

//    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
//    private int currentlyDraggedItemIndex = -1;
//    private int currentlySelectedtemIndex = -1;


//    private void Awake()
//    {
//        // Hide on start
//        Hide();
//        mouseFollower.Toggle(false);
//    }

//    private void Start()
//    {
//        if (InventoryController.Instance != null)
//        {
//            InventoryController.Instance.AssignAndInitializeUI(this);
//        }
//    }

//    public void InitializeInventoryUI(int inventorySize)
//    {
//        foreach (Transform child in contentPanel) Destroy(child.gameObject);
//        listOfUIItems.Clear();

//        for (int i = 0; i < inventorySize; i++)
//        {
//            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
//            listOfUIItems.Add(uiItem);
//            uiItem.SetIndex(i);
//            uiItem.OnItemClicked += HandleItemSelection;
//            uiItem.OnItemBeginDrag += HandleBeginDrag;
//            uiItem.OnItemDroppedOn += HandleDrop;
//            uiItem.OnItemEndDrag += HandleEndDrag;
//            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//        }
//    }

//    // --- LIFECYCLE & REFRESH ---
//    private void OnEnable()
//    {
//        if (InventoryController.Instance != null)
//        {
//            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
//            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
//        }
//        RefreshAllSlotsUI();
//    }

//    private void OnDisable()
//    {
//        if (InventoryController.Instance != null)
//        {
//            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
//            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
//        }
//    }

//    private void UpdateSlotUI(int index, ItemData data, int quantity)
//    {
//        if (index < 0 || index >= listOfUIItems.Count) return;
//        if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
//        else listOfUIItems[index].ResetData();
//    }

//    private void RefreshAllSlotsUI()
//    {
//        if (InventoryController.Instance == null || listOfUIItems.Count == 0) return;
//        for (int i = 0; i < listOfUIItems.Count; i++)
//        {
//            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
//        }
//    }

//    // --- EVENT HANDLERS ---
//    private void HandleItemSelection(UIInventoryItem item)
//    {
//        currentlySelectedtemIndex = item.SlotIndex;
//        DeselectAllItems();
//        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex))
//        {
//            itemDescription.ResetDescription();
//            return;
//        }
//        item.Select();
//        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
//        if (data != null)
//        {
//            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
//        }
//    }

//    private void HandleBeginDrag(UIInventoryItem item)
//    {
//        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
//        currentlyDraggedItemIndex = item.SlotIndex;
//        mouseFollower.Toggle(true);
//        mouseFollower.SetData(
//            InventoryController.Instance.GetItemDataInSlot(currentlyDraggedItemIndex).itemIcon,
//            InventoryController.Instance.GetQuantityInSlot(currentlyDraggedItemIndex)
//        );
//        // Hide the item in the original slot to create the illusion of picking it up
//        listOfUIItems[currentlyDraggedItemIndex].ResetData();
//    }

//    private void HandleDrop(UIInventoryItem dropTargetItem)
//    {
//        if (currentlyDraggedItemIndex == -1) return;
//        InventoryController.Instance.SwapItems(currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
//        HandleEndDrag(null); // Finish the drag operation
//    }

//    private void HandleEndDrag(UIInventoryItem item)
//    {
//        mouseFollower.Toggle(false);
//        // If the drag didn't end on a valid slot, the full refresh will fix the visuals
//        if (currentlyDraggedItemIndex != -1)
//        {
//            RefreshAllSlotsUI();
//        }
//        currentlyDraggedItemIndex = -1;
//    }

//    // --- NEW: Right-Click Actions ---
//    private void HandleShowItemActions(UIInventoryItem item)
//    {
//        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

//        // In a real game, you would pop up a small menu with "Use" and "Drop" buttons.
//        // For this example, we'll use keyboard keys as shortcuts.
//        Debug.Log("Right-clicked on item. Press 'U' to Use or 'G' to Drop.");
//        currentlySelectedtemIndex = item.SlotIndex; // Remember which item is selected for actions
//    }

//    private void Update()
//    {
//        // Check for action inputs if an item is selected
//        if (currentlySelectedtemIndex != -1)
//        {
//            // Use Item
//            if (Input.GetKeyDown(KeyCode.U))
//            {
//                InventoryController.Instance.UseItem(currentlySelectedtemIndex);
//                // After using, deselect or refresh to show changes
//                currentlySelectedtemIndex = -1;
//                itemDescription.ResetDescription();
//                DeselectAllItems();
//            }
//            // Drop Item
//            else if (Input.GetKeyDown(KeyCode.G))
//            {
//                // For simplicity, we drop the whole stack. You could add a quantity dialog.
//                int quantityToDrop = InventoryController.Instance.GetQuantityInSlot(currentlySelectedtemIndex);
//                InventoryController.Instance.DropItem(currentlySelectedtemIndex, quantityToDrop);
//                // After dropping, deselect or refresh to show changes
//                currentlySelectedtemIndex = -1;
//                itemDescription.ResetDescription();
//                DeselectAllItems();
//            }
//        }
//    }

//    // --- GENERAL UI CONTROL ---
//    public void Show() => gameObject.SetActive(true);
//    public void Hide()
//    {
//        gameObject.SetActive(false);
//        // Reset state when hiding
//        DeselectAllItems();
//        HandleEndDrag(null);
//    }

//    private void DeselectAllItems()
//    {
//        foreach (UIInventoryItem item in listOfUIItems)
//        {
//            item.Deselect();
//        }
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
    private RectTransform contentPanel; // This should be the 'Content' object inside the Scroll View

    [SerializeField]
    private UIInventoryDescription itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;

    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
    private int currentlyDraggedItemIndex = -1;
    private int currentlySelectedtemIndex = -1;

    private void Awake()
    {
        // Start hidden
        Hide();
        if (mouseFollower != null) mouseFollower.Toggle(false);
    }

    private void Start()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.AssignAndInitializeUI(this);
        }
        else
        {
            Debug.LogError("UIInventoryPage: InventoryController.Instance is not found!");
        }
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        if (contentPanel == null || itemPrefab == null)
        {
            Debug.LogError("UIInventoryPage is not set up correctly in the Inspector!");
            return;
        }

        foreach (Transform child in contentPanel) Destroy(child.gameObject);
        listOfUIItems.Clear();

        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
            listOfUIItems.Add(uiItem);
            uiItem.SetIndex(i);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleDrop;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    // --- LIFECYCLE & REFRESH ---

    private void OnEnable()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
        }
        RefreshAllSlotsUI();
    }

    private void OnDisable()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
        }
    }

    private void UpdateSlotUI(int index, ItemData data, int quantity)
    {
        if (index < 0 || index >= listOfUIItems.Count) return;

        if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
        else listOfUIItems[index].ResetData();
    }

    private void RefreshAllSlotsUI()
    {
        if (InventoryController.Instance == null || listOfUIItems.Count == 0) return;

        DeselectAllItems();
        for (int i = 0; i < listOfUIItems.Count; i++)
        {
            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
        }
    }

    // --- EVENT HANDLERS ---
    private void HandleItemSelection(UIInventoryItem item)
    {
        currentlySelectedtemIndex = item.SlotIndex;
        DeselectAllItems();
        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex))
        {
            if (itemDescription != null) itemDescription.ResetDescription();
            return;
        }
        item.Select();
        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
        if (data != null && itemDescription != null)
        {
            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
        }
    }

    private void HandleBeginDrag(UIInventoryItem item)
    {
        Debug.LogError("drag");
        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

        currentlyDraggedItemIndex = item.SlotIndex;
        if (mouseFollower != null)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(
                InventoryController.Instance.GetItemDataInSlot(currentlyDraggedItemIndex).itemIcon,
                InventoryController.Instance.GetQuantityInSlot(currentlyDraggedItemIndex)
            );
        }
        listOfUIItems[currentlyDraggedItemIndex].ResetData();
    }


    private void HandleDrop(UIInventoryItem dropTargetItem)
    {
        if (currentlyDraggedItemIndex == -1) return;
        InventoryController.Instance.SwapItems(currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
    }

    private void HandleEndDrag(UIInventoryItem item)
    {

        Debug.LogError("enddrag");

        // This is called when the drag ends, regardless of whether it was on a valid slot.
        // We need to hide the follower and reset the drag state.
        if (mouseFollower != null) mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;

        // A full refresh will fix any visual glitches, like if the item was
        // dropped outside the inventory, it will snap back to its original slot.
        RefreshAllSlotsUI();
    }

    private void HandleShowItemActions(UIInventoryItem item)
    {
        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
        Debug.Log("Right-clicked on item. Press 'U' to Use or 'G' to Drop.");
        currentlySelectedtemIndex = item.SlotIndex;
    }

    private void Update()
    {
        if (currentlySelectedtemIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                InventoryController.Instance.UseItem(currentlySelectedtemIndex);
                DeselectAndReset();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                int quantityToDrop = InventoryController.Instance.GetQuantityInSlot(currentlySelectedtemIndex);
                InventoryController.Instance.DropItem(currentlySelectedtemIndex, quantityToDrop);
                DeselectAndReset();
            }
        }
    }

    // --- GENERAL UI CONTROL ---
    public void Show() => gameObject.SetActive(true);

    public void Hide()
    {
        gameObject.SetActive(false);
        DeselectAndReset();
        if (mouseFollower != null) mouseFollower.Toggle(false);
    }

    private void DeselectAndReset()
    {
        DeselectAllItems();
        if (itemDescription != null) itemDescription.ResetDescription();
        currentlySelectedtemIndex = -1;
    }

    private void DeselectAllItems()
    {
        foreach (UIInventoryItem item in listOfUIItems)
        {
            item.Deselect();
        }
    }
}