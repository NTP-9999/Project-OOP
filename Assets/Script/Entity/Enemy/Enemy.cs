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
    private bool canAttack = false;

    // Runtime scaled values
    private float currentDamage;
    private float currentSpeed;

    // Target references
    private Transform target;       // current target (Base or Tower)
    private Transform baseTarget;   // permanent reference to Base
    private bool chasingStructure = false;

    private Coroutine attackRoutine;

    // --- Initialization ---
    public void InitStats(int difficulty)
    {
        Health = baseHealth * difficulty;
        currentDamage = baseDamage * difficulty;
        currentSpeed = baseSpeed + (0.2f * difficulty);
    }

    private void Start()
    {
        // Set base as default target
        Base baseObj = FindFirstObjectByType<Base>();
        if (baseObj != null)
        {
            baseTarget = baseObj.transform;
            target = baseTarget;
        }

        attackRoutine = StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        Move();
    }

    // --- IEntity override ---
    public override void Move()
    {
        if (target == null) return;

        // Smoothly rotate toward target
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        // Move toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            Time.deltaTime * currentSpeed
        );

        // Check if in attack range
        float distance = Vector3.Distance(transform.position, target.position);
        canAttack = distance <= attackRange;
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (canAttack && target != null)
            {
                Attack();
            }
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private void Attack()
    {
        if (target == null) return;

        IEntity entity = target.GetComponent<IEntity>();
        if (entity != null)
        {
            entity.TakeDamage(currentDamage);
            Debug.Log($"{name} attacks {target.name} for {currentDamage} damage!");
        }
    }

    // --- Target Switching ---
    public void OnAttackedBy(Transform attacker)
    {
        // If tower attacks this enemy
        IPlaceableStructure structure = attacker.GetComponent<IPlaceableStructure>();
        if (structure != null && !chasingStructure)
        {
            target = attacker;
            chasingStructure = true;
            Debug.Log($"{name} switched target to {attacker.name}!");
        }
    }

    public void OnTargetDestroyed(Transform destroyedTarget)
    {
        if (chasingStructure && destroyedTarget == target)
        {
            ReturnToBase();
        }
    }

    private void ReturnToBase()
    {
        if (baseTarget != null)
        {
            target = baseTarget;
            chasingStructure = false;
            Debug.Log($"{name} returns to attack the base!");
        }
    }

    private void OnDestroy()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }
    }
}
