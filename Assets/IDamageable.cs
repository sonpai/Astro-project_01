using UnityEngine;
using System;

public interface IDamageable
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsDead { get; }

    void TakeDamage(int damageAmount, GameObject attacker); // Attacker can be null or source of damage
    event Action OnDeath; // Event triggered upon death
    event Action<int, int> OnHealthChanged; // CurrentHealth, MaxHealth
}
