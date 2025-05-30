//// InventoryUISlot.cs
//using UnityEngine;
//using UnityEngine.UI; // For Image
//using TMPro;          // For TextMeshProUGUI

//public class InventoryUISlot : MonoBehaviour
//{
//    public Image itemIconImage;
//    public TextMeshProUGUI quantityText;
//    public Button slotButton; // Optional: if you want to click slots for actions

//    private Item _currentItemData;
//    private int _slotIndex; // For interaction with PlayerInventory

//    public void Initialize(int slotIndex)
//    {
//        _slotIndex = slotIndex;
//        if (slotButton != null)
//        {
//            slotButton.onClick.AddListener(OnSlotClicked);
//        }
//        ClearSlotDisplay();
//    }

//    public void UpdateSlotDisplay(Item item, int quantity)
//    {
//        _currentItemData = item;
//        if (item != null && quantity > 0)
//        {
//            if (itemIconImage != null)
//            {
//                itemIconImage.sprite = item.itemIcon;
//                itemIconImage.enabled = true;
//            }
//            if (quantityText != null)
//            {
//                quantityText.text = item.isStackable && quantity > 1 ? quantity.ToString() : "";
//                quantityText.enabled = true;
//            }
//        }
//        else
//        {
//            ClearSlotDisplay();
//        }
//    }

//    public void ClearSlotDisplay()
//    {
//        _currentItemData = null;
//        if (itemIconImage != null)
//        {
//            itemIconImage.sprite = null;
//            itemIconImage.enabled = false;
//        }
//        if (quantityText != null)
//        {
//            quantityText.text = "";
//            quantityText.enabled = false;
//        }
//    }

//    private void OnSlotClicked()
//    {
//        if (_currentItemData != null && PlayerInventory.Instance != null)
//        {
//            Debug.Log($"Clicked on slot {_slotIndex} containing {_currentItemData.itemName}");
//            // Example: Use item on click (if it's not handled by hotkeys)
//            // PlayerInventory.Instance.UseItem(_slotIndex);
//        }
//        else
//        {
//            Debug.Log($"Clicked on empty slot {_slotIndex}");
//        }
//    }
//}