//////// ShopUIManager.cs
//////using UnityEngine;
//////using UnityEngine.UI; // Required for Button
//////using TMPro;          // Required for TextMeshProUGUI
//////using System.Collections.Generic;
//////using System.Collections;

//////public class ShopUIManager : MonoBehaviour
//////{
//////    public static ShopUIManager Instance { get; private set; }

//////    [Header("Main Panels")]
//////    public GameObject mainShopPanel; // The root panel for the entire shop interface
//////    public GameObject buyPanel;      // Panel specifically for showing items to buy
//////    public GameObject sellPanel;     // Panel specifically for showing player's items to sell

//////    [Header("Buttons")]
//////    public Button openBuyPanelButton;
//////    public Button openSellPanelButton;
//////    public Button backFromBuyButton;    // Button inside buyPanel to go back to mainShopPanel choices
//////    public Button backFromSellButton;   // Button inside sellPanel to go back to mainShopPanel choices
//////    public Button closeShopOverallButton; // A general button to close the entire shop interface

//////    [Header("Item List Containers")]
//////    public Transform buyItemsContainer;  // Parent RectTransform for instantiated buyable item UIs
//////    public Transform sellItemsContainer; // Parent RectTransform for instantiated sellable item UIs

//////    [Header("Item UI Prefabs")]
//////    public GameObject shopItemUIPrefab; // Prefab for displaying a single item in buy/sell lists

//////    [Header("Player Info & Notifications")]
//////    public TextMeshProUGUI playerCoinsText;
//////    public TextMeshProUGUI notificationText;
//////    public float notificationDisplayTime = 3f;
//////    private float _notificationTimer;

//////    private ShopSystem _shopSystem;

//////    // Reference to your game's main menu panel (or whatever should reappear when shop closes)
//////    public GameObject panelToReactivateOnClose;

//////    void Awake()
//////    {
//////        if (Instance == null)
//////        {
//////            Instance = this;
//////            // DontDestroyOnLoad(gameObject); // Optional: if shop UI persists across scenes
//////        }
//////        else if (Instance != this)
//////        {
//////            Destroy(gameObject);
//////            return;
//////        }

//////        // Initially hide the entire shop UI
//////        if (mainShopPanel != null && mainShopPanel.transform.parent != null) // Assuming mainShopPanel is the root or child of root
//////            mainShopPanel.transform.parent.gameObject.SetActive(false);
//////        else if (mainShopPanel != null)
//////            mainShopPanel.SetActive(false); // Fallback if no clear parent
//////    }

//////    void Start()
//////    {
//////        _shopSystem = FindFirstObjectByType<ShopSystem>();
//////        if (_shopSystem == null)
//////        {
//////            Debug.LogError("ShopUIManager: ShopSystem not found in the scene! UI will not function.");
//////            enabled = false; // Disable this script
//////            return;
//////        }

//////        // Assign button listeners
//////        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
//////        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
//////        backFromBuyButton?.onClick.AddListener(ShowMainShopChoicesPanel);
//////        backFromSellButton?.onClick.AddListener(ShowMainShopChoicesPanel);
//////        closeShopOverallButton?.onClick.AddListener(CloseEntireShop);

//////        // Subscribe to PlayerWallet coin changes
//////        if (PlayerWallet.Instance != null)
//////        {
//////            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinDisplay;
//////            UpdateCoinDisplay(PlayerWallet.Instance.CurrentCoins); // Initial display
//////        }
//////        else Debug.LogError("ShopUIManager: PlayerWallet.Instance is null. Coin display will not update.");

//////        // Subscribe to InventoryController changes to refresh sell panel
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;
//////            InventoryController.Instance.OnInventorySlotUpdated += HandleInventorySlotUpdateForSellPanel;
//////        }
//////        else Debug.LogError("ShopUIManager: InventoryController.Instance is null. Sell panel may not update correctly.");

//////        if (notificationText != null) notificationText.gameObject.SetActive(false); // Start with notification hidden
//////        // Note: The shop typically starts hidden. It's opened by another script (e.g., player interaction).
//////    }

//////    private void OnDestroy()
//////    {
//////        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinDisplay;
//////        if (InventoryController.Instance != null)
//////        {
//////            InventoryController.Instance.OnInventoryRefreshed -= RefreshSellPanelIfActive;
//////            InventoryController.Instance.OnInventorySlotUpdated -= HandleInventorySlotUpdateForSellPanel;
//////        }
//////    }

