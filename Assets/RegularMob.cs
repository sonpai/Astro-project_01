//// RegularMob.cs
//using UnityEngine;
//using UnityEngine.AI; // For NavMeshAgent

//[RequireComponent(typeof(NavMeshAgent))]
//public class RegularMob : Enemy
//{
//    [Header("Mob Specific Stats")]
//    [SerializeField] private float healthMultiplierPerWorld = 10f;
//    [SerializeField] private int punchDamage = 2; // 1 heart

//    [Header("Mob AI Settings")]
//    [SerializeField] private float followSpeed = 3.5f;
//    [SerializeField] private float stoppingDistance = 1.5f; // How close to get to player
//    [SerializeField] private float attackRange = 2f;
//    [SerializeField] private float attackCooldown = 2f;
//    [SerializeField] private float detectionRange = 15f; // Range to detect player

//    private NavMeshAgent agent;
//    private Transform playerTransform;
//    private float _currentAttackCooldown = 0f;
//    private Animator animator; // Optional

//    public override void InitializeStats(int worldNumber)
//    {
//        _currentMaxHealth = baseMaxHealth + (int)((worldNumber - 1) * healthMultiplierPerWorld); // worldNumber = 1 means base health
//        _currentHealth = _currentMaxHealth; // Full health on init
//       // OnHealthChanged?.Invoke(_currentHealth, _currentMaxHealth);
//        Debug.Log($"{gameObject.name} (Mob) initialized. World: {worldNumber}, MaxHP: {_currentMaxHealth}");
//    }

//    protected override void Awake()
//    {
//        base.Awake(); // Calls Enemy.Awake()
//        agent = GetComponent<NavMeshAgent>();
//        animator = GetComponent<Animator>(); // Get Animator if present
//    }

//    protected override void Start()
//    {
//        base.Start(); // Calls Enemy.Start() which will call InitializeStats if needed.

//        // Find player - can be improved with a PlayerManager or by passing reference on spawn
//        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//        if (playerObj != null)
//        {
//            playerTransform = playerObj.transform;
//        }
//        else
//        {
//            Debug.LogError($"{gameObject.name} (Mob) couldn't find Player. AI will be disabled.");
//            enabled = false; // Disable script if no player
//            SetAIActive(false);
//            return;
//        }

//        agent.speed = followSpeed;
//        agent.stoppingDistance = stoppingDistance;
//        SetAIActive(true);
//    }

//    private void Update()
//    {
//        if (_isDead || playerTransform == null || !agent.enabled) return;

//        if (_currentAttackCooldown > 0)
//        {
//            _currentAttackCooldown -= Time.deltaTime;
//        }

//        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

//        if (distanceToPlayer <= detectionRange)
//        {
//            agent.SetDestination(playerTransform.position);
//            if (animator != null) animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);


//            if (distanceToPlayer <= agent.stoppingDistance + 0.1f) // Close enough to consider attacking
//            {
//                // Face the player
//                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
//                if (directionToPlayer != Vector3.zero)
//                {
//                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
//                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
//                }

//                if (distanceToPlayer <= attackRange && _currentAttackCooldown <= 0)
//                {
//                    AttackPlayer();
//                }
//            }
//        }
//        else
//        {
//            if (animator != null) animator.SetBool("IsMoving", false);
//            // Optionally, implement patrolling or idle behavior here
//            if (agent.hasPath) agent.ResetPath();
//        }
//    }

//    private void AttackPlayer()
//    {
//        _currentAttackCooldown = attackCooldown;
//        Debug.Log($"{gameObject.name} (Mob) attacks player for {punchDamage} damage.");
//        if (animator != null) animator.SetTrigger("Attack"); // Trigger attack animation

//        // Actual damage dealing can be delayed by animation event or simple timer
//        // For simplicity, deal damage immediately after a short delay or animation event call
//        // This example is immediate. For better feel, use animation events.

//        // Check if player is still in range for the punch
//        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
//        {
//            IDamageable playerDamageable = playerTransform.GetComponent<IDamageable>();
//            if (playerDamageable != null && !playerDamageable.IsDead)
//            {
//                playerDamageable.TakeDamage(punchDamage, gameObject);
//            }
//        }
//    }

//    protected override void SetAIActive(bool isActive)
//    {
//        base.SetAIActive(isActive);
//        if (agent != null) agent.enabled = isActive;
//        if (animator != null) animator.enabled = isActive;
//        enabled = isActive; // Disables Update() for this script
//    }

//    protected override void Die(GameObject killer)
//    {
//        base.Die(killer); // Handles loot, despawn, etc.
//        // Additional mob-specific death behavior if any
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, detectionRange);
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
//    }
//}