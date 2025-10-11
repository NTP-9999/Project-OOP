using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : EntityBase
{
    [Header("Base Stats")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackRange = 8f;

    [Header("UI")]
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Slider healthBar;
    private float currentDamage;

    private Transform target;
    private Transform baseTarget;
    private bool chasingStructure = false;

    private Coroutine attackCoroutine;
    private bool isDead = false;
    public bool IsDead => isDead;

    private NavMeshAgent agent;
    private Animator animator;

    private float originStoppingDistance;
    private float originAttackRange;

    public void InitStats(int difficulty)
    {
        Health = MaxHealth * difficulty;
        currentDamage = baseDamage * difficulty;

        if (agent != null) agent.speed = baseSpeed + (0.2f * difficulty);
        
        agent.stoppingDistance = attackRange * 0.8f; // stop a bit before hitting range
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"❌ {name} has no NavMeshAgent component!");
        }
        animator = GetComponent<Animator>();

        originAttackRange = attackRange;
        originStoppingDistance = agent.stoppingDistance;
    }

    private void Start()
    {
        Base baseObj = FindFirstObjectByType<Base>();
        if (baseObj != null)
        {
            baseTarget = baseObj.transform;
            target = baseTarget;
        }
        else
        {
            Debug.LogWarning("Enemy: No Base found in scene.");
        }

        if (agent != null && target != null)
        {
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.SetDestination(target.position);
        }

        InitStats(GameManager.Instance.difficulty);
        Health = MaxHealth;
        attackCoroutine = StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        if (isDead) return;

        if (target != null && agent != null && agent.enabled)
        {
            // คอยอัปเดตปลายทางหากเป้าหมายเคลื่อนที่
            if (!agent.pathPending && agent.remainingDistance > attackRange)
            {
                agent.SetDestination(target.position);
            }

            float speedPercent = agent.velocity.magnitude / agent.speed; // normalize 0-1
            if (animator != null)
                animator.SetFloat("Speed", speedPercent);
        }

        healthBar.maxValue = MaxHealth;
        healthBar.value = Health;
    }

    // =============================
    // Attack System
    // =============================
    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (target != null && agent != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                if (dist <= attackRange)
                {
                    agent.isStopped = true;  // หยุดเมื่อถึงระยะโจมตี
                    Attack();
                }
                else
                {
                    agent.isStopped = false;
                }
            }

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private void Attack()
    {
        if (target == null || isDead) return;

        IEntity entity = target.GetComponent<IEntity>();
        if (entity != null)
        {
            entity.TakeDamage(currentDamage);
            animator.SetTrigger("Attack");
            Debug.Log($"{name} attacked {target.name} for {currentDamage} damage");
        }
    }

    // =============================
    // Target switching
    // =============================
    public void OnAttackedBy(Transform attacker)
    {
        if (isDead) return;

        IPlaceableStructure structure = attacker.GetComponent<IPlaceableStructure>();
        if (structure != null)
        {
            target = attacker;
            chasingStructure = true;
            Debug.Log($"{name}: switching target to structure {attacker.name}");

            if (agent != null && agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }

            agent.stoppingDistance = 1.5f;
            attackRange = 1.8f;
        }
    }

    public void OnTargetDestroyed(Transform destroyed)
    {
        if (isDead) return;

        if (chasingStructure && destroyed == target)
        {
            Move();
            agent.stoppingDistance = originStoppingDistance;
            attackRange = originAttackRange;
        }
    }

    public override void Move()
    {
        if (baseTarget != null)
        {
            target = baseTarget;
            chasingStructure = false;
            Debug.Log($"{name}: returning to base target");

            if (agent != null && agent.enabled)
                agent.SetDestination(baseTarget.position);
        }
    }

    // =============================
    // Damage & Death
    // =============================
    public override void TakeDamage(float amount)
    {
        if (isDead) return;
        base.TakeDamage(amount);
    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        if (agent != null && agent.enabled)
            agent.isStopped = true;

        Debug.Log($"{name} died.");
        Destroy(gameObject);
    }
}
