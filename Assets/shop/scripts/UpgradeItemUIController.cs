// UpgradeItemUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UpgradeItemUIController : MonoBehaviour
{
    [SerializeField] private Image currentItemIcon;
    [SerializeField] private Image nextItemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private Button upgradeButton;

    private int _slotIndexOfItem;

    public void Setup(ItemData itemData, int slotIndex)
    {
        _slotIndexOfItem = slotIndex;

        currentItemIcon.sprite = itemData.itemIcon;
        nextItemIcon.sprite = itemData.nextLevelUpgrade.itemIcon;
        itemNameText.text = $"{itemData.itemName}  ->  {itemData.nextLevelUpgrade.itemName}";

        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        UpdateRequirementsDisplay(itemData);
    }

    public void UpdateRequirementsDisplay(ItemData itemData)
    {
        bool canUpgrade = CheckRequirements(itemData, out string requirementsString);
        upgradeButton.interactable = canUpgrade;
        requirementsText.text = requirementsString;
    }

    private bool CheckRequirements(ItemData item, out string reqString)
    {
        StringBuilder sb = new StringBuilder();
        bool allReqsMet = true;

        // Check coins
        int playerCoins = PlayerWallet.Instance.CurrentCoins;
        if (playerCoins < item.upgradeCost)
        {
            sb.AppendLine($"<color=red>{playerCoins}/{item.upgradeCost} Coins</color>");
            allReqsMet = false;
        }
        else
        {
            sb.AppendLine($"{playerCoins}/{item.upgradeCost} Coins");
        }

        // Check materials
        foreach (var req in item.upgradeRequirements)
        {
            int owned = InventoryController.Instance.GetTotalQuantityOfItem(req.requiredItem.itemID);
            if (owned < req.requiredQuantity)
            {
                sb.AppendLine($"<color=red>{owned}/{req.requiredQuantity} {req.requiredItem.itemName}</color>");
                allReqsMet = false;
            }
            else
            {
                sb.AppendLine($"{owned}/{req.requiredQuantity} {req.requiredItem.itemName}");
            }
        }

        reqString = sb.ToString();
        return allReqsMet;
    }

    private void OnUpgradeClicked()
    {
        FindFirstObjectByType<UpgradeSystem>().TryUpgradeItem(_slotIndexOfItem);
    }
}