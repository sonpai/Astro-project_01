// PlayerHealth.cs
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int initialMaxHearts = 3;
    [SerializeField] private int maxPossibleHearts = 10;
    private int _currentMaxHearts;

    private int _currentHP;
    private const int HP_PER_HEART = 2;

    public int CurrentHealth => _currentHP;
    public int MaxHealth => _currentMaxHearts * HP_PER_HEART; // Max HP based on current max hearts
    public int CurrentMaxHearts => _currentMaxHearts;
    public bool IsDead { get; private set; }

    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged; // currentHP, MaxHealth
    public event Action<int> OnMaxHeartsChanged;   // newMaxHearts

    // Singleton for easy access, or use a proper dependency injection / service locator
    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // If player persists across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _currentMaxHearts = initialMaxHearts;
        _currentHP = MaxHealth; // Start with full health
        IsDead = false;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(_currentHP, MaxHealth);
        OnMaxHeartsChanged?.Invoke(_currentMaxHearts);
        // Load player health data if implementing save/load
    }

    public void TakeDamage(int damageAmount, GameObject attacker)
    {
        if (IsDead || damageAmount <= 0) return;

        _currentHP -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage from {attacker?.name ?? "Unknown"}. Current HP: {_currentHP}/{MaxHealth}");

        if (_currentHP <= 0)
        {
            _currentHP = 0;
            Die();
        }
        OnHealthChanged?.Invoke(_currentHP, MaxHealth);
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;
        _currentHP = Mathf.Min(_currentHP + amount, MaxHealth);
        OnHealthChanged?.Invoke(_currentHP, MaxHealth);
        Debug.Log($"{gameObject.name} healed {amount}. Current HP: {_currentHP}/{MaxHealth}");
    }


    public void GainMaxHeart()
    {
        if (_currentMaxHearts < maxPossibleHearts)
        {
            _currentMaxHearts++;
            // Optionally fully heal or add 2 HP when gaining a heart
            _currentHP = Mathf.Min(_currentHP + HP_PER_HEART, MaxHealth);

            OnMaxHeartsChanged?.Invoke(_currentMaxHearts);
            OnHealthChanged?.Invoke(_currentHP, MaxHealth); // Update health display due to MaxHealth change
            Debug.Log($"{gameObject.name} gained a max heart! Total hearts: {_currentMaxHearts}. Max HP: {MaxHealth}");
            // Save player health data
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"{gameObject.name} has died.");
        OnDeath?.Invoke();
        // Handle player death (e.g., game over screen, respawn logic)
        // For example:
        // GameManager.Instance.HandlePlayerDeath();
    }

    // For debug or specific mechanics
    public void SetMaxHearts(int hearts)
    {
        _currentMaxHearts = Mathf.Clamp(hearts, 1, maxPossibleHearts);
        _currentHP = Mathf.Min(_currentHP, MaxHealth); // Adjust current HP if it exceeds new max
        OnMaxHeartsChanged?.Invoke(_currentMaxHearts);
        OnHealthChanged?.Invoke(_currentHP, MaxHealth);
    }
}