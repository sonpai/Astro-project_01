
// UIInventoryItem.cs
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
//public class UIInventoryItem : MonoBehaviour, IPointerClickHandler

{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

    private bool _empty = true;
    public int SlotIndex { get; private set; }

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
            itemImage.sprite = null;
            itemImage.gameObject.SetActive(false);
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
        if (quantityTxt != null) quantityTxt.text = quantity > 1 ? quantity.ToString() : "";
        _empty = false;
    }

    public void Select()
    {
        if (borderImage != null) borderImage.enabled = true;
    }

    // --- INTERFACE IMPLEMENTATIONS ---
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) OnRightMouseBtnClick?.Invoke(this);
        else OnItemClicked?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.LogError("hi! dragginga");

        if (!_empty) OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData) => OnItemEndDrag?.Invoke(this);
    public void OnDrop(PointerEventData eventData) => OnItemDroppedOn?.Invoke(this);
    public void OnDrag(PointerEventData eventData) { } // Must be implemented for the interface
}