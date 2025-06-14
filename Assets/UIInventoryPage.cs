////////// UIInventoryPage.cs
////////using UnityEngine;
////////using System.Collections.Generic;

////////public class UIInventoryPage : MonoBehaviour
////////{
////////    [Header("Component References")]
////////    [SerializeField] private UIInventoryItem itemPrefab;
////////    [SerializeField] private RectTransform contentPanel;
////////    [SerializeField] private UIInventoryDescription itemDescription;
////////    [SerializeField] private MouseFollower mouseFollower;

////////    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
////////    private int _currentlyDraggedItemIndex = -1;
////////    private int _currentlySelectedItemIndex = -1;

////////    private void Awake()
////////    {
////////        Hide();
////////        if (mouseFollower != null) mouseFollower.Toggle(false);
////////    }

////////    private void Start()
////////    {
////////        if (InventoryController.Instance != null)
////////        {
////////            InventoryController.Instance.AssignAndInitializeUI(this);
////////        }
////////    }

////////    public void InitializeInventoryUI(int inventorySize)
////////    {
////////        foreach (Transform child in contentPanel) Destroy(child.gameObject);
////////        listOfUIItems.Clear();
////////        for (int i = 0; i < inventorySize; i++)
////////        {
////////            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
////////            listOfUIItems.Add(uiItem);
////////            uiItem.SetIndex(i);

////////            uiItem.OnItemClicked += HandleItemSelection;
////////            uiItem.OnItemBeginDrag += HandleBeginDrag;
////////            uiItem.OnItemDroppedOn += HandleDrop;
////////            uiItem.OnItemEndDrag += HandleEndDrag;
////////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
////////        }
////////    }

////////    private void Update()
////////    {
////////        // Handle dropping the selected item
////////        if (_currentlySelectedItemIndex != -1)
////////        {
////////            if (Input.GetKeyDown(KeyCode.G))
////////            {
////////                InventoryController.Instance.DropItem(_currentlySelectedItemIndex, 1); // Drop 1 item
////////                DeselectAndReset(); // Clear selection after dropping
////////            }
////////        }
////////    }

////////    private void OnEnable()
////////    {
////////        if (InventoryController.Instance != null)
////////        {
////////            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
////////            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
////////        }
////////        RefreshAllSlotsUI();
////////    }

////////    private void OnDisable()
////////    {
////////        if (InventoryController.Instance != null)
////////        {
////////            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
////////            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
////////        }
////////    }

////////    private void UpdateSlotUI(int index, ItemData data, int quantity)
////////    {
////////        if (index >= 0 && index < listOfUIItems.Count)
////////        {
////////            if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
////////            else listOfUIItems[index].ResetData();
////////        }
////////    }

////////    private void RefreshAllSlotsUI()
////////    {
////////        if (InventoryController.Instance == null) return;
////////        for (int i = 0; i < listOfUIItems.Count; i++)
////////        {
////////            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
////////        }
////////    }

////////    // --- EVENT HANDLERS FOR UI ACTIONS ---

////////    private void HandleItemSelection(UIInventoryItem item)
////////    {
////////        DeselectAndReset(); // Reset previous selection
////////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

////////        _currentlySelectedItemIndex = item.SlotIndex;
////////        item.Select();
////////        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
////////        if (data != null && itemDescription != null)
////////        {
////////            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
////////        }
////////    }

////////    private void HandleBeginDrag(UIInventoryItem item)
////////    {
////////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
////////        _currentlyDraggedItemIndex = item.SlotIndex;
////////        if (mouseFollower != null)
////////        {
////////            mouseFollower.Toggle(true);
////////            mouseFollower.SetData(InventoryController.Instance.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon, InventoryController.Instance.GetQuantityInSlot(_currentlyDraggedItemIndex));
////////        }
////////        listOfUIItems[_currentlyDraggedItemIndex].ResetData();
////////    }

