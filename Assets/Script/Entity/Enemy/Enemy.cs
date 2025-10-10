using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : EntityBase
{
    [Header("Base Stats")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackRange = 2f;

    private float currentDamage;
    private float currentSpeed;

    private Transform target;
    private Transform baseTarget;
    private bool chasingStructure = false;

    private Coroutine attackCoroutine;
    private bool isDead = false;

    private NavMeshAgent agent;

    // =============================
    // Initialization
    // =============================
    public void InitStats(int difficulty)
    {
        Health = baseHealth * difficulty;
        currentDamage = baseDamage * difficulty;
        currentSpeed = baseSpeed + (0.2f * difficulty);

        if (agent != null)
            agent.speed = currentSpeed;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"❌ {name} has no NavMeshAgent component!");
        }
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
            agent.speed = currentSpeed;
            agent.stoppingDistance = attackRange * 0.8f; // stop a bit before hitting range
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.SetDestination(target.position);
        }

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
        }
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
                agent.SetDestination(target.position);
        }
    }

    public void OnTargetDestroyed(Transform destroyed)
    {
        if (isDead) return;

        if (chasingStructure && destroyed == target)
        {
            Move();
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

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        if (agent != null && agent.enabled)
            agent.isStopped = true;

        Debug.Log($"{name} died.");
        Destroy(gameObject);
    }
}
