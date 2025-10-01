using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour, IPlaceableStructure
{
    public PlaceableStructureSO placeData { get; private set; }
    public RepairRecipe repairData { get; private set; }
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public bool EnemyInArea { get; private set; }
    public bool PlayerInArea { get; private set; }
    public float AttackDamage { get; private set; }
    public float AttackCooldown { get; private set; }
    private List<Enemy> enemiesInRange = new();
    private float lastAttackTime = -Mathf.Infinity;


    private void Update()
    {
        if (EnemyInArea && Time.time - lastAttackTime >= AttackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0) DestroyStructure();
    }
    public void Fix()
    {
        Player.Instance.Repair(repairData);
        Health = MaxHealth;
    }
    public abstract void DestroyStructure();

    private void Attack() => StartCoroutine(AttackIE());
    protected IEnumerator AttackIE()
    {
        if (!EnemyInArea) yield break;

        // Implement attack logic here, e.g., deal damage to enemies in range

        yield return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Add(enemy);
            EnemyInArea = true;
        }
        else if (other.TryGetComponent<Player>(out Player player))
        {
            PlayerInArea = true;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            if (Input.GetKeyDown(KeyCode.R)) Fix();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Remove(enemy);
            if (enemiesInRange.Count == 0) EnemyInArea = false;
        }
        else if (other.TryGetComponent<Player>(out Player player))
        {
            PlayerInArea = false;
        }
    }
}