////////    private void HandleDrop(UIInventoryItem dropTargetItem)
////////    {
////////        if (_currentlyDraggedItemIndex == -1) return;
////////        InventoryController.Instance.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
////////        HandleEndDrag(null);
////////    }

////////    private void HandleEndDrag(UIInventoryItem item)
////////    {
////////        if (mouseFollower != null) mouseFollower.Toggle(false);
////////        if (_currentlyDraggedItemIndex != -1)
////////        {
////////            RefreshAllSlotsUI(); // A full refresh ensures any cancelled drag is fixed visually
////////        }
////////        _currentlyDraggedItemIndex = -1;
////////    }

////////    private void HandleShowItemActions(UIInventoryItem item) => HandleItemSelection(item);

////////    // --- GENERAL UI CONTROL ---
////////    public void Show() => gameObject.SetActive(true);
////////    public void Hide() { gameObject.SetActive(false); DeselectAndReset(); }

////////    private void DeselectAndReset()
////////    {
////////        if (itemDescription != null) itemDescription.ResetDescription();
////////        _currentlySelectedItemIndex = -1;
////////        foreach (var item in listOfUIItems) item.Deselect();
////////    }
////////}

//////// UIInventoryPage.cs (Corrected with Drop/Use logic and Logs)
//////using UnityEngine;
//////using System.Collections.Generic;

//////public class UIInventoryPage : MonoBehaviour
//////{
//////    [Header("Component References")]
//////    [SerializeField] private UIInventoryItem itemPrefab;
//////    [SerializeField] private RectTransform contentPanel;
//////    [SerializeField] private UIInventoryDescription itemDescription;
//////    [SerializeField] private MouseFollower mouseFollower;

//////    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
//////    private int _currentlyDraggedItemIndex = -1;
//////    private int _currentlySelectedItemIndex = -1;

//////    private void Awake()
//////    {
//////        Hide();
//////        if (mouseFollower != null) mouseFollower.Toggle(false);
//////    }

//////    private void Start()
//////    {
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.AssignAndInitializeUI(this);
//////        }
//////    }

//////    private void Update()
//////    {
//////        if (_currentlySelectedItemIndex != -1)
//////        {
//////            if (Input.GetKeyDown(KeyCode.U))
//////            {
//////                Debug.Log($"UI: 'U' key pressed. Requesting to USE item in slot {_currentlySelectedItemIndex}.");
//////                InventoryController.Instance.UseItem(_currentlySelectedItemIndex);
//////                DeselectAndReset(); // Reset selection after using
//////            }
//////            else if (Input.GetKeyDown(KeyCode.G))
//////            {
//////                Debug.Log($"UI: 'G' key pressed. Requesting to DROP 1 item from slot {_currentlySelectedItemIndex}.");
//////                InventoryController.Instance.DropItem(_currentlySelectedItemIndex, 1); // Drop just one item
//////                DeselectAndReset(); // Reset selection after dropping
//////            }
//////        }
//////    }

//////    // ... (The rest of the script is the same as the previous correct version)

//////    public void InitializeInventoryUI(int inventorySize)
//////    {
//////        foreach (Transform child in contentPanel) Destroy(child.gameObject);
//////        listOfUIItems.Clear();
//////        for (int i = 0; i < inventorySize; i++)
//////        {
//////            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
//////            listOfUIItems.Add(uiItem);
//////            uiItem.SetIndex(i);

//////            uiItem.OnItemClicked += HandleItemSelection;
//////            uiItem.OnItemBeginDrag += HandleBeginDrag;
//////            uiItem.OnItemDroppedOn += HandleDrop;
//////            uiItem.OnItemEndDrag += HandleEndDrag;
//////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//////        }
//////    }

//////    private void OnEnable()
//////    {
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
//////            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
//////        }
//////        RefreshAllSlotsUI();
//////    }

//////    private void OnDisable()
//////    {
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
//////            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
//////        }
//////    }

