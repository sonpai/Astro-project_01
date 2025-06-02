// LevelCoinDisplay.cs
using UnityEngine;
using TMPro;

public class LevelCoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinTextElement; // Assign your TextMeshProUGUI for coins here

    private void Start()
    {
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnCoinsChanged += UpdateCoinText;
            UpdateCoinText(PlayerWallet.Instance.CurrentCoins); // Initial display
        }
        else
        {
            Debug.LogError("LevelCoinDisplay: PlayerWallet.Instance not found!");
            if (coinTextElement != null) coinTextElement.text = "Coins: ERR";
        }
    }

    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinText;
        }
    }

    private void UpdateCoinText(int newAmount)
    {
        if (coinTextElement != null)
        {
            coinTextElement.text = $"Coins: {newAmount}";
        }
    }
}