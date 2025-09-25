using UnityEngine;
using System.Collections;

public class Enemy : EntityBase   // or : MonoBehaviour, IEntity
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private bool canAttack = false;
    public bool CanAttack => canAttack;

    private Transform target;

    public override void Move()
    {
        // Move toward target if any
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                Time.deltaTime * 2f
            );
        }
    }

    public void Attack()
    {
        if (!canAttack) return;
        // Immediate attack logic
        Debug.Log($"{name} attacks for {attackDamage} damage.");
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
    }
}
