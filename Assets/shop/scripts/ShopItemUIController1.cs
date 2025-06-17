////using UnityEngine;
////using UnityEngine.UI;
////using TMPro;

////public class ShopItemUIController : MonoBehaviour
////{
////    [SerializeField] private Image itemIcon;
////    [SerializeField] private TMP_Text itemNameText;
////    [SerializeField] private TMP_Text itemPriceText;
////    [SerializeField] private Button actionButton;

////    private ItemData _item;
////    private ShopSystem _shopSystem;

////    // Configure the UI for an item the player can BUY
////    public void SetupBuyItem(ItemData item, ShopSystem shopSystem)
////    {
////        _item = item;
////        _shopSystem = shopSystem;

////        itemIcon.sprite = _item.itemIcon;
////        itemNameText.text = _item.itemName;
////        itemPriceText.text = $"{_item.buyPrice} Coins";

////        actionButton.onClick.RemoveAllListeners();
////        actionButton.onClick.AddListener(HandleBuyItem);

////        // You might want to change the button text as well
////        actionButton.GetComponentInChildren<TMP_Text>().text = "Buy";
////    }

////    // Configure the UI for an item the player can SELL
////    public void SetupSellItem(ItemData item, int quantity, ShopSystem shopSystem)
////    {
////        _item = item;
////        _shopSystem = shopSystem;

////        itemIcon.sprite = _item.itemIcon;
////        // Show quantity for sellable items
////        itemNameText.text = $"{_item.itemName} (x{quantity})";
////        itemPriceText.text = $"{_item.sellPrice} Coins";

////        actionButton.onClick.RemoveAllListeners();
////        actionButton.onClick.AddListener(HandleSellItem);

////        actionButton.GetComponentInChildren<TMP_Text>().text = "Sell";
////    }

////    private void HandleBuyItem()
////    {
////        if (_item != null && _shopSystem != null)
////        {
////            _shopSystem.PlayerBuysItem(_item);
////        }
////    }

////    private void HandleSellItem()
////    {
////        if (_item != null && _shopSystem != null)
////        {
////            _shopSystem.PlayerSellsItem(_item);
////        }
////    }
////}

//// File: ShopItemUIController.cs
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class ShopItemUIController : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private Image itemIcon;
//    [SerializeField] private TMP_Text itemNameText;
//    [SerializeField] private TMP_Text itemPriceText;
//    [SerializeField] private Button actionButton;
//    [SerializeField] private TMP_Text actionButtonText;

//    private ItemData currentItemData;
//    private ShopSystem currentShopSystem;

//    // Called when populating the BUY list
//    public void SetupBuyItem(ItemData itemData, ShopSystem shopSystem)
//    {
//        if (itemData == null || shopSystem == null) return;

//        currentItemData = itemData;
//        currentShopSystem = shopSystem;

//        // Update UI elements
//        itemIcon.sprite = currentItemData.itemIcon;
//        itemNameText.text = currentItemData.itemName;
//        itemPriceText.text = $"{currentItemData.buyPrice} Coins";
//        actionButtonText.text = "Buy";

//        // Assign the correct listener
//        actionButton.onClick.RemoveAllListeners();
//        actionButton.onClick.AddListener(OnBuyButtonClicked);
//    }

//    // Called when populating the SELL list
//    public void SetupSellItem(ItemData itemData, int quantity, ShopSystem shopSystem)
//    {
//        if (itemData == null || shopSystem == null) return;

//        currentItemData = itemData;
//        currentShopSystem = shopSystem;

//        // Update UI elements
//        itemIcon.sprite = currentItemData.itemIcon;
//        itemNameText.text = $"{currentItemData.itemName} (x{quantity})"; // Show quantity for selling
//        itemPriceText.text = $"{currentItemData.sellPrice} Coins";
//        actionButtonText.text = "Sell";

//        // Assign the correct listener
//        actionButton.onClick.RemoveAllListeners();
//        actionButton.onClick.AddListener(OnSellButtonClicked);
//    }

//    private void OnBuyButtonClicked()
//    {
//        if (currentItemData != null && currentShopSystem != null)
//        {
//            // The ShopSystem handles the logic and notifications
//            currentShopSystem.PlayerBuysItem(currentItemData, 1);
//        }
//    }

//    private void OnSellButtonClicked()
//    {
//        if (currentItemData != null && currentShopSystem != null)
//        {
//            // The ShopSystem handles the logic and notifications
//            bool sold = currentShopSystem.PlayerSellsItem(currentItemData, 1);
//            // If the item was successfully sold, this UI element will be destroyed and rebuilt
//            // when the ShopUIManager refreshes the sell panel.
//        }
//    }
//}