//////    private void UpdateSlotUI(int index, ItemData data, int quantity)
//////    {
//////        if (index >= 0 && index < listOfUIItems.Count)
//////        {
//////            if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
//////            else listOfUIItems[index].ResetData();
//////        }
//////    }

//////    private void RefreshAllSlotsUI()
//////    {
//////        if (InventoryController.Instance == null) return;
//////        DeselectAndReset();
//////        for (int i = 0; i < listOfUIItems.Count; i++)
//////        {
//////            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
//////        }
//////    }

//////    private void HandleItemSelection(UIInventoryItem item)
//////    {
//////        DeselectAndReset();
//////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

//////        _currentlySelectedItemIndex = item.SlotIndex;
//////        Debug.Log($"UI: Selected item '{InventoryController.Instance.GetItemDataInSlot(item.SlotIndex).itemName}' at slot {_currentlySelectedItemIndex}.");
//////        item.Select();
//////        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
//////        if (data != null && itemDescription != null)
//////        {
//////            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
//////        }
//////    }

//////    private void HandleBeginDrag(UIInventoryItem item)
//////    {
//////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
//////        _currentlyDraggedItemIndex = item.SlotIndex;
//////        Debug.Log($"UI: Begin Drag on slot {_currentlyDraggedItemIndex}.");
//////        if (mouseFollower != null)
//////        {
//////            mouseFollower.Toggle(true);
//////            mouseFollower.SetData(InventoryController.Instance.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon, InventoryController.Instance.GetQuantityInSlot(_currentlyDraggedItemIndex));
//////        }
//////        listOfUIItems[_currentlyDraggedItemIndex].ResetData();
//////    }

//////    private void HandleDrop(UIInventoryItem dropTargetItem)
//////    {
//////        if (_currentlyDraggedItemIndex == -1) return;
//////        Debug.Log($"UI: Dropped on slot {dropTargetItem.SlotIndex}.");
//////        InventoryController.Instance.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
//////        HandleEndDrag(null);
//////    }

//////    private void HandleEndDrag(UIInventoryItem item)
//////    {
//////        if (_currentlyDraggedItemIndex != -1) Debug.Log("UI: Drag operation ended.");
//////        if (mouseFollower != null) mouseFollower.Toggle(false);
//////        if (_currentlyDraggedItemIndex != -1) RefreshAllSlotsUI();
//////        _currentlyDraggedItemIndex = -1;
//////    }

//////    private void HandleShowItemActions(UIInventoryItem item)
//////    {
//////        Debug.Log("UI: Right-click action triggered.");
//////        HandleItemSelection(item);
//////    }

//////    public void Show() { gameObject.SetActive(true); RefreshAllSlotsUI(); }
//////    public void Hide() { gameObject.SetActive(false); DeselectAndReset(); }

//////    private void DeselectAndReset()
//////    {
//////        if (itemDescription != null) itemDescription.ResetDescription();
//////        _currentlySelectedItemIndex = -1;
//////        foreach (var item in listOfUIItems) item.Deselect();
//////    }
//////}

////// PASTE THIS ENTIRE CODE INTO YOUR 'UIInventoryPage.cs' FILE

////using UnityEngine;
////using System.Collections.Generic;

////public class UIInventoryPage : MonoBehaviour
////{
////    [Header("Component References")]
////    [SerializeField] private UIInventoryItem itemPrefab;
////    [SerializeField] private RectTransform contentPanel;
////    [SerializeField] private UIInventoryDescription itemDescription;
////    [SerializeField] private MouseFollower mouseFollower;

////    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
////    private int _currentlyDraggedItemIndex = -1;
////    private int _currentlySelectedItemIndex = -1;

////    private void Awake()
////    {
////        Hide();
////        if (mouseFollower != null) mouseFollower.Toggle(false);
////    }

////    private void Start()
////    {
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.AssignAndInitializeUI(this);
////        }
////    }