//////    private void HandleInventorySlotUpdateForSellPanel(int slotIndex, ItemData itemData, int quantity)
//////    {
//////        RefreshSellPanelIfActive(); // Refresh sell list if any inventory slot changes
//////    }

//////    void Update()
//////    {
//////        // Handle notification timer
//////        if (_notificationTimer > 0)
//////        {
//////            _notificationTimer -= Time.deltaTime;
//////            if (_notificationTimer <= 0 && notificationText != null)
//////            {
//////                notificationText.gameObject.SetActive(false);
//////            }
//////        }
//////    }

//////    // Call this from an external script (e.g., player interaction) to open the shop
//////    public void OpenEntireShop()
//////    {
//////        GameObject shopRoot = mainShopPanel?.transform.parent?.gameObject ?? mainShopPanel;
//////        if (shopRoot != null)
//////        {
//////            shopRoot.SetActive(true);
//////            ShowMainShopChoicesPanel(); // Show the initial buy/sell choice panel
//////            UpdateCoinDisplay(PlayerWallet.Instance != null ? PlayerWallet.Instance.CurrentCoins : 0);

//////            if (panelToReactivateOnClose != null)
//////                panelToReactivateOnClose.SetActive(false); // Hide the other panel (e.g., main menu)
//////        }
//////        else Debug.LogError("ShopUIManager: Cannot open shop, mainShopPanel or its parent is not assigned.");
//////    }

//////    public void CloseEntireShop()
//////    {
//////        GameObject shopRoot = mainShopPanel?.transform.parent?.gameObject ?? mainShopPanel;
//////        if (shopRoot != null)
//////        {
//////            shopRoot.SetActive(false);
//////            if (panelToReactivateOnClose != null)
//////                panelToReactivateOnClose.SetActive(true); // Show the other panel again
//////        }
//////        Debug.Log("Shop UI Closed.");
//////    }

//////    // Shows the panel with "Buy Items" and "Sell Items" buttons
//////    public void ShowMainShopChoicesPanel()
//////    {
//////        mainShopPanel?.SetActive(true);
//////        buyPanel?.SetActive(false);
//////        sellPanel?.SetActive(false);
//////    }

//////    public void ShowBuyPanel()
//////    {
//////        mainShopPanel?.SetActive(false);
//////        buyPanel?.SetActive(true);
//////        sellPanel?.SetActive(false);
//////        PopulateBuyList();
//////    }

//////    public void ShowSellPanel()
//////    {
//////        mainShopPanel?.SetActive(false);
//////        buyPanel?.SetActive(false);
//////        sellPanel?.SetActive(true);
//////        PopulateSellList();
//////    }

//////    private void PopulateBuyList()
//////    {
//////        if (buyItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null)
//////        {
//////            Debug.LogError("PopulateBuyList: Missing references (buyItemsContainer, shopItemUIPrefab, or _shopSystem).");
//////            return;
//////        }
//////        foreach (Transform child in buyItemsContainer) Destroy(child.gameObject); // Clear previous items

//////        List<ItemData> itemsToDisplay = _shopSystem.GetItemsForSale();
//////        if (itemsToDisplay.Count == 0) Debug.Log("Shop has no items for sale.");

//////        foreach (ItemData item in itemsToDisplay)
//////        {
//////            if (item == null || !item.canBeBought) continue;

//////            GameObject itemGO = Instantiate(shopItemUIPrefab, buyItemsContainer);
//////            ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
//////            if (uiController != null)
//////            {
//////                uiController.SetupBuyItem(item, _shopSystem); // Pass ShopSystem for buy action
//////            }
//////            else Debug.LogError($"ShopItemUIController script not found on prefab '{shopItemUIPrefab.name}'.");
//////        }
//////    }

//////    private void PopulateSellList()
//////    {
//////        if (sellItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null)
//////        {
//////            Debug.LogError("PopulateSellList: Missing references (sellItemsContainer, shopItemUIPrefab, or _shopSystem).");
//////            return;
//////        }
//////        foreach (Transform child in sellItemsContainer) Destroy(child.gameObject); // Clear previous items

//////        if (InventoryController.Instance == null)
//////        {
//////            Debug.LogError("PopulateSellList: InventoryController.Instance is null.");
//////            return;
//////        }

//////        bool foundSellableItem = false;
//////        for (int i = 0; i < InventoryController.Instance.InventorySize; i++)
//////        {
//////            ItemData itemData = InventoryController.Instance.GetItemDataInSlot(i);
//////            int quantity = InventoryController.Instance.GetQuantityInSlot(i);

