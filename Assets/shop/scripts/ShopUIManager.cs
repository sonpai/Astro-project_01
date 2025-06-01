// ShopUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System; // For Action

public class ShopUIManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject mainShopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;

    [Header("Buttons")]
    public Button openBuyPanelButton;
    public Button openSellPanelButton;
    public Button backFromBuyButton;
    public Button backFromSellButton;
    // public Button closeShopButton;

    [Header("Item List Containers")]
    public Transform buyItemsContainer;
    public Transform sellItemsContainer;

    [Header("Item UI Prefabs")]
    public GameObject shopItemUIPrefab;

    [Header("Player Info & Notifications")]
    public TextMeshProUGUI playerCoinsText;
    public TextMeshProUGUI notificationText;
    public float notificationDisplayTime = 3f;
    private float _notificationTimer;

    private ShopSystem _shopSystem;

    void Start()
    {
        // For Unity 2023.1+
        _shopSystem = FindFirstObjectByType<ShopSystem>();
        // For older Unity versions, use:
        // _shopSystem = FindObjectOfType<ShopSystem>(); 

        if (_shopSystem == null)
        {
            Debug.LogError("ShopUIManager: ShopSystem not found in the scene!");
            enabled = false;
            return;
        }

        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
        backFromBuyButton?.onClick.AddListener(ShowMainShopPanel);
        backFromSellButton?.onClick.AddListener(ShowMainShopPanel);
        // closeShopButton?.onClick.AddListener(CloseShop);

        // Ensure PlayerWallet and PlayerInventory instances are ready
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinDisplay;
            UpdateCoinDisplay(PlayerWallet.Instance.CurrentCoins); // Initial display
        }
        else Debug.LogError("PlayerWallet.Instance is null in ShopUIManager.Start()");


        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += RefreshSellPanelIfActive;
        }
        else Debug.LogError("PlayerInventory.Instance is null in ShopUIManager.Start()");


        ShopSystem.OnTransactionStatus += ShowNotification;

        ShowMainShopPanel();
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinDisplay;
        if (PlayerInventory.Instance != null) PlayerInventory.Instance.OnInventoryChanged -= RefreshSellPanelIfActive;
        ShopSystem.OnTransactionStatus -= ShowNotification; // Assuming OnTransactionStatus is static
    }

    void Update()
    {
        if (_notificationTimer > 0)
        {
            _notificationTimer -= Time.deltaTime;
            if (_notificationTimer <= 0 && notificationText != null)
            {
                notificationText.gameObject.SetActive(false);
            }
        }
    }

    public void ShowMainShopPanel()
    {
        mainShopPanel?.SetActive(true);
        buyPanel?.SetActive(false);
        sellPanel?.SetActive(false);
    }

    public void ShowBuyPanel()
    {
        mainShopPanel?.SetActive(false);
        buyPanel?.SetActive(true);
        sellPanel?.SetActive(false);
        PopulateBuyList();
    }

    public void ShowSellPanel()
    {
        mainShopPanel?.SetActive(false);
        buyPanel?.SetActive(false);
        sellPanel?.SetActive(true);
        PopulateSellList();
    }

    public void CloseShop()
    {
        Debug.Log("Shop Closed (Implement scene change or UI hide).");
        // Example: gameObject.SetActive(false); 
    }

    private void PopulateBuyList()
    {
        if (buyItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null) return;
        foreach (Transform child in buyItemsContainer) Destroy(child.gameObject);

        foreach (ItemData item in _shopSystem.itemsForSale)
        {
            if (item == null || !item.canBeBought) continue;
            GameObject itemGO = Instantiate(shopItemUIPrefab, buyItemsContainer);
            ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
            if (uiController != null)
            {
                uiController.SetupBuyItem(item, _shopSystem);
            }
        }
    }

    // Inside ShopUIManager.cs
    // ShopUIManager.cs
    // ...
    private void PopulateSellList()
    {
        if (sellItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null ||
            InventoryController.Instance == null || ItemDataManager.Instance == null) // You will still need InventoryController.cs
        {
            Debug.LogError("Missing references for PopulateSellList. Ensure InventoryController is in the scene and its script is correct.");
            return;
        }

        foreach (Transform child in sellItemsContainer) Destroy(child.gameObject);

        var inventoryDataSlots = InventoryController.Instance.InventorySlots_ReadOnly;
        for (int i = 0; i < inventoryDataSlots.Count; i++)
        {
            // ***** FIX HERE *****
            // Change InventorySlotData to InventorySlot, assuming InventorySlots_ReadOnly returns a list of InventorySlot
            InventorySlot slotData = inventoryDataSlots[i];
            // ********************

            if (slotData != null && !slotData.IsEmpty()) // Added null check for slotData itself
            {
                ItemData item = ItemDataManager.Instance.GetItemByID(slotData.itemID);
                if (item != null && item.canBeSold && item.sellPrice > 0)
                {
                    GameObject itemGO = Instantiate(shopItemUIPrefab, sellItemsContainer);
                    ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
                    if (uiController != null)
                    {
                        uiController.SetupSellItem(item, slotData.quantity, _shopSystem);
                    }
                }
            }
        }
    }
    // ...

    // Change subscription in Start/OnDestroy:
    // From: PlayerInventory.Instance.OnInventoryChanged += RefreshSellPanelIfActive;
    // To:
    // if (InventoryController.Instance != null) InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;
    // And also consider OnInventorySlotUpdated if you want more dynamic updates without full refresh.
    // For simplicity, OnInventoryRefreshed is a good start when the whole sell panel might change.
    // Let's use OnInventoryRefreshed and also call it when an item is sold successfully.

    // In ShopUIManager.cs, modify Start() and OnDestroy() for event subscription:
    // ...
    // if (InventoryController.Instance != null)
    // {
    //     InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;
    //     // Also, when an individual slot updates, you might want to refresh if it's a sellable item
    //     InventoryController.Instance.OnInventorySlotUpdated += HandlePotentialSellableItemChange;
    // }
    // ...
    // private void HandlePotentialSellableItemChange(int slotIndex, ItemData itemData, int quantity)
    // {
    //     // If the sell panel is active and this item could have been on it, refresh
    //     if (sellPanel != null && sellPanel.activeSelf)
    //     {
    //         RefreshSellPanelIfActive(); // Simple refresh for now
    //     }
    // }
    // ...

    private void RefreshSellPanelIfActive()
    {
        if (sellPanel != null && sellPanel.activeSelf)
        {
            PopulateSellList();
        }
    }

    private void UpdateCoinDisplay(int newAmount)
    {
        if (playerCoinsText != null)
        {
            playerCoinsText.text = $"Coins: {newAmount}";
        }
    }

    private void ShowNotification(string message, Color color)
    {
        if (notificationText == null) return;
        notificationText.text = message;
        notificationText.color = color;
        notificationText.gameObject.SetActive(true);
        _notificationTimer = notificationDisplayTime;
    }
}