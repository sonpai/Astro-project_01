// PlayerWallet.cs
using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{

    private int _currentCoins;
    public int CurrentCoins => _currentCoins;

    public event Action<int> OnCoinsChanged; // Passes new coin amount

    private const string COINS_PREFS_KEY = "PlayerTotalCoins";

    public static PlayerWallet Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasEnoughCoins(int amount)
    {
        return _currentCoins >= amount;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        _currentCoins += amount;
        OnCoinsChanged?.Invoke(_currentCoins);
        SaveCoins();
        Debug.Log($"{amount} coins added. Total: {_currentCoins}");
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return false;
        if (HasEnoughCoins(amount))
        {
            _currentCoins -= amount;
            OnCoinsChanged?.Invoke(_currentCoins);
            SaveCoins();
            Debug.Log($"{amount} coins spent. Remaining: {_currentCoins}");
            return true;
        }
        Debug.LogWarning("Not enough coins to spend " + amount);
        return false;
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_PREFS_KEY, _currentCoins);
        PlayerPrefs.Save(); // Good practice to call Save, though Unity does it on quit
    }

    private void LoadCoins()
    {
        _currentCoins = PlayerPrefs.GetInt(COINS_PREFS_KEY, 200); // Start with 100 coins for testing
        OnCoinsChanged?.Invoke(_currentCoins); // Notify UI on load
    }

    [ContextMenu("DEBUG: Add 100 Coins")]
    public void DebugAdd100Coins() => AddCoins(200);
    [ContextMenu("DEBUG: Reset Coins")]
    public void DebugResetCoins() { PlayerPrefs.DeleteKey(COINS_PREFS_KEY); LoadCoins(); }
}