//////            if (itemData != null && quantity > 0 && itemData.canBeSold && itemData.sellPrice > 0)
//////            {
//////                foundSellableItem = true;
//////                GameObject itemGO = Instantiate(shopItemUIPrefab, sellItemsContainer);
//////                ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
//////                if (uiController != null)
//////                {
//////                    uiController.SetupSellItem(itemData, quantity, _shopSystem); // Pass ShopSystem for sell action
//////                }
//////                else Debug.LogError($"ShopItemUIController script not found on prefab '{shopItemUIPrefab.name}'.");
//////            }
//////        }
//////        if (!foundSellableItem) Debug.Log("Player has no sellable items.");
//////    }

//////    private void RefreshSellPanelIfActive()
//////    {
//////        if (sellPanel != null && sellPanel.activeSelf)
//////        {
//////            PopulateSellList();
//////        }
//////    }

//////    private void UpdateCoinDisplay(int newAmount)
//////    {
//////        if (playerCoinsText != null)
//////        {
//////            playerCoinsText.text = $"Coins: {newAmount}";
//////        }
//////    }

//////    public void ShowNotification(string message)
//////    {
//////        if (notificationText == null) return;
//////        notificationText.text = message;
//////        notificationText.gameObject.SetActive(true);
//////        _notificationTimer = notificationDisplayTime; // Reset timer

//////    }

//////    private void PopulateSellList()
//////    {
//////        // ... (Clear previous items)

//////        var inventorySlots = InventoryController.Instance.InventorySlots_ReadOnly;
//////        for (int i = 0; i < inventorySlots.Count; i++)
//////        {
//////            if (!inventorySlots[i].IsEmpty())
//////            {
//////                ItemData data = ItemDataManager.Instance.GetItemByID(inventorySlots[i].itemID);
//////                if (data.canBeSold)
//////                {
//////                    // ... (Instantiate your shopItemUIPrefab)
//////                    // Pass the slot index 'i' to the UI item so it knows which item to sell
//////                    // shopItemUI.SetupSellItem(data, inventorySlots[i].quantity, i);
//////                }
//////            }
//////        }
//////    }
//////}

////// ShopUIManager.cs
////using UnityEngine;
////using UnityEngine.UI; // Required for Button
////using TMPro;          // Required for TextMeshProUGUI
////using System.Collections.Generic;
////using System.Collections;

////public class ShopUIManager : MonoBehaviour
////{
////    public static ShopUIManager Instance { get; private set; }

////    [Header("Main Panels")]
////    public GameObject mainShopPanel; // The root panel for the entire shop interface
////    public GameObject buyPanel;      // Panel specifically for showing items to buy
////    public GameObject sellPanel;     // Panel specifically for showing player's items to sell

////    [Header("Buttons")]
////    public Button openBuyPanelButton;
////    public Button openSellPanelButton;
////    public Button backFromBuyButton;    // Button inside buyPanel to go back to mainShopPanel choices
////    public Button backFromSellButton;   // Button inside sellPanel to go back to mainShopPanel choices
////    public Button closeShopOverallButton; // A general button to close the entire shop interface

////    [Header("Item List Containers")]
////    public Transform buyItemsContainer;  // Parent RectTransform for instantiated buyable item UIs
////    public Transform sellItemsContainer; // Parent RectTransform for instantiated sellable item UIs

////    [Header("Item UI Prefabs")]
////    public GameObject shopItemUIPrefab; // Prefab for displaying a single item in buy/sell lists

////    [Header("Player Info & Notifications")]
////    public TextMeshProUGUI playerCoinsText;
////    public TextMeshProUGUI notificationText;
////    public float notificationDisplayTime = 3f;
////    private float _notificationTimer;

////    private ShopSystem _shopSystem;

////    // Reference to your game's main menu panel (or whatever should reappear when shop closes)
////    public GameObject panelToReactivateOnClose;

////    void Awake()
////    {
////        if (Instance == null)
////        {
////            Instance = this;
////            // DontDestroyOnLoad(gameObject); // Optional: if shop UI persists across scenes
////        }
////        else if (Instance != this)
////        {
////            Destroy(gameObject);
////            return;
////        }

////        // Initially hide the entire shop UI
////        if (mainShopPanel != null && mainShopPanel.transform.parent != null) // Assuming mainShopPanel is the root or child of root
////            mainShopPanel.transform.parent.gameObject.SetActive(false);
////        else if (mainShopPanel != null)
////            mainShopPanel.SetActive(false); // Fallback if no clear parent
////    }

