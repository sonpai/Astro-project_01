// ShopUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // If using TextMeshPro

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("Main Panels")]
    public GameObject mainShopPanel; // Assign the overall shop window
    public GameObject buyPanel;       // Assign the panel for buyable items
    public GameObject sellPanel;      // Assign the panel for sellable player items

    [Header("Buttons")]
    public Button openBuyPanelButton;
    public Button openSellPanelButton;
    public Button closeShopButton; // Or back buttons for each panel

    [Header("Item List Containers")]
    public RectTransform buyItemsContainer;  // Parent for shop's items to buy
    public RectTransform sellItemsContainer; // Parent for player's items to sell

    [Header("Item UI Prefabs")]
    public GameObject shopItemUIPrefab; // Prefab for a single item entry in buy/sell lists (has ShopItemUIController.cs)

    [Header("Player Info & Notifications")]
    public TMP_Text playerCoinsText;
    public TMP_Text notificationText;
    public float notificationDisplayTime = 3f;

    private ShopSystem shopSystem; // Reference to the shop's backend logic

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopSystem = GetComponent<ShopSystem>(); // Assuming ShopSystem is on the same GameObject
        if (shopSystem == null) Debug.LogError("ShopUIManager: ShopSystem component not found!");
    }

    void Start()
    {
        if (mainShopPanel != null) mainShopPanel.SetActive(false); // Start hidden
        if (buyPanel != null) buyPanel.SetActive(false);
        if (sellPanel != null) sellPanel.SetActive(false);

        // Setup Button Listeners
        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
        closeShopButton?.onClick.AddListener(HideShop); // Example

        // Subscribe to PlayerWallet for coin updates
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinsDisplay;
            UpdateCoinsDisplay(PlayerWallet.Instance.CurrentCoins); // Initial display
        }
        else Debug.LogWarning("ShopUIManager: PlayerWallet.Instance not found on Start.");
    }

    void OnDestroy()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinsDisplay;
    }

    public void ShowShop() // Called by player interaction (e.g., talking to NPC)
    {
        if (mainShopPanel == null) { Debug.LogError("MainShopPanel not assigned!"); return; }
        mainShopPanel.SetActive(true);
        ShowBuyPanel(); // Default to buy panel or neither
        UpdateCoinsDisplay(PlayerWallet.Instance != null ? PlayerWallet.Instance.CurrentCoins : 0);
    }

    public void HideShop()
    {
        if (mainShopPanel != null) mainShopPanel.SetActive(false);
    }

    void ShowBuyPanel()
    {
        if (buyPanel != null) buyPanel.SetActive(true);
        if (sellPanel != null) sellPanel.SetActive(false);
        PopulateBuyList();
    }

    void ShowSellPanel()
    {
        if (sellPanel != null) sellPanel.SetActive(true);
        if (buyPanel != null) buyPanel.SetActive(false);
        PopulateSellList();
    }

    void PopulateBuyList()
    {
        if (shopSystem == null || buyItemsContainer == null || shopItemUIPrefab == null) return;
        ClearContainer(buyItemsContainer);

        List<ItemData> itemsToBuy = shopSystem.GetItemsForSale(); // Get from ShopSystem
        foreach (ItemData item in itemsToBuy)
        {
            if (item.canBeBought) // Double check
            {
                GameObject itemEntry = Instantiate(shopItemUIPrefab, buyItemsContainer);
                ShopItemUIController entryController = itemEntry.GetComponent<ShopItemUIController>();
                if (entryController != null)
                {
                    entryController.SetupBuyItem(item, shopSystem); // Pass ShopSystem for buy action
                }
            }
        }
    }

    void PopulateSellList()
    {
        if (InventoryController.Instance == null || sellItemsContainer == null || shopItemUIPrefab == null) return;
        ClearContainer(sellItemsContainer);

        // Get sellable items directly from the Player's InventoryController
        for (int i = 0; i < InventoryController.Instance.inventorySize; i++)
        {
            ItemData item = InventoryController.Instance.GetItemDataInSlot(i);
            int quantity = InventoryController.Instance.GetQuantityInSlot(i);

            if (item != null && item.canBeSold && quantity > 0)
            {
                GameObject itemEntry = Instantiate(shopItemUIPrefab, sellItemsContainer);
                ShopItemUIController entryController = itemEntry.GetComponent<ShopItemUIController>();
                if (entryController != null)
                {
                    entryController.SetupSellItem(item, quantity, shopSystem); // Pass ShopSystem for sell action
                }
            }
        }
    }

    void ClearContainer(RectTransform container)
    {
        foreach (Transform child in container) Destroy(child.gameObject);
    }

    public void UpdateCoinsDisplay(int currentCoins)
    {
        if (playerCoinsText != null) playerCoinsText.text = "Coins: " + currentCoins.ToString();
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideNotification)); // Cancel previous hide invoke
            Invoke(nameof(HideNotification), notificationDisplayTime);
        }
    }

    void HideNotification()
    {
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }
}