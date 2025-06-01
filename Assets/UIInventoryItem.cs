// UIInventoryItem.cs
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Required for event system interfaces

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image borderImage; // Used for selection highlight

    public event Action<UIInventoryItem> OnItemClicked,
        OnItemDroppedOn, // Event when another item is dropped onto this one
        OnItemBeginDrag,
        OnItemEndDrag,
        OnRightMouseBtnClick;

    private bool _empty = true;
    public int SlotIndex { get; private set; } = -1; // To identify which backend slot this UI represents

    private void Awake()
    {
        ResetData();
        Deselect();
    }

    public void SetIndex(int index)
    {
        SlotIndex = index;
    }

    public void ResetData()
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
            itemImage.sprite = null;
        }
        if (quantityTxt != null) quantityTxt.text = "";
        _empty = true;
    }

    public void Deselect()
    {
        if (borderImage != null) borderImage.enabled = false;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
        }
        if (quantityTxt != null)
        {
            // Show quantity only if > 1 (or based on your preference for stackable items)
            quantityTxt.text = quantity > 1 ? quantity.ToString() : "";
        }
        _empty = false;
    }

    public void Select()
    {
        if (borderImage != null) borderImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_empty && eventData.button != PointerEventData.InputButton.Right) // Don't invoke left click on empty unless needed
        {
            // Optionally, deselect other items or handle empty slot click
            // OnItemClicked?.Invoke(this); // If you want empty slots to be "selectable"
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else // Left or Middle click
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_empty) return;
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // MouseFollower handles visual dragging. This method is required by IDragHandler.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // This is called on the item that WAS BEING DRAGGED
        OnItemEndDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // This is called on the item that another item WAS DROPPED ONTO
        OnItemDroppedOn?.Invoke(this);
    }
}