////    void Start()
////    {
////        _shopSystem = FindFirstObjectByType<ShopSystem>();
////        if (_shopSystem == null)
////        {
////            Debug.LogError("ShopUIManager: ShopSystem not found in the scene! UI will not function.");
////            enabled = false; // Disable this script
////            return;
////        }

////        // Assign button listeners
////        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
////        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
////        backFromBuyButton?.onClick.AddListener(ShowMainShopChoicesPanel);
////        backFromSellButton?.onClick.AddListener(ShowMainShopChoicesPanel);
////        closeShopOverallButton?.onClick.AddListener(CloseEntireShop);

////        // Subscribe to PlayerWallet coin changes
////        if (PlayerWallet.Instance != null)
////        {
////            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinDisplay;
////            UpdateCoinDisplay(PlayerWallet.Instance.CurrentCoins); // Initial display
////        }
////        else Debug.LogError("ShopUIManager: PlayerWallet.Instance is null. Coin display will not update.");

////        // Subscribe to InventoryController changes to refresh sell panel
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;
////            InventoryController.Instance.OnInventorySlotUpdated += HandleInventorySlotUpdateForSellPanel;
////        }
////        else Debug.LogError("ShopUIManager: InventoryController.Instance is null. Sell panel may not update correctly.");

////        if (notificationText != null) notificationText.gameObject.SetActive(false); // Start with notification hidden
////        // Note: The shop typically starts hidden. It's opened by another script (e.g., player interaction).
////    }

////    private void OnDestroy()
////    {
////        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinDisplay;
////        if (InventoryController.Instance != null)
////        {
////            InventoryController.Instance.OnInventoryRefreshed -= RefreshSellPanelIfActive;
////            InventoryController.Instance.OnInventorySlotUpdated -= HandleInventorySlotUpdateForSellPanel;
////        }
////    }

////    private void HandleInventorySlotUpdateForSellPanel(int slotIndex, ItemData itemData, int quantity)
////    {
////        RefreshSellPanelIfActive(); // Refresh sell list if any inventory slot changes
////    }

////    void Update()
////    {
////        // Handle notification timer
////        if (_notificationTimer > 0)
////        {
////            _notificationTimer -= Time.deltaTime;
////            if (_notificationTimer <= 0 && notificationText != null)
////            {
////                notificationText.gameObject.SetActive(false);
////            }
////        }
////    }

////    // Call this from an external script (e.g., player interaction) to open the shop
////    public void OpenEntireShop()
////    {
////        GameObject shopRoot = mainShopPanel?.transform.parent?.gameObject ?? mainShopPanel;
////        if (shopRoot != null)
////        {
////            shopRoot.SetActive(true);
////            ShowMainShopChoicesPanel(); // Show the initial buy/sell choice panel
////            UpdateCoinDisplay(PlayerWallet.Instance != null ? PlayerWallet.Instance.CurrentCoins : 0);

////            if (panelToReactivateOnClose != null)
////                panelToReactivateOnClose.SetActive(false); // Hide the other panel (e.g., main menu)
////        }
////        else Debug.LogError("ShopUIManager: Cannot open shop, mainShopPanel or its parent is not assigned.");
////    }

////    public void CloseEntireShop()
////    {
////        GameObject shopRoot = mainShopPanel?.transform.parent?.gameObject ?? mainShopPanel;
////        if (shopRoot != null)
////        {
////            shopRoot.SetActive(false);
////            if (panelToReactivateOnClose != null)
////                panelToReactivateOnClose.SetActive(true); // Show the other panel again
////        }
////        Debug.Log("Shop UI Closed.");
////    }

////    // Shows the panel with "Buy Items" and "Sell Items" buttons
////    public void ShowMainShopChoicesPanel()
////    {
////        mainShopPanel?.SetActive(true);
////        buyPanel?.SetActive(false);
////        sellPanel?.SetActive(false);
////    }

////    public void ShowBuyPanel()
////    {
////        mainShopPanel?.SetActive(false);
////        buyPanel?.SetActive(true);
////        sellPanel?.SetActive(false);
////        PopulateBuyList();
////    }

////    public void ShowSellPanel()
////    {
////        mainShopPanel?.SetActive(false);
////        buyPanel?.SetActive(false);
////        sellPanel?.SetActive(true);
////        PopulateSellList();
////    }

////    private void PopulateBuyList()
////    {
////        if (buyItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null)
////        {
////            Debug.LogError("PopulateBuyList: Missing references (buyItemsContainer, shopItemUIPrefab, or _shopSystem).");
////            return;
////        }
////        foreach (Transform child in buyItemsContainer) Destroy(child.gameObject); // Clear previous items

