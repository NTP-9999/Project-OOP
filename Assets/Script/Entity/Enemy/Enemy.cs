using UnityEngine;
using System.Collections;

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
    private Transform baseTarget;   // reference to Base
    private bool chasingStructure = false;

    private Coroutine attackCoroutine;

    private bool isDead = false;

    // Called externally right after instantiation
    public void InitStats(int difficulty)
    {
        Health = baseHealth * difficulty;
        currentDamage = baseDamage * difficulty;
        currentSpeed = baseSpeed + (0.2f * difficulty);
    }

    private void Awake()
    {
        // Optionally set some defaults
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

        // You may delay starting attack until in range
        attackCoroutine = StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        if (isDead) return;

        Move();
    }

    public override void Move()
    {
        if (target == null) return;

        // Rotate toward target smoothly
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        // Move
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime
        );
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);
                if (dist <= attackRange)
                {
                    Attack();
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

    public void OnAttackedBy(Transform attacker)
    {
        if (isDead) return;

        // If it’s a tower / structure
        IPlaceableStructure structure = attacker.GetComponent<IPlaceableStructure>();
        if (structure != null)
        {
            // Optionally check priority (distance, health, etc.)
            target = attacker;
            chasingStructure = true;
            Debug.Log($"{name}: switching target to structure {attacker.name}");
        }
    }

    public void OnTargetDestroyed(Transform destroyed)
    {
        if (isDead) return;

        if (chasingStructure && destroyed == target)
        {
            ReturnToBaseOrOtherTarget();
        }
    }

    private void ReturnToBaseOrOtherTarget()
    {
        if (baseTarget != null)
        {
            target = baseTarget;
            chasingStructure = false;
            Debug.Log($"{name}: returning to base target");
        }
    }

    public override void TakeDamage(float amount)
    {
        if (isDead) return;

        base.TakeDamage(amount);

        // Optionally: if attacked, attacker could call OnAttackedBy
        // But here this method itself doesn't know who attacked
    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop attack loop
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        Debug.Log($"{name} died.");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Clean up if needed
    }
}