////    private void Update()
////    {
////        // This checks if an item is selected, and then listens for the key press.
////        if (_currentlySelectedItemIndex != -1)
////        {
////            if (Input.GetKeyDown(KeyCode.U))
////            {
////                InventoryController.Instance.UseItem(_currentlySelectedItemIndex);
////                DeselectAndReset(); // Deselect after using
////            }
////            else if (Input.GetKeyDown(KeyCode.G))
////            {
////                InventoryController.Instance.DropItem(_currentlySelectedItemIndex, 1);
////                DeselectAndReset(); // Deselect after dropping
////            }
////        }
////    }

////    public void InitializeInventoryUI(int inventorySize)
////    {
////        foreach (Transform child in contentPanel) Destroy(child.gameObject);
////        listOfUIItems.Clear();
////        for (int i = 0; i < inventorySize; i++)
////        {
////            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
////            listOfUIItems.Add(uiItem);
////            uiItem.SetIndex(i);

////            uiItem.OnItemClicked += HandleItemSelection;
////            uiItem.OnItemBeginDrag += HandleBeginDrag;
////            uiItem.OnItemDroppedOn += HandleDrop;
////            uiItem.OnItemEndDrag += HandleEndDrag;
////            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
////        }
////    }

////    private void OnEnable()
////    {
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.OnInventorySlotUpdated += UpdateSlotUI;
////            InventoryController.Instance.OnInventoryRefreshed += RefreshAllSlotsUI;
////        }
////        RefreshAllSlotsUI();
////    }

////    private void OnDisable()
////    {
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.OnInventorySlotUpdated -= UpdateSlotUI;
////            InventoryController.Instance.OnInventoryRefreshed -= RefreshAllSlotsUI;
////        }
////    }

////    private void UpdateSlotUI(int index, ItemData data, int quantity)
////    {
////        if (index >= 0 && index < listOfUIItems.Count)
////        {
////            if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
////            else listOfUIItems[index].ResetData();
////        }
////    }

////    private void RefreshAllSlotsUI()
////    {
////        if (InventoryController.Instance == null) return;
////        DeselectAndReset();
////        for (int i = 0; i < listOfUIItems.Count; i++)
////        {
////            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
////        }
////    }

////    private void HandleItemSelection(UIInventoryItem item)
////    {
////        DeselectAndReset();
////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

////        _currentlySelectedItemIndex = item.SlotIndex;
////        item.Select();
////        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
////        if (data != null && itemDescription != null)
////        {
////            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
////        }
////    }

////    private void HandleBeginDrag(UIInventoryItem item)
////    {
////        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
////        _currentlyDraggedItemIndex = item.SlotIndex;
////        if (mouseFollower != null)
////        {
////            mouseFollower.Toggle(true);
////            mouseFollower.SetData(InventoryController.Instance.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon, InventoryController.Instance.GetQuantityInSlot(_currentlyDraggedItemIndex));
////        }
////        listOfUIItems[_currentlyDraggedItemIndex].ResetData();
////    }

////    private void HandleDrop(UIInventoryItem dropTargetItem)
////    {
////        if (_currentlyDraggedItemIndex == -1) return;
////        InventoryController.Instance.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
////        HandleEndDrag(null);
////    }

////    private void HandleEndDrag(UIInventoryItem item)
////    {
////        if (mouseFollower != null) mouseFollower.Toggle(false);
////        if (_currentlyDraggedItemIndex != -1) RefreshAllSlotsUI();
////        _currentlyDraggedItemIndex = -1;
////    }

////    private void HandleShowItemActions(UIInventoryItem item)
////    {
////        HandleItemSelection(item);
////    }

////    public void Show() { gameObject.SetActive(true); RefreshAllSlotsUI(); }
////    public void Hide() { gameObject.SetActive(false); DeselectAndReset(); }

////    private void DeselectAndReset()
////    {
////        if (itemDescription != null) itemDescription.ResetDescription();
////        _currentlySelectedItemIndex = -1;
////        foreach (var item in listOfUIItems) item.Deselect();
////    }
////}