////        List<ItemData> itemsToDisplay = _shopSystem.GetItemsForSale();
////        if (itemsToDisplay.Count == 0) Debug.Log("Shop has no items for sale.");

////        foreach (ItemData item in itemsToDisplay)
////        {
////            if (item == null || !item.canBeBought) continue;

////            GameObject itemGO = Instantiate(shopItemUIPrefab, buyItemsContainer);
////            //GameObject itemGO = Instantiate(ShopItemEntry_Prefab_Source, buyItemsContainer);

////            ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
////            if (uiController != null)
////            {
////                uiController.SetupBuyItem(item, _shopSystem); // Pass ShopSystem for buy action
////            }
////            //else Debug.LogError($"ShopItemUIController script not found on prefab '{ShopItemEntry_Prefab_Source.NameText}'.");
////        }
////    }

////    private void PopulateSellList()
////    {
////        if (sellItemsContainer == null || shopItemUIPrefab == null || _shopSystem == null)
////        {
////            Debug.LogError("PopulateSellList: Missing references (sellItemsContainer, shopItemUIPrefab, or _shopSystem).");
////            return;
////        }
////        foreach (Transform child in sellItemsContainer) Destroy(child.gameObject); // Clear previous items

////        if (InventoryController.Instance == null)
////        {
////            Debug.LogError("PopulateSellList: InventoryController.Instance is null.");
////            return;
////        }

////        bool foundSellableItem = false;
////        for (int i = 0; i < InventoryController.Instance.inventorySize; i++)
////        {
////            ItemData itemData = InventoryController.Instance.GetItemDataInSlot(i);
////            int quantity = InventoryController.Instance.GetQuantityInSlot(i);

////            if (itemData != null && quantity > 0 && itemData.canBeSold && itemData.sellPrice > 0)
////            {
////                foundSellableItem = true;
////                GameObject itemGO = Instantiate(shopItemUIPrefab, sellItemsContainer);
////                ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
////                if (uiController != null)
////                {
////                    uiController.SetupSellItem(itemData, quantity, _shopSystem); // Pass ShopSystem for sell action
////                }
////                else Debug.LogError($"ShopItemUIController script not found on prefab '{shopItemUIPrefab.name}'.");
////            }
////        }
////        if (!foundSellableItem) Debug.Log("Player has no sellable items.");
////    }

////    private void RefreshSellPanelIfActive()
////    {
////        if (sellPanel != null && sellPanel.activeSelf)
////        {
////            PopulateSellList();
////        }
////    }

////    private void UpdateCoinDisplay(int newAmount)
////    {
////        if (playerCoinsText != null)
////        {
////            playerCoinsText.text = $"Coins: {newAmount}";
////        }
////    }

////    public void ShowNotification(string message)
////    {
////        if (notificationText == null) return;
////        notificationText.text = message;
////        notificationText.gameObject.SetActive(true);
////        _notificationTimer = notificationDisplayTime; // Reset timer
////    }
////}

//// File: ShopUIManager.cs
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections.Generic;

//public class ShopUIManager : MonoBehaviour
//{
//    public static ShopUIManager Instance { get; private set; }

//    [Header("Main Panels")]
//    public GameObject mainShopPanel;
//    public GameObject buyPanel;
//    public GameObject sellPanel;

//    [Header("Buttons")]
//    public Button openBuyPanelButton;
//    public Button openSellPanelButton;
//    public Button backFromBuyButton;
//    public Button backFromSellButton;
//    public Button closeShopOverallButton;

//    [Header("Item List Containers")]
//    public Transform buyItemsContainer;
//    public Transform sellItemsContainer;

//    [Header("Item UI Prefabs")]
//    public GameObject shopItemUIPrefab;

//    [Header("Player Info & Notifications")]
//    public TextMeshProUGUI playerCoinsText;
//    public TextMeshProUGUI notificationText;
//    public float notificationDisplayTime = 3f;

//    private ShopSystem _shopSystem;
//    private Coroutine _notificationCoroutine;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else if (Instance != this)
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        // Find the shop system in the scene
//        _shopSystem = FindFirstObjectByType<ShopSystem>();
//        if (_shopSystem == null)
//        {
//            Debug.LogError("ShopUIManager Error: No ShopSystem found in the scene! UI will not function.", this);
//            enabled = false;
//            return;
//        }

