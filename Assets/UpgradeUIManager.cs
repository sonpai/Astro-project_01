// UpgradeUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("Panels & References")]
    public GameObject upgradePanel;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI notificationText;
    public Transform requirementsContainer;
    public GameObject requirementPrefab; // A prefab to display one material requirement
    public Button upgradeButton;

    private UpgradeSystem _upgradeSystem;
    private int _currentTargetSlotIndex = -1;

    private void Start()
    {
        _upgradeSystem = FindFirstObjectByType<UpgradeSystem>();
        //upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        UpgradeSystem.OnUpgradeStatus += ShowNotification;
        upgradePanel.SetActive(false);
    }

    private void OnDestroy() => UpgradeSystem.OnUpgradeStatus -= ShowNotification;

    public void ShowUpgradePanel(int slotIndex)
    {
        ItemData item = InventoryController.Instance.GetItemDataInSlot(slotIndex);
        if (item == null || item.nextLevelUpgrade == null)
        {
            ClosePanel();
            return;
        }

        _currentTargetSlotIndex = slotIndex;

        // Populate UI
        itemIcon.sprite = item.itemIcon;
        itemName.text = $"Upgrade {item.itemName}?";
        itemDescription.text = $"To: {item.nextLevelUpgrade.itemName}";

        // Populate requirements
        foreach (Transform child in requirementsContainer) Destroy(child.gameObject);

        // Coin requirement
        GameObject coinReqObj = Instantiate(requirementPrefab, requirementsContainer);
        coinReqObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.upgradeCost} Coins";

        // Material requirements
        foreach (var req in item.upgradeRequirements)
        {
            GameObject reqObj = Instantiate(requirementPrefab, requirementsContainer);
            reqObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{req.requiredQuantity}x {req.requiredItem.itemName}";
        }

        upgradePanel.SetActive(true);
    }

    private void OnUpgradeButtonClicked()
    {
        if (_currentTargetSlotIndex != -1)
        {
            _upgradeSystem.TryUpgradeItem(_currentTargetSlotIndex);
        }
        ClosePanel();
    }

    public void ClosePanel() => upgradePanel.SetActive(false);
    private void ShowNotification(string message) => notificationText.text = message;
}