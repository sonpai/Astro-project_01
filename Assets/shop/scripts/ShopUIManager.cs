//// ShopUIManager.cs
//using UnityEngine;
//using UnityEngine.UI; // Required for Button
//using TMPro;          // Required for TextMeshProUGUI
//using System.Collections.Generic;
//using System.Collections;

//public class ShopUIManager : MonoBehaviour
//{
//    public static ShopUIManager Instance { get; private set; }

//    [Header("Main Panels")]
//    public GameObject mainShopPanel; // The root panel for the entire shop interface
//    public GameObject buyPanel;      // Panel specifically for showing items to buy
//    public GameObject sellPanel;     // Panel specifically for showing player's items to sell

//    [Header("Buttons")]
//    public Button openBuyPanelButton;
//    public Button openSellPanelButton;
//    public Button backFromBuyButton;    // Button inside buyPanel to go back to mainShopPanel choices
//    public Button backFromSellButton;   // Button inside sellPanel to go back to mainShopPanel choices
//    public Button closeShopOverallButton; // A general button to close the entire shop interface

//    [Header("Item List Containers")]
//    public Transform buyItemsContainer;  // Parent RectTransform for instantiated buyable item UIs
//    public Transform sellItemsContainer; // Parent RectTransform for instantiated sellable item UIs

//    [Header("Item UI Prefabs")]
//    public GameObject shopItemUIPrefab; // Prefab for displaying a single item in buy/sell lists

//    [Header("Player Info & Notifications")]
//    public TextMeshProUGUI playerCoinsText;
//    public TextMeshProUGUI notificationText;
//    public float notificationDisplayTime = 3f;
//    private float _notificationTimer;

//    private ShopSystem _shopSystem;

//    // Reference to your game's main menu panel (or whatever should reappear when shop closes)
//    public GameObject panelToReactivateOnClose;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            // DontDestroyOnLoad(gameObject); // Optional: if shop UI persists across scenes
//        }
//        else if (Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Initially hide the entire shop UI
//        if (mainShopPanel != null && mainShopPanel.transform.parent != null) // Assuming mainShopPanel is the root or child of root
//            mainShopPanel.transform.parent.gameObject.SetActive(false);
//        else if (mainShopPanel != null)
//            mainShopPanel.SetActive(false); // Fallback if no clear parent
//    }

//    void Start()
//    {
//        _shopSystem = FindFirstObjectByType<ShopSystem>();
//        if (_shopSystem == null)
//        {
//            Debug.LogError("ShopUIManager: ShopSystem not found in the scene! UI will not function.");
//            enabled = false; // Disable this script
//            return;
//        }

//        // Assign button listeners
//        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
//        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
//        backFromBuyButton?.onClick.AddListener(ShowMainShopChoicesPanel);
//        backFromSellButton?.onClick.AddListener(ShowMainShopChoicesPanel);
//        closeShopOverallButton?.onClick.AddListener(CloseEntireShop);

//        // Subscribe to PlayerWallet coin changes
//        if (PlayerWallet.Instance != null)
//        {
//            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinDisplay;
//            UpdateCoinDisplay(PlayerWallet.Instance.CurrentCoins); // Initial display
//        }
//        else Debug.LogError("ShopUIManager: PlayerWallet.Instance is null. Coin display will not update.");

//        // Subscribe to InventoryController changes to refresh sell panel
//        if (InventoryController.Instance != null)
//        {
//            InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;
//            InventoryController.Instance.OnInventorySlotUpdated += HandleInventorySlotUpdateForSellPanel;
//        }
//        else Debug.LogError("ShopUIManager: InventoryController.Instance is null. Sell panel may not update correctly.");

//        if (notificationText != null) notificationText.gameObject.SetActive(false); // Start with notification hidden
//        // Note: The shop typically starts hidden. It's opened by another script (e.g., player interaction).
//    }

//    private void OnDestroy()
//    {
//        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinDisplay;
//        if (InventoryController.Instance != null)
//        {
//            InventoryController.Instance.OnInventoryRefreshed -= RefreshSellPanelIfActive;
//            InventoryController.Instance.OnInventorySlotUpdated -= HandleInventorySlotUpdateForSellPanel;
//        }
//    }

//    private void HandleInventorySlotUpdateForSellPanel(int slotIndex, ItemData itemData, int quantity)
//    {
//        RefreshSellPanelIfActive(); // Refresh sell list if any inventory slot changes
//    }

//    void Update()
//    {
//        // Handle notification timer
//        if (_notificationTimer > 0)
//        {
//            _notificationTimer -= Time.deltaTime;
//            if (_notificationTimer <= 0 && notificationText != null)
//            {
//                notificationText.gameObject.SetActive(false);
//            }
//        }
//    }