//        // Assign button listeners
//        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
//        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
//        backFromBuyButton?.onClick.AddListener(ShowMainShopChoicesPanel);
//        backFromSellButton?.onClick.AddListener(ShowMainShopChoicesPanel);
//        closeShopOverallButton?.onClick.AddListener(CloseEntireShop);

//        // Subscribe to events for automatic UI updates
//        if (PlayerWallet.Instance != null)
//        {
//            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinDisplay;
//        }
//        if (InventoryController.Instance != null)
//        {
//            // This event tells us to refresh the sell list whenever the inventory changes
//            InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;
//        }

//        // Initialize UI state
//        if (mainShopPanel.transform.parent != null)
//            mainShopPanel.transform.parent.gameObject.SetActive(false); // Hide the root object
//        else
//            //mainShop - Panel.SetActive(false);

//        notificationText?.gameObject.SetActive(false);
//    }

//    // Unsubscribe from events to prevent memory leaks
//    private void OnDestroy()
//    {
//        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinDisplay;
//        if (InventoryController.Instance != null) InventoryController.Instance.OnInventoryRefreshed -= RefreshSellPanelIfActive;
//    }

//    public void OpenEntireShop()
//    {
//        if (mainShopPanel.transform.parent != null)
//            mainShopPanel.transform.parent.gameObject.SetActive(true);
//        else
//            mainShopPanel.SetActive(true);

//        ShowMainShopChoicesPanel();
//        UpdateCoinDisplay(PlayerWallet.Instance.CurrentCoins);
//    }

//    public void CloseEntireShop()
//    {
//        if (mainShopPanel.transform.parent != null)
//            mainShopPanel.transform.parent.gameObject.SetActive(false);
//        else
//            mainShopPanel.SetActive(false);
//    }

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
//        if (buyItemsContainer == null || shopItemUIPrefab == null) return;

//        // Clear previous items
//        foreach (Transform child in buyItemsContainer)
//        {
//            Destroy(child.gameObject);
//        }

//        List<ItemData> itemsToDisplay = _shopSystem.GetItemsForSale();
//        foreach (ItemData item in itemsToDisplay)
//        {
//            if (item == null || !item.canBeBought) continue;

//            GameObject itemGO = Instantiate(shopItemUIPrefab, buyItemsContainer);
//            ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
//            if (uiController != null)
//            {
//                uiController.SetupBuyItem(item, _shopSystem);
//            }
//        }
//    }

//    private void PopulateSellList()
//    {
//        if (sellItemsContainer == null || shopItemUIPrefab == null || InventoryController.Instance == null) return;

//        // Clear previous items
//        foreach (Transform child in sellItemsContainer)
//        {
//            Destroy(child.gameObject);
//        }

//        // Loop through player's inventory
//        for (int i = 0; i < InventoryController.Instance.inventorySize; i++)
//        {
//            ItemData itemData = InventoryController.Instance.GetItemDataInSlot(i);
//            int quantity = InventoryController.Instance.GetQuantityInSlot(i);

//            if (itemData != null && quantity > 0 && itemData.canBeSold && itemData.sellPrice > 0)
//            {
//                GameObject itemGO = Instantiate(shopItemUIPrefab, sellItemsContainer);
//                ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
//                if (uiController != null)
//                {
//                    uiController.SetupSellItem(itemData, quantity, _shopSystem);
//                }
//            }
//        }
//    }

//    private void RefreshSellPanelIfActive()
//    {
//        if (sellPanel != null && sellPanel.activeInHierarchy)
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
//        if (_notificationCoroutine != null)
//        {
//            StopCoroutine(_notificationCoroutine);
//        }
//        _notificationCoroutine = StartCoroutine(NotificationRoutine(message));
//    }

//    private System.Collections.IEnumerator NotificationRoutine(string message)
//    {
//        notificationText.text = message;
//        notificationText.gameObject.SetActive(true);
//        yield return new WaitForSeconds(notificationDisplayTime);
//        notificationText.gameObject.SetActive(false);
//    }
//}

