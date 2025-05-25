// Boss.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // For List

public abstract class Boss : Enemy
{
    [Header("Boss Specific Info")]
    public string bossName = "Unnamed Boss";
    public int bossIndex = 0; // 0-6 for the seven bosses, for tracking progression

    // Bosses have fixed HP, not scaled by world number in this design
    // but InitializeStats will be used to set their unique HP.

    protected Coroutine currentAttackPatternCoroutine;

    public override void InitializeStats(int bossDifficultyTier) // For bosses, this could be their index or a fixed difficulty
    {
        // Each boss subclass will set its specific _currentMaxHealth here
        // For example: if (bossName == "Fire Lord") _currentMaxHealth = 500;
        // _currentHealth will be set in Enemy.Start() or here.
        // For now, derived classes must set _currentMaxHealth.
        Debug.Log($"Boss {bossName} (Tier {bossDifficultyTier}) stats being initialized by derived class.");
    }

    protected override void Start()
    {
        base.Start(); // Calls Enemy.Start() which ensures _currentHealth = _currentMaxHealth
        if (!_isDead)
        {
            StartBossFight();
        }
    }

    protected virtual void StartBossFight()
    {
        Debug.Log($"Boss fight started: {bossName}!");
        SetAIActive(true);
        // Begin the main attack pattern
        if (currentAttackPatternCoroutine == null)
        {
            currentAttackPatternCoroutine = StartCoroutine(MasterAttackPattern());
        }
    }

    // Each boss will implement its own sequence of attacks
    protected abstract IEnumerator MasterAttackPattern();

    protected override void Die(GameObject killer)
    {
        Debug.Log($"Boss {bossName} defeated!");
        if (currentAttackPatternCoroutine != null)
        {
            StopCoroutine(currentAttackPatternCoroutine);
            currentAttackPatternCoroutine = null;
        }

        // Grant player a max heart
        if (killer != null && killer.CompareTag("Player"))
        {
            PlayerHealth.Instance?.GainMaxHeart(); // Assumes PlayerHealth has a singleton instance
        }

        // Additional logic for game progression (e.g., notify GameManager)
        // GameManager.Instance.BossDefeated(bossIndex);

        base.Die(killer); // Handles exclusive drops, despawn, etc.
    }

    protected override void SetAIActive(bool isActive)
    {
        base.SetAIActive(isActive); // Disables NavMeshAgent, Animator if they exist on base.
        // Boss-specific components may need to be handled here or in derived classes.
        enabled = isActive; // Disables Update if this script has one.
    }

    // Utility for bosses to face the player
    protected IEnumerator FacePlayer(Transform playerTransform, float duration)
    {
        float timer = 0;
        while (timer < duration && playerTransform != null && !_isDead)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Adjust rotation speed
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}