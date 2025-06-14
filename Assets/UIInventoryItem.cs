//// UIInventoryItem.cs
//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.EventSystems; // Required for event system interfaces

//public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler 
//{
//    [SerializeField] private Image itemImage;
//    [SerializeField] private TMP_Text quantityTxt;
//    [SerializeField] private Image borderImage; // Used for selection highlight

//    public event Action<UIInventoryItem> OnItemClicked,
//        OnItemDroppedOn, // Event when another item is dropped onto this one
//        OnItemBeginDrag,
//        OnItemEndDrag,
//        OnRightMouseBtnClick;

//    private bool _empty = true;
//    public int SlotIndex { get; private set; } = -1; // To identify which backend slot this UI represents

//    private void Awake()
//    {
//        Debug.Log("1"); // <<< ADD THIS LOG

//        ResetData();
//        Deselect();
//    }

//    public void SetIndex(int index)
//    {
//        SlotIndex = index;
//    }

//    public void ResetData()
//    {
//        // Debug.Log($"UIInventoryItem [{SlotIndex}] ResetData called.");
//        if (itemImage != null)
//        {
//            itemImage.sprite = null; // Important: clear the sprite
//            itemImage.gameObject.SetActive(false); // Hide the image element
//        }
//        if (quantityTxt != null) quantityTxt.text = "";
//        _empty = true;
//    }

//    public void Deselect()
//    {
//        if (borderImage != null) borderImage.enabled = false;
//    }

//    public void SetData(Sprite sprite, int quantity)
//    {
//        Debug.Log("2"); // <<< ADD THIS LOG

//        // Debug.Log($"UIInventoryItem [{SlotIndex}] SetData - Sprite: {(sprite != null ? sprite.name : "NULL")}, Qty: {quantity}");
//        if (itemImage == null) { Debug.LogError($"UIInventoryItem [{SlotIndex}]: itemImage field is NOT ASSIGNED in prefab/inspector!"); return; }
//        if (quantityTxt == null && quantity > 1) { Debug.LogWarning($"UIInventoryItem [{SlotIndex}]: quantityTxt field is NOT ASSIGNED but quantity is > 1!"); }

//        itemImage.gameObject.SetActive(true);
//        itemImage.sprite = sprite;
//        // Debug.Log($"UIInventoryItem [{SlotIndex}]: itemImage.sprite set to {(itemImage.sprite != null ? itemImage.sprite.name : "NULL")}. Active: {itemImage.gameObject.activeSelf}, Enabled: {itemImage.enabled}");


//        if (quantityTxt != null)
//        {
//            quantityTxt.text = quantity > 1 ? quantity.ToString() : "";
//        }
//        _empty = false;
//    }

//    public void Select()
//    {
//        if (borderImage != null) borderImage.enabled = true;
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {

//        if (eventData.button == PointerEventData.InputButton.Right)
//        {
//            OnRightMouseBtnClick?.Invoke(this);
//        }
//        else // Left or Middle click
//        {
//            OnItemClicked?.Invoke(this);
//        }
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        if (_empty) return;
//        OnItemBeginDrag?.Invoke(this);
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        // MouseFollower handles visual dragging. This method is required by IDragHandler.
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // This is called on the item that WAS BEING DRAGGED
//        OnItemEndDrag?.Invoke(this);
//    }

//    public void OnDrop(PointerEventData eventData)
//    {
//        // This is called on the item that another item WAS DROPPED ONTO
//        OnItemDroppedOn?.Invoke(this);
//    }
//}

// UIInventoryItem.cs
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;
    public int SlotIndex { get; private set; }
    private bool _empty = true;

    private void Awake()
    {
        ResetData();
        Deselect();
    }

    public void SetIndex(int index) => SlotIndex = index;

    public void ResetData()
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
        }
        if (quantityTxt != null) quantityTxt.text = "";
        _empty = true;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        if (itemImage == null)
        {
            Debug.LogError($"Item Image is not assigned on the UIInventoryItem prefab!", this.gameObject);
            return;
        }

        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        if (quantityTxt != null) quantityTxt.text = quantity > 1 ? quantity.ToString() : "";
        _empty = false;
    }

    public void Select() { if (borderImage != null) borderImage.enabled = true; }
    public void Deselect() { if (borderImage != null) borderImage.enabled = false; }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) OnRightMouseBtnClick?.Invoke(this);
        else OnItemClicked?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData) { if (!_empty) OnItemBeginDrag?.Invoke(this); }
    public void OnEndDrag(PointerEventData eventData) => OnItemEndDrag?.Invoke(this);
    public void OnDrop(PointerEventData eventData) => OnItemDroppedOn?.Invoke(this);
}