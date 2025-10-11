using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [SerializeField] private PlaceableStructureSO placeData;
    public PlaceableStructureSO PlaceData => placeData;
    [SerializeField] private RepairRecipeSO repairData;
    [SerializeField] private float MaxHealth;
    [SerializeField] private float Health;
    [SerializeField] private bool EnemyInArea;
    [SerializeField] private bool PlayerInArea;
    [SerializeField] private float AttackDamage;
    [SerializeField] private float AttackCooldown;
    [SerializeField] private List<Enemy> enemiesInRange = new();
    private float lastAttackTime = -Mathf.Infinity;
    private GameObject bullet;

    private void Start()
    {
        Health = MaxHealth;
        bullet = Resources.Load<GameObject>("Bullet");
    }

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
    public void DestroyStructure()
    {
        Destroy(gameObject);
    }

    private void Attack() => StartCoroutine(AttackIE());
    private IEnumerator AttackIE()
    {
        if (!EnemyInArea) yield break;

        GameObject bullett = Instantiate(bullet, gameObject.transform);
        bullett.GetComponent<Bullet>().attackDamage = AttackDamage;

        Destroy(bullett, 1f);
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
