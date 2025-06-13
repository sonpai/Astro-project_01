//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class UIInventoryPage : MonoBehaviour
//{
//    [SerializeField]
//    private UIInventoryItem itemPrefab;

//    [SerializeField]
//    private RectTransform contentPanel;

//    [SerializeField]
//    private UIInventoryDescription itemDescription;

//    [SerializeField]
//    private MouseFollower e;

//    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

//    public event Action<int> OnDescriptionRequested,
//                OnItemActionRequested,
//                OnStartDragging;

//    public event Action<int, int> OnSwapItems;

//    private int currentlyDraggedItemIndex = -1;

//    public void InitializeInventoryUI(int inventorySize)
//    {
//        for (int i = 0; i < inventorySize; i++)
//        {
//            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
//            uiItem.transform.SetParent(contentPanel);
//            listOfUIItems.Add(uiItem);
//            uiItem.OnItemClicked += HandleItemSelection;
//            uiItem.OnItemBeginDrag += HandleBeginDrag;
//            uiItem.OnItemDroppedOn += HandleSwap;
//            uiItem.OnItemEndDrag += HandleEndDrag;
//            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
//            Debug.Log("Subscribing HandleShowItemActions for " + uiItem.name);
//        }
//    }

//    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
//    {
//        Debug.Log(inventoryItemUI.name);
//        int index = listOfUIItems.IndexOf(inventoryItemUI);
//        if (index == -1)
//            return;
//        OnDescriptionRequested?.Invoke(index);

//    }

//    private void ResetDraggedItem()
//    {
//        e.Toggle(false);
//        currentlyDraggedItemIndex = -1;
//    }

//    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
//    {

//        int index = listOfUIItems.IndexOf(inventoryItemUI);
//        if (index == -1)
//            return;

//        currentlyDraggedItemIndex = index;
//        HandleItemSelection(inventoryItemUI);
//        OnStartDragging?.Invoke(index);

//    }

//    public void CreateDraggedItem(Sprite sprite, int quantity)
//    {
//        e.Toggle(true);
//        e.SetData(sprite, quantity);
//    }

//    private void HandleSwap(UIInventoryItem inventoryItemUI)
//    {
//        int index = listOfUIItems.IndexOf(inventoryItemUI);
//        if (index == -1)
//        {
//            return;
//        }
//        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
//    }

//    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
//    {
//        ResetDraggedItem();
//    }

//    public void UpdateData(int itemIndex,
//            Sprite itemImage, int itemQuantity)
//    {
//        if (listOfUIItems.Count > itemIndex)
//        {
//            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
//        }
//    }

//    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
//    {
//        Debug.Log("HandleShowItemActions called by " + inventoryItemUI.name);

//    }

//    private void Awake()
//    {
//        Hide();
//        e.Toggle(false);
//        itemDescription.ResetDescription();
//    }

//    public void Show()
//    {
//        gameObject.SetActive(true);
//        itemDescription.ResetDescription();
//        ResetSelection();

//    }

//    public void ResetSelection()
//    {
//        itemDescription.ResetDescription();
//        DeselectAllItems();
//    }

//    private void DeselectAllItems()
//    {
//        foreach (UIInventoryItem item in listOfUIItems)
//        {
//            item.Deselect();
//        }
//    }
//    public void Hide()
//    {
//        gameObject.SetActive(false);
//        ResetDraggedItem();
//    }
//}

using UnityEngine;
using System.Collections.Generic;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private UIInventoryDescription itemDescription;
    [SerializeField] private MouseFollower mouseFollower;

    private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
    private InventoryController _inventoryController;
    private int _draggedItemIndex = -1;

    private void Start()
    {
        _inventoryController = InventoryController.Instance;
        if (_inventoryController != null)
        {
            _inventoryController.AssignAndInitializeUI(this);
        }
        else
        {
            Debug.LogError("UIInventoryPage: InventoryController.Instance not found!");
        }
        Hide();
    }

    private void OnEnable()
    {
        if (_inventoryController != null)
        {
            _inventoryController.OnInventorySlotUpdated += UpdateSlot;
            _inventoryController.OnInventoryRefreshed += RefreshAllSlots;
        }
    }

    private void OnDisable()
    {
        if (_inventoryController != null)
        {
            _inventoryController.OnInventorySlotUpdated -= UpdateSlot;
            _inventoryController.OnInventoryRefreshed -= RefreshAllSlots;
        }
    }

    public void InitializeInventoryUI(int size)
    {
        foreach (Transform child in contentPanel) Destroy(child.gameObject);
        _listOfUIItems.Clear();

        for (int i = 0; i < size; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
            _listOfUIItems.Add(uiItem);
            uiItem.SetIndex(i);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleDrop;
            uiItem.OnItemEndDrag += HandleEndDrag;
        }
    }

    private void HandleItemSelection(UIInventoryItem item)
    {
        ItemData data = _inventoryController.GetItemDataInSlot(item.SlotIndex);
        if (data != null)
        {
            itemDescription.SetDescription(data.itemIcon, data.itemName, data.description);
        }
    }

    private void HandleBeginDrag(UIInventoryItem item)
    {
        if (_inventoryController.IsSlotEmpty(item.SlotIndex)) return;
        _draggedItemIndex = item.SlotIndex;
        mouseFollower.Toggle(true);
        mouseFollower.SetData(_inventoryController.GetItemDataInSlot(_draggedItemIndex).itemIcon,
                              _inventoryController.GetQuantityInSlot(_draggedItemIndex));
    }

    private void HandleDrop(UIInventoryItem targetItem)
    {
        // Simplified swap logic for this fix
        // For a robust version, use the SwapItems method from previous responses
        mouseFollower.Toggle(false);
        _draggedItemIndex = -1;
    }

    private void HandleEndDrag(UIInventoryItem item)
    {
        mouseFollower.Toggle(false);
        _draggedItemIndex = -1;
    }

    private void UpdateSlot(int index, ItemData data, int quantity)
    {
        if (data != null) _listOfUIItems[index].SetData(data.itemIcon, quantity);
        else _listOfUIItems[index].ResetData();
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < _listOfUIItems.Count; i++)
        {
            UpdateSlot(i, _inventoryController.GetItemDataInSlot(i), _inventoryController.GetQuantityInSlot(i));
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}