//    // Call this from an external script (e.g., player interaction) to open the shop
//    public void OpenEntireShop()
//    {
//        GameObject shopRoot = mainShopPanel?.transform.parent?.gameObject ?? mainShopPanel;
//        if (shopRoot != null)
//        {
//            shopRoot.SetActive(true);
//            ShowMainShopChoicesPanel(); // Show the initial buy/sell choice panel
//            UpdateCoinDisplay(PlayerWallet.Instance != null ? PlayerWallet.Instance.CurrentCoins : 0);

//            if (panelToReactivateOnClose != null)
//                panelToReactivateOnClose.SetActive(false); // Hide the other panel (e.g., main menu)
//        }
//        else Debug.LogError("ShopUIManager: Cannot open shop, mainShopPanel or its parent is not assigned.");
//    }

//    public void CloseEntireShop()
//    {
//        GameObject shopRoot = mainShopPanel?.transform.parent?.gameObject ?? mainShopPanel;
//        if (shopRoot != null)
//        {
//            shopRoot.SetActive(false);
//            if (panelToReactivateOnClose != null)
//                panelToReactivateOnClose.SetActive(true); // Show the other panel again
//        }
//        Debug.Log("Shop UI Closed.");
//    }

//    // Shows the panel with "Buy Items" and "Sell Items" buttons
//    public void ShowMainShopChoicesPanel()
//    {
//        mainShopPanel?.SetActive(true);
//        buyPanel?.SetActive(false);
//        sellPanel?.SetActive(false);
//    }

//    public void ShowBuyPanel()
//    {
//        mainShopPanel?.SetActive(false);
//        buyPanel?.SetActive(true);
//        sellPanel?.SetActive(false);
//        PopulateBuyList();
//    }

//    public void ShowSellPanel()
//    {
//        mainShopPanel?.SetActive(false);
//        buyPanel?.SetActive(false);
//        sellPanel?.SetActive(true);
//        PopulateSellList();
//    }

//    private void PopulateBuyList()
//    {
//        if (buyItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null)
//        {
//            Debug.LogError("PopulateBuyList: Missing references (buyItemsContainer, shopItemUIPrefab, or _shopSystem).");
//            return;
//        }
//        foreach (Transform child in buyItemsContainer) Destroy(child.gameObject); // Clear previous items

//        List<ItemData> itemsToDisplay = _shopSystem.GetItemsForSale();
//        if (itemsToDisplay.Count == 0) Debug.Log("Shop has no items for sale.");

//        foreach (ItemData item in itemsToDisplay)
//        {
//            if (item == null || !item.canBeBought) continue;

//            GameObject itemGO = Instantiate(shopItemUIPrefab, buyItemsContainer);
//            ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
//            if (uiController != null)
//            {
//                uiController.SetupBuyItem(item, _shopSystem); // Pass ShopSystem for buy action
//            }
//            else Debug.LogError($"ShopItemUIController script not found on prefab '{shopItemUIPrefab.name}'.");
//        }
//    }

//    private void PopulateSellList()
//    {
//        if (sellItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null)
//        {
//            Debug.LogError("PopulateSellList: Missing references (sellItemsContainer, shopItemUIPrefab, or _shopSystem).");
//            return;
//        }
//        foreach (Transform child in sellItemsContainer) Destroy(child.gameObject); // Clear previous items

//        if (InventoryController.Instance == null)
//        {
//            Debug.LogError("PopulateSellList: InventoryController.Instance is null.");
//            return;
//        }

//        bool foundSellableItem = false;
//        for (int i = 0; i < InventoryController.Instance.InventorySize; i++)
//        {
//            ItemData itemData = InventoryController.Instance.GetItemDataInSlot(i);
//            int quantity = InventoryController.Instance.GetQuantityInSlot(i);

//            if (itemData != null && quantity > 0 && itemData.canBeSold && itemData.sellPrice > 0)
//            {
//                foundSellableItem = true;
//                GameObject itemGO = Instantiate(shopItemUIPrefab, sellItemsContainer);
//                ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
//                if (uiController != null)
//                {
//                    uiController.SetupSellItem(itemData, quantity, _shopSystem); // Pass ShopSystem for sell action
//                }
//                else Debug.LogError($"ShopItemUIController script not found on prefab '{shopItemUIPrefab.name}'.");
//            }
//        }
//        if (!foundSellableItem) Debug.Log("Player has no sellable items.");
//    }

//    private void RefreshSellPanelIfActive()
//    {
//        if (sellPanel != null && sellPanel.activeSelf)
//        {
//            PopulateSellList();
//        }
//    }

//    private void UpdateCoinDisplay(int newAmount)
//    {
//        if (playerCoinsText != null)
//        {
//            playerCoinsText.text = $"Coins: {newAmount}";
//        }
//    }

//    public void ShowNotification(string message)
//    {
//        if (notificationText == null) return;
//        notificationText.text = message;
//        notificationText.gameObject.SetActive(true);
//        _notificationTimer = notificationDisplayTime; // Reset timer
//    }
//}