//// PASTE THIS ENTIRE CODE INTO YOUR 'UIInventoryPage.cs' FILE

//using UnityEngine;
//using System.Collections.Generic;

//public class UIInventoryPage : MonoBehaviour
//{
//    [Header("Component References")]
//    [SerializeField] private UIInventoryItem itemPrefab;
//    [SerializeField] private RectTransform contentPanel;
//    [SerializeField] private UIInventoryDescription itemDescription;
//    [SerializeField] private MouseFollower mouseFollower;

//    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
//    private int _currentlyDraggedItemIndex = -1;
//    private int _currentlySelectedItemIndex = -1;

//    private void Awake()
//    {
//        Hide();
//        if (mouseFollower != null) mouseFollower.Toggle(false);
//    }

//    private void Start()
//    {
//        if (InventoryController.Instance != null)
//        {
//            InventoryController.Instance.AssignAndInitializeUI(this);
//        }
//    }


//    // THIS IS THE MAIN FIX: The Update method to handle all keyboard input
//    private void Update()
//    {
//        // --- Toggle Inventory Visibility ---
//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            // If the inventory UI is currently active, hide it. Otherwise, show it.
//            if (gameObject.activeSelf)
//            {
//                Hide();
//            }
//            else
//            {
//                Show();
//            }
//        }

//        // --- Handle Item Actions (Use/Drop) if an item is selected ---
//        if (_currentlySelectedItemIndex != -1)
//        {
//            if (Input.GetKeyDown(KeyCode.U))
//            {
//                InventoryController.Instance.UseItem(_currentlySelectedItemIndex);
//                DeselectAndReset();
//            }
//            else if (Input.GetKeyDown(KeyCode.G))
//            {
//                InventoryController.Instance.DropItem(_currentlySelectedItemIndex, 1);
//                DeselectAndReset();
//            }
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
//        if (index >= 0 && index < listOfUIItems.Count)
//        {
//            if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
//            else listOfUIItems[index].ResetData();
//        }
//    }

//    private void RefreshAllSlotsUI()
//    {
//        if (InventoryController.Instance == null) return;
//        DeselectAndReset();
//        for (int i = 0; i < listOfUIItems.Count; i++)
//        {
//            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
//        }
//    }

//    private void HandleItemSelection(UIInventoryItem item)
//    {
//        DeselectAndReset();
//        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

//        _currentlySelectedItemIndex = item.SlotIndex;
//        item.Select();
//        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
//        if (data != null && itemDescription != null)
//        {
//            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
//        }
//    }

//    private void HandleBeginDrag(UIInventoryItem item)
//    {
//        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
//        _currentlyDraggedItemIndex = item.SlotIndex;
//        if (mouseFollower != null)
//        {
//            mouseFollower.Toggle(true);
//            mouseFollower.SetData(InventoryController.Instance.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon, InventoryController.Instance.GetQuantityInSlot(_currentlyDraggedItemIndex));
//        }
//        listOfUIItems[_currentlyDraggedItemIndex].ResetData();
//    }

//    private void HandleDrop(UIInventoryItem dropTargetItem)
//    {
//        if (_currentlyDraggedItemIndex == -1) return;
//        InventoryController.Instance.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
//        HandleEndDrag(null);
//    }

//    private void HandleEndDrag(UIInventoryItem item)
//    {
//        if (mouseFollower != null) mouseFollower.Toggle(false);
//        if (_currentlyDraggedItemIndex != -1) RefreshAllSlotsUI();
//        _currentlyDraggedItemIndex = -1;
//    }

//    private void HandleShowItemActions(UIInventoryItem item)
//    {
//        HandleItemSelection(item);
//    }

//    public void Show() { gameObject.SetActive(true); RefreshAllSlotsUI(); }
//    public void Hide() { gameObject.SetActive(false); DeselectAndReset(); }

