// ShopItemUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUIController : MonoBehaviour
{
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemPriceText;
    [SerializeField] private TMP_Text itemQuantityText; // For sell panel primarily
    [SerializeField] private Button actionButton; // This will be "Buy" or "Sell"

    private ItemData _currentItemData;
    private int _currentQuantityInPlayerInventory; // Only relevant for selling
    private ShopSystem _shopSystemInstance;
    private bool _isBuyMode;

    public void SetupBuyItem(ItemData data, ShopSystem shopSystem)
    {
        _currentItemData = data;
        _shopSystemInstance = shopSystem;
        _isBuyMode = true;

        if (itemIconImage != null) itemIconImage.sprite = data.itemIcon;
        if (itemNameText != null) itemNameText.text = data.itemName;
        if (itemPriceText != null) itemPriceText.text = "Price: " + data.buyPrice.ToString();
        if (itemQuantityText != null) itemQuantityText.gameObject.SetActive(false); // Not needed for buy list

        actionButton?.onClick.RemoveAllListeners();
        actionButton?.onClick.AddListener(OnActionButtonClicked);
        if (actionButton?.GetComponentInChildren<TMP_Text>() != null)
            actionButton.GetComponentInChildren<TMP_Text>().text = "Buy";
    }

    public void SetupSellItem(ItemData data, int quantity, ShopSystem shopSystem)
    {
        _currentItemData = data;
        _currentQuantityInPlayerInventory = quantity;
        _shopSystemInstance = shopSystem;
        _isBuyMode = false;

        if (itemIconImage != null) itemIconImage.sprite = data.itemIcon;
        if (itemNameText != null) itemNameText.text = data.itemName;
        if (itemPriceText != null) itemPriceText.text = "Sell for: " + data.sellPrice.ToString();

        if (itemQuantityText != null)
        {
            itemQuantityText.gameObject.SetActive(true);
            itemQuantityText.text = "Qty: " + quantity.ToString();
        }

        actionButton?.onClick.RemoveAllListeners();
        actionButton?.onClick.AddListener(OnActionButtonClicked);
        if (actionButton?.GetComponentInChildren<TMP_Text>() != null)
            actionButton.GetComponentInChildren<TMP_Text>().text = "Sell 1"; // Or "Sell All" with more logic
    }

    void OnActionButtonClicked()
    {
        if (_currentItemData == null || _shopSystemInstance == null) return;

        if (_isBuyMode)
        {
            _shopSystemInstance.PlayerBuysItem(_currentItemData);
            // The ShopUIManager might re-populate lists if stock changes or coins update.
            // For simplicity, this example doesn't force an immediate list refresh here,
            // assuming coin updates are handled by PlayerWallet events.
        }
        else // Sell Mode
        {
            // For now, always sell 1. You can add quantity selection later.
            bool sold = _shopSystemInstance.PlayerSellsItem(_currentItemData, 1);
            //if (sold)
            //{
            //    // Important: The sell list needs to be refreshed because item quantity changed or item removed.
            //    ShopUIManager.Instance?.ShowSellPanel(); // Re-calls PopulateSellList
            //}
        }
    }
}