// File: ShopUIManager.cs (Diagnostic Version)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections; // Required for IEnumerator

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("Main Panels")]
    public GameObject mainShopPanel;
    public GameObject buyPanel;
    public GameObject sellPanel;

    [Header("Buttons")]
    public Button openBuyPanelButton;
    public Button openSellPanelButton;
    public Button backFromBuyButton;
    public Button backFromSellButton;
    public Button closeShopOverallButton;

    [Header("Item List Containers")]
    public Transform buyItemsContainer;
    public Transform sellItemsContainer;

    [Header("Item UI Prefabs")]
    public GameObject shopItemUIPrefab;

    [Header("Player Info & Notifications")]
    public TextMeshProUGUI playerCoinsText;
    public TextMeshProUGUI notificationText;
    public float notificationDisplayTime = 3f;

    private ShopSystem _shopSystem;
    private Coroutine _notificationCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("[ShopUIManager] Starting...");
        // Find the shop system in the scene
        _shopSystem = FindFirstObjectByType<ShopSystem>();
        if (_shopSystem == null)
        {
            Debug.LogError("[ShopUIManager] CRITICAL ERROR: No ShopSystem found in the scene! UI will not function.", this);
            enabled = false;
            return;
        }
        Debug.Log("[ShopUIManager] ShopSystem found successfully.", _shopSystem.gameObject);

        // Assign button listeners
        openBuyPanelButton?.onClick.AddListener(ShowBuyPanel);
        openSellPanelButton?.onClick.AddListener(ShowSellPanel);
        backFromBuyButton?.onClick.AddListener(ShowMainShopChoicesPanel);
        backFromSellButton?.onClick.AddListener(ShowMainShopChoicesPanel);
        closeShopOverallButton?.onClick.AddListener(CloseEntireShop);

        // Subscribe to events for automatic UI updates
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged += UpdateCoinDisplay;
        if (InventoryController.Instance != null) InventoryController.Instance.OnInventoryRefreshed += RefreshSellPanelIfActive;

        // Initialize UI state
        if (mainShopPanel.transform.parent != null)
            mainShopPanel.transform.parent.gameObject.SetActive(false);
        else
            mainShopPanel.SetActive(false);

        notificationText?.gameObject.SetActive(false);
        Debug.Log("[ShopUIManager] Initialization complete. Shop is hidden.");
    }

    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinDisplay;
        if (InventoryController.Instance != null) InventoryController.Instance.OnInventoryRefreshed -= RefreshSellPanelIfActive;
    }

    public void OpenEntireShop()
    {
        Debug.Log("[ShopUIManager] OpenEntireShop() called. Activating UI.");
        if (mainShopPanel.transform.parent != null)
            mainShopPanel.transform.parent.gameObject.SetActive(true);
        else
            mainShopPanel.SetActive(true);

        ShowMainShopChoicesPanel();
        UpdateCoinDisplay(PlayerWallet.Instance.CurrentCoins);
    }

    public void CloseEntireShop()
    {
        Debug.Log("[ShopUIManager] CloseEntireShop() called. Hiding UI.");
        if (mainShopPanel.transform.parent != null)
            mainShopPanel.transform.parent.gameObject.SetActive(false);
        else
            mainShopPanel.SetActive(false);
    }

    public void ShowMainShopChoicesPanel()
    {
        mainShopPanel?.SetActive(true);
        buyPanel?.SetActive(false);
        sellPanel?.SetActive(false);
    }

    public void ShowBuyPanel()
    {
        Debug.Log("[ShopUIManager] ShowBuyPanel() called.");
        mainShopPanel?.SetActive(false);
        buyPanel?.SetActive(true);
        sellPanel?.SetActive(false);
        PopulateBuyList();
    }

    public void ShowSellPanel()
    {
        Debug.Log("[ShopUIManager] ShowSellPanel() called.");
        mainShopPanel?.SetActive(false);
        buyPanel?.SetActive(false);
        sellPanel?.SetActive(true);
        PopulateSellList();
    }

    private void PopulateBuyList()
    {
        Debug.Log("--- [PopulateBuyList] START ---");

        if (buyItemsContainer == null) { Debug.LogError("[PopulateBuyList] Aborting: 'buyItemsContainer' is not assigned in the Inspector!"); return; }
        if (shopItemUIPrefab == null) { Debug.LogError("[PopulateBuyList] Aborting: 'shopItemUIPrefab' is not assigned in the Inspector!"); return; }

        Debug.Log("[PopulateBuyList] Clearing previous items from container.", buyItemsContainer);
        foreach (Transform child in buyItemsContainer)
        {
            Destroy(child.gameObject);
        }

        List<ItemData> itemsToDisplay = _shopSystem.GetItemsForSale();
        Debug.Log($"[PopulateBuyList] ShopSystem provided {itemsToDisplay.Count} item(s) to display.");

        if (itemsToDisplay.Count == 0)
        {
            Debug.LogWarning("[PopulateBuyList] No items to display. Check the 'Shop Stock' list on your ShopSystem object.");
        }

        foreach (ItemData item in itemsToDisplay)
        {
            if (item == null)
            {
                Debug.LogWarning("[PopulateBuyList] Found a NULL item in the shop stock. Skipping.");
                continue;
            }

            Debug.Log($"[PopulateBuyList] Processing item: '{item.itemName}'. Checking if it can be bought...");

            if (item.canBeBought)
            {
                Debug.Log($"[PopulateBuyList] -> YES, '{item.itemName}' is buyable. Instantiating UI prefab.");
                GameObject itemGO = Instantiate(shopItemUIPrefab, buyItemsContainer);
                ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
                if (uiController != null)
                {
                    uiController.SetupBuyItem(item, _shopSystem);
                    Debug.Log($"[PopulateBuyList] -> Successfully set up UI for '{item.itemName}'.");
                }
                else
                {
                    Debug.LogError($"[PopulateBuyList] -> FAILED to get ShopItemUIController script from the instantiated prefab '{shopItemUIPrefab.name}'. Make sure the script is on the prefab!");
                }
            }
            else
            {
                Debug.LogWarning($"[PopulateBuyList] -> SKIPPING '{item.itemName}' because its 'canBeBought' property is FALSE.");
            }
        }
        Debug.Log("--- [PopulateBuyList] END ---");
    }

    private void PopulateSellList()
    {
        Debug.Log("--- [PopulateSellList] START ---");
        if (sellItemsContainer == null) { Debug.LogError("[PopulateSellList] Aborting: 'sellItemsContainer' is not assigned in the Inspector!"); return; }
        if (shopItemUIPrefab == null) { Debug.LogError("[PopulateSellList] Aborting: 'shopItemUIPrefab' is not assigned in the Inspector!"); return; }
        if (InventoryController.Instance == null) { Debug.LogError("[PopulateSellList] Aborting: InventoryController.Instance is not available!"); return; }

        Debug.Log("[PopulateSellList] Clearing previous items from container.", sellItemsContainer);
        foreach (Transform child in sellItemsContainer)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"[PopulateSellList] Looping through player's inventory of size {InventoryController.Instance.inventorySize}...");
        bool foundSellableItem = false;
        for (int i = 0; i < InventoryController.Instance.inventorySize; i++)
        {
            ItemData itemData = InventoryController.Instance.GetItemDataInSlot(i);
            int quantity = InventoryController.Instance.GetQuantityInSlot(i);

            // Check if the slot is empty first
            if (itemData == null || quantity <= 0)
            {
                // This is normal, so we don't log it unless we want excessive spam.
                continue;
            }

            foundSellableItem = true; // We found at least one item
            Debug.Log($"[PopulateSellList] Found item '{itemData.itemName}' (x{quantity}) in slot {i}. Checking if it's sellable...");

            if (itemData.canBeSold && itemData.sellPrice > 0)
            {
                Debug.Log($"[PopulateSellList] -> YES, '{itemData.itemName}' is sellable. Instantiating UI prefab.");
                GameObject itemGO = Instantiate(shopItemUIPrefab, sellItemsContainer);
                ShopItemUIController uiController = itemGO.GetComponent<ShopItemUIController>();
                if (uiController != null)
                {
                    uiController.SetupSellItem(itemData, quantity, _shopSystem);
                    Debug.Log($"[PopulateSellList] -> Successfully set up UI for selling '{itemData.itemName}'.");
                }
                else
                {
                    Debug.LogError($"[PopulateSellList] -> FAILED to get ShopItemUIController script from the instantiated prefab '{shopItemUIPrefab.name}'.");
                }
            }
            else
            {
                Debug.LogWarning($"[PopulateSellList] -> SKIPPING '{itemData.itemName}' because its 'canBeSold' property is FALSE or its sell price is 0.");
            }
        }

        if (!foundSellableItem)
        {
            Debug.LogWarning("[PopulateSellList] Finished looping through inventory. No sellable items were found.");
        }

        Debug.Log("--- [PopulateSellList] END ---");
    }

    private void RefreshSellPanelIfActive()
    {
        if (sellPanel != null && sellPanel.activeInHierarchy)
        {
            Debug.Log("[ShopUIManager] Inventory changed, refreshing active sell panel.");
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

    public void ShowNotification(string message)
    {
        if (notificationText == null) return;
        if (_notificationCoroutine != null) StopCoroutine(_notificationCoroutine);
        _notificationCoroutine = StartCoroutine(NotificationRoutine(message));
    }

    private IEnumerator NotificationRoutine(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(notificationDisplayTime);
        notificationText.gameObject.SetActive(false);
    }
}