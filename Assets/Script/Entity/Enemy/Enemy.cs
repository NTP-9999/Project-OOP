using UnityEngine;
using System.Collections;

public class Enemy : EntityBase   // inherits IEntity via EntityBase
{
    [Header("Base Stats")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackRange = 2f;
    private bool canAttack = false;
    public bool CanAttack => canAttack;

    // Runtime values (scaled per wave)
    private float currentDamage;
    private float currentSpeed;

    private Transform target;

    // --- Init enemy stats each night ---
    public void InitStats(int difficulty)
    {
        Health = baseHealth * difficulty;               // from EntityBase
        currentDamage = baseDamage * difficulty;        // stronger attack
        currentSpeed = baseSpeed + (0.2f * difficulty); // slight speed increase
    }

    public override void Move()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                Time.deltaTime * currentSpeed
            );
        }
    }

    public void Attack()
    {
        if (!canAttack || target == null) return;

        // Immediate attack logic
        Debug.Log($"{name} attacks {target.name} for {currentDamage} damage.");

        // Example: if the target also implements IEntity, deal damage
        IEntity entity = target.GetComponent<IEntity>();
        if (entity != null)
        {
            entity.TakeDamage(currentDamage);
        }
    }

    public IEnumerator AttackIE()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        canAttack = true; // allow attack once target is assigned
    }
}