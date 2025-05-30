//// Enemy.cs
//using UnityEngine;
//using System.Collections.Generic;
//using System; // For Action

//public abstract class Enemy : MonoBehaviour, IDamageable
//{
//    [Header("Base Enemy Stats")]
//    [SerializeField] protected int baseMaxHealth = 20;
//    [SerializeField] protected int coinsOnDefeat = 10;
//    [SerializeField] protected List<LootDrop> lootTable;
//    [SerializeField] protected GameObject deathEffectPrefab; // Optional particle effect on death
//    [SerializeField] protected float despawnTimeAfterDeath = 5f; // Time before GameObject is destroyed

//    protected int _currentHealth;
//    protected int _currentMaxHealth; // Can be set by InitializeHealth
//    protected bool _isDead = false;

//    public int CurrentHealth => _currentHealth;
//    public int MaxHealth => _currentMaxHealth;
//    public bool IsDead => _isDead;

//    public event Action OnDeath;
//    public event Action<int, int> OnHealthChanged; // current, max

//    // To be implemented by derived classes for scaling or fixed values
//    public abstract void InitializeStats(int progressionValue); // e.g., worldNumber or difficulty tier

//    protected virtual void Awake()
//    {
//        // InitializeStats should be called by spawner or GameManager upon instantiation
//        // For testing, you might call it in Start with a default value.
//    }

//    protected virtual void Start()
//    {
//        // If not initialized by a spawner, provide a default initialization
//        if (_currentMaxHealth <= 0)
//        {
//            InitializeStats(1); // Default progression value
//        }
//        _currentHealth = _currentMaxHealth;
//        OnHealthChanged?.Invoke(_currentHealth, _currentMaxHealth);
//    }

//    public virtual void TakeDamage(int damageAmount, GameObject attacker)
//    {
//        if (_isDead || damageAmount <= 0) return;

//        _currentHealth -= damageAmount;
//        Debug.Log($"{gameObject.name} took {damageAmount} damage from {attacker?.name ?? "Unknown"}. Current HP: {_currentHealth}/{_currentMaxHealth}");
//        OnHealthChanged?.Invoke(_currentHealth, _currentMaxHealth);


//        if (_currentHealth <= 0)
//        {
//            _currentHealth = 0;
//            Die(attacker);
//        }
//        else
//        {
//            // Optional: Play hit animation, sound, visual feedback
//            // TriggerAggro(attacker); // If not already aggroed
//        }
//    }

//    protected virtual void Die(GameObject killer) // Killer can be player or other source
//    {
//        if (_isDead) return;
//        _isDead = true;

//        Debug.Log($"{gameObject.name} has been defeated by {killer?.name ?? "Unknown Forces"}.");
//        OnDeath?.Invoke(); // Notify any listeners (e.g., quest system, spawner)

//        // Stop AI, movement, attacks
//        SetAIActive(false);

//        // Play death animation/effects
//        if (deathEffectPrefab != null)
//        {
//            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
//        }
//        // Trigger ragdoll, fade out, etc.
//        // GetComponent<Animator>()?.SetTrigger("Die");


//        // Drop Loot
//        DropLoot();
//        // Give Coins (directly to player or drop coin objects)
//        if (killer != null && killer.CompareTag("Player")) // Example: check if killer is player
//        {
//            // This is a simplification. Usually, a CurrencyManager or PlayerStats would handle this.
//            // For now, we'll assume player has a method to add coins or log it.
//            Debug.Log($"Player earned {coinsOnDefeat} coins from {gameObject.name}.");
//            // PlayerCurrency.Instance.AddCoins(coinsOnDefeat);
//        }

//        // Despawn or pool object
//        Destroy(gameObject, despawnTimeAfterDeath);
//    }

//    protected virtual void DropLoot()
//    {
//        foreach (LootDrop drop in lootTable)
//        {
//            if (UnityEngine.Random.value <= drop.dropChance) // Random.value is 0.0 to 1.0
//            {
//                int quantity = UnityEngine.Random.Range(drop.minQuantity, drop.maxQuantity + 1);
//                if (quantity > 0 && drop.itemData != null)
//                {
//                    Debug.Log($"Dropping {quantity}x {drop.itemData.itemName} from {gameObject.name}");
//                    // Logic to instantiate item pickup GameObject or directly add to player inventory if close enough
//                    // For simplicity, we'll assume PlayerInventory.Instance is accessible globally.
//                    // This is not ideal for decoupling but works for a quick example.
//                    if (PlayerInventory.Instance != null)
//                    {
//                        PlayerInventory.Instance.AddItem(drop.itemData, quantity);
//                    }
//                    else
//                    {
//                        // Instantiate a physical representation of the item on the ground
//                        // GameObject itemPickup = Instantiate(itemPickupPrefab, transform.position + Vector3.up, Quaternion.identity);
//                        // itemPickup.GetComponent<ItemPickup>().Initialize(drop.itemData, quantity);
//                        Debug.LogWarning("PlayerInventory.Instance not found. Cannot directly add loot.");
//                    }
//                }
//            }
//        }
//    }

//    // Helper to enable/disable AI components like NavMeshAgent, Animators, colliders for attacks
//    protected virtual void SetAIActive(bool isActive)
//    {
//        // Example:
//        // if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = isActive;
//        // if (GetComponent<Animator>() != null) GetComponent<Animator>().enabled = isActive;
//        // Disable attack colliders, etc.
//    }
//}