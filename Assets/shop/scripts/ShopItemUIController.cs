// ShopItemUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use TextMeshPro

public class ShopItemUIController : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText; // Used for Buy Price or "Qty: X | Sell Price: Y"
    public Button actionButton; // Buy or Sell button

    private ItemData _currentItem;
    private int _currentQuantityInInventory; // Only relevant for sell panel
    private ShopSystem _shopSystemInstance;
    private bool _isBuyItem; // True if this UI is for buying, false for selling

    public void SetupBuyItem(ItemData item, ShopSystem shopSystem)
    {
        _currentItem = item;
        _shopSystemInstance = shopSystem;
        _isBuyItem = true;

        if (itemIconImage != null) itemIconImage.sprite = item.itemIcon;
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemPriceText != null) itemPriceText.text = $"Price: {item.buyPrice} C";

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners(); // Clear previous
            actionButton.onClick.AddListener(OnActionButtonClicked);
            TextMeshProUGUI buttonText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = "Buy";
            actionButton.interactable = item.canBeBought && PlayerWallet.Instance.HasEnoughCoins(item.buyPrice); // Initial interactability
        }
    }

    public void SetupSellItem(ItemData item, int quantity, ShopSystem shopSystem)
    {
        _currentItem = item;
        _currentQuantityInInventory = quantity;
        _shopSystemInstance = shopSystem;
        _isBuyItem = false;

        if (itemIconImage != null) itemIconImage.sprite = item.itemIcon;
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemPriceText != null) itemPriceText.text = $"Qty: {quantity} | Sell for: {item.sellPrice} C";

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionButtonClicked);
            TextMeshProUGUI buttonText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = "Sell 1"; // Or "Sell All" with more logic
            actionButton.interactable = item.canBeSold && quantity > 0;
        }
    }

    private void OnActionButtonClicked()
    {
        if (_currentItem == null || _shopSystemInstance == null) return;

        if (_isBuyItem)
        {
            _shopSystemInstance.TryPurchaseItem(_currentItem);
            // Optionally, update this button's interactable state after purchase attempt
            if (actionButton != null) actionButton.interactable = _currentItem.canBeBought && PlayerWallet.Instance.HasEnoughCoins(_currentItem.buyPrice);
        }
        else // Selling
        {
            // For simplicity, this sells one at a time.
            // You could add a quantity selector or "Sell All" button.
            _shopSystemInstance.TrySellItem(_currentItem, 1);
            // The sell list will be refreshed by ShopUIManager, so this specific item UI might be destroyed and recreated.
        }
    }

    // Optional: Listen to coin changes to update buy button interactability
    private void OnEnable()
    {
        if (_isBuyItem && _currentItem != null) PlayerWallet.Instance.OnCoinsChanged += HandleCoinsChangedForBuyButton;
    }
    private void OnDisable()
    {
        if (_isBuyItem && _currentItem != null && PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= HandleCoinsChangedForBuyButton;
    }
    private void HandleCoinsChangedForBuyButton(int newCoinAmount)
    {
        if (actionButton != null && _isBuyItem && _currentItem != null)
        {
            actionButton.interactable = _currentItem.canBeBought && newCoinAmount >= _currentItem.buyPrice;
        }
    }
}