using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIInventoryDescription itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    public Sprite image;
    public int quantity;
    public string title, description;

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            Debug.Log("Subscribing HandleShowItemActions for " + uiItem.name);
        }
    }

    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        Debug.Log(inventoryItemUI.name);
        itemDescription.SetDescription(image, title, description);
        inventoryItemUI.Select();

    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        Debug.Log("hi");

        mouseFollower.Toggle(true);
        mouseFollower.SetData(image, quantity);

    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {

    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        mouseFollower.Toggle(false);

    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        Debug.Log("HandleShowItemActions called by " + inventoryItemUI.name);

    }

    private void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
        itemDescription.ResetDescription();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        itemDescription.ResetDescription();
        listOfUIItems[0].SetData(image, quantity);

    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}