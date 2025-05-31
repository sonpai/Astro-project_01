//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System;
//using UnityEngine.EventSystems;



//public class UIInventoryItem : MonoBehaviour , IPointerClickHandler
//{
//    [SerializeField]
//    private Image itemImage;
//    [SerializeField]
//    private TMP_Text quantityTxt;

//    [SerializeField]
//    private Image borderImage;

//      public event Action<UIInventoryItem> OnItemClicked,
//        OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag,
//        OnRightMouseBtnClick;


//    private bool empty = true;

//    public void Awake()
//    {
//        ResetData();
//        Deselect();
//    }
//    public void ResetData()
//    {
//        this.itemImage.gameObject.SetActive(false);
//        empty = true;
//    }
//    public void Deselect()
//    {
//        borderImage.enabled = false;
//    }
//    public void SetData(Sprite sprite, int quantity)
//    {
//        this.itemImage.gameObject.SetActive(true);
//        this.itemImage.sprite = sprite;
//       this.quantityTxt.text = quantity + "";
//        empty = false;
//    }
//    public void Select()
//    {
//        borderImage.enabled = true;
//    }
//    public void OnBeginDrag()
//    {
//        Debug.Log("Drag started");

//        if (empty)
//            return;
//        OnItemBeginDrag?.Invoke(this);
//    }
//    public void OnDrop()
//    {
//        OnItemDroppedOn?.Invoke(this);
//    }

//    public void OnEndDrag()
//    {
//            Debug.Log("Drag ended");

//        OnItemEndDrag?.Invoke(this);
//    }


//    public void OnPointerClick(PointerEventData eventData)
//    {
//        if (eventData.button == PointerEventData.InputButton.Right)
//        {
//            OnRightMouseBtnClick?.Invoke(this);
//        }
//        else
//        {
//            OnItemClicked?.Invoke(this);
//        }
//    }





//}


using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private TMP_Text quantityTxt;
    [SerializeField]
    private Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked,
        OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag,
        OnRightMouseBtnClick;

    private bool empty = true;

    private void Awake()
    {
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false);
        this.quantityTxt.text = "";
        empty = true;
    }

    public void Deselect()
    {
        borderImage.enabled = false;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.quantityTxt.text = quantity.ToString();
        empty = false;
    }

    public void Select()
    {
        borderImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (empty) return;
        Debug.Log("Drag started");
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Optional: Add functionality for visual feedback during dragging.
        Debug.Log("Dragging...");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag ended");
        OnItemEndDrag?.Invoke(this);
    }
}