//    private void DeselectAndReset()
//    {
//        if (itemDescription != null) itemDescription.ResetDescription();
//        _currentlySelectedItemIndex = -1;
//        foreach (var item in listOfUIItems) item.Deselect();
//    }
//}

// PASTE THIS ENTIRE CODE INTO YOUR 'UIInventoryPage.cs' FILE

using UnityEngine;
using System.Collections.Generic;

public class UIInventoryPage : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private UIInventoryDescription itemDescription;
    [SerializeField] private MouseFollower mouseFollower;

    private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
    private int _currentlyDraggedItemIndex = -1;
    private int _currentlySelectedItemIndex = -1;

    private void Awake()
    {
        Hide();
        if (mouseFollower != null) mouseFollower.Toggle(false);
    }

    private void Start()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.AssignAndInitializeUI(this);
        }
    }

    // --- THIS IS THE FIX ---
    // The 'I' key logic has been REMOVED from here.
    // This Update method now ONLY handles actions that should happen
    // when the inventory is already open.
    private void Update()
    {
        if (_currentlySelectedItemIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                InventoryController.Instance.UseItem(_currentlySelectedItemIndex);
                DeselectAndReset();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                InventoryController.Instance.DropItem(_currentlySelectedItemIndex, 1);
                DeselectAndReset();
            }
        }
    }

    public void InitializeInventoryUI(int inventorySize)
    {
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
        if (index >= 0 && index < listOfUIItems.Count)
        {
            if (data != null) listOfUIItems[index].SetData(data.itemIcon, quantity);
            else listOfUIItems[index].ResetData();
        }
    }

    private void RefreshAllSlotsUI()
    {
        if (InventoryController.Instance == null) return;
        DeselectAndReset();
        for (int i = 0; i < listOfUIItems.Count; i++)
        {
            UpdateSlotUI(i, InventoryController.Instance.GetItemDataInSlot(i), InventoryController.Instance.GetQuantityInSlot(i));
        }
    }

    private void HandleItemSelection(UIInventoryItem item)
    {
        DeselectAndReset();
        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;

        _currentlySelectedItemIndex = item.SlotIndex;
        item.Select();
        ItemData data = InventoryController.Instance.GetItemDataInSlot(item.SlotIndex);
        if (data != null && itemDescription != null)
        {
            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
        }
    }

    private void HandleBeginDrag(UIInventoryItem item)
    {
        if (InventoryController.Instance.IsSlotEmpty(item.SlotIndex)) return;
        _currentlyDraggedItemIndex = item.SlotIndex;
        if (mouseFollower != null)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(InventoryController.Instance.GetItemDataInSlot(_currentlyDraggedItemIndex).itemIcon, InventoryController.Instance.GetQuantityInSlot(_currentlyDraggedItemIndex));
        }
        listOfUIItems[_currentlyDraggedItemIndex].ResetData();
    }

    private void HandleDrop(UIInventoryItem dropTargetItem)
    {
        if (_currentlyDraggedItemIndex == -1) return;
        InventoryController.Instance.SwapItems(_currentlyDraggedItemIndex, dropTargetItem.SlotIndex);
        HandleEndDrag(null);
    }

    private void HandleEndDrag(UIInventoryItem item)
    {
        if (mouseFollower != null) mouseFollower.Toggle(false);
        if (_currentlyDraggedItemIndex != -1) RefreshAllSlotsUI();
        _currentlyDraggedItemIndex = -1;
    }

    private void HandleShowItemActions(UIInventoryItem item)
    {
        HandleItemSelection(item);
    }

    public void Show() { gameObject.SetActive(true); RefreshAllSlotsUI(); }
    public void Hide() { gameObject.SetActive(false); DeselectAndReset(); }

    private void DeselectAndReset()
    {
        if (itemDescription != null) itemDescription.ResetDescription();
        _currentlySelectedItemIndex = -1;
        foreach (var item in listOfUIItems) item.Deselect();
    }
}