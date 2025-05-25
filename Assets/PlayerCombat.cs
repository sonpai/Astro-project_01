// PlayerCombat.cs
using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int baseDamage = 4; // 2 hearts
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint; // Assign an empty GameObject in front of player

    private float _currentAttackCooldown = 0f;
    private int _currentDamageModifier = 0;
    private Coroutine _damagePotionCoroutine;

    public int CurrentAttackDamage => baseDamage + _currentDamageModifier;

    // Player movement script reference (optional, to prevent attacking while moving for example)
    // public PlayerMovement playerMovement; 

    private void Update()
    {
        if (_currentAttackCooldown > 0)
        {
            _currentAttackCooldown -= Time.deltaTime;
        }

        // Example Attack Input
        if (Input.GetButtonDown("Fire1") && _currentAttackCooldown <= 0) // Typically "Fire1" is Left Mouse Click
        {
            Attack();
            _currentAttackCooldown = attackCooldown;
        }
    }

    void Attack()
    {
        // Simple sphere cast for melee attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        Debug.Log($"Player attacking. Damage: {CurrentAttackDamage}");

        foreach (Collider enemyCollider in hitEnemies)
        {
            IDamageable damageable = enemyCollider.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsDead) // Check if the enemy is also the player itself by tag or layer if needed
            {
                if (enemyCollider.gameObject == gameObject) continue; // Don't hit self

                Debug.Log($"Player hit {enemyCollider.name} for {CurrentAttackDamage} damage.");
                damageable.TakeDamage(CurrentAttackDamage, gameObject); // Pass 'this.gameObject' as attacker
            }
        }

        // Play attack animation, sound effects etc.
        // Animator anim = GetComponent<Animator>();
        // if (anim != null) anim.SetTrigger("Attack");
    }

    public void ApplyDamagePotion(Item potionData)
    {
        if (potionData.potionEffect == PotionEffect.DamageBoost)
        {
            if (_damagePotionCoroutine != null)
            {
                StopCoroutine(_damagePotionCoroutine); // Stop existing potion effect if any
            }
            _damagePotionCoroutine = StartCoroutine(DamageBoostRoutine(potionData.effectValue, potionData.effectDuration));
        }
    }

    private IEnumerator DamageBoostRoutine(int damageIncrease, float duration)
    {
        _currentDamageModifier = damageIncrease;
        Debug.Log($"Damage Potion activated! Damage +{damageIncrease} for {duration}s. Total: {CurrentAttackDamage}");
        // Update UI if any for potion effect

        yield return new WaitForSeconds(duration);

        _currentDamageModifier = 0; // Reset modifier
        Debug.Log("Damage Potion expired. Total: " + CurrentAttackDamage);
        // Update UI
        _damagePotionCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}