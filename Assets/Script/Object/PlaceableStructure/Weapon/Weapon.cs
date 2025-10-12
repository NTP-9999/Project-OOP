using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Weapon : MonoBehaviour, IPlaceableStructure
{
    [SerializeField] private PlaceableStructureSO placeData;
    public PlaceableStructureSO PlaceData => placeData;
    [SerializeField] private RepairRecipeSO repairData;
    public RepairRecipeSO RepairData => repairData;
    [SerializeField] private float MaxHealth;
    [SerializeField] private float Health;
    [SerializeField] private bool EnemyInArea;
    [SerializeField] private bool playerInArea;
    [SerializeField] private float AttackDamage;
    [SerializeField] private float AttackCooldown;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private List<Enemy> enemiesInRange = new();
    
    // Explicit interface implementations to satisfy IPlaceableStructure
    PlaceableStructureSO IPlaceableStructure.placeData => placeData;
    RepairRecipeSO IPlaceableStructure.repairData => repairData;
    float IPlaceableStructure.maxHealth => MaxHealth;
    float IPlaceableStructure.health { get => Health; }

    private Enemy currentTarget;
    private float lastAttackTime = -Mathf.Infinity;
    private GameObject bullet;

    [SerializeField] private Slider healthBar;
    [SerializeField] private float playerCheckRadius = 10f;
    [SerializeField] private LayerMask playerLayer;

    private void Start()
    {
        Health = MaxHealth;
        bullet = Resources.Load<GameObject>("Bullet");
        if (bullet == null) Debug.LogError("Bullet prefab not found in Resources!");
    }

    private void Update()
    {
        // ลบศัตรูที่ตายหรือ null
        enemiesInRange.RemoveAll(e => e == null || e.IsDead);

        if (enemiesInRange.Count == 0)
        {
            EnemyInArea = false;
            currentTarget = null;
            return;
        }

        EnemyInArea = true;

        healthBar.maxValue = MaxHealth;
        healthBar.value = Health;

        // Lock กับศัตรูตัวแรก
        currentTarget = enemiesInRange[0];

        if (currentTarget != null)
        {
            RotateTowardsTarget(currentTarget);

            // ตรวจมุมยิง
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(-transform.forward, directionToTarget); // ใช้ -forward เพราะโมเดลหันหลัง

            if (angle < 10f && Time.time - lastAttackTime >= AttackCooldown) // 10° คือพอเหมาะ
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        playerInArea = PlayerInArea();
    }

    private bool PlayerInArea()
    {
        Collider[] player = Physics.OverlapSphere(transform.position, playerCheckRadius, playerLayer);

        return player.Length >= 1;
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
        foreach (var e in enemiesInRange)
        {
            e.OnTargetDestroyed(transform);
        }

        Destroy(gameObject);
    }

    private void Attack() => StartCoroutine(AttackIE());

    private IEnumerator AttackIE()
    {
        if (!EnemyInArea || currentTarget == null) yield break;

        // Spawn bullet 1.5 หน่วยข้างหน้าของ Ballista
        Vector3 spawnPos = transform.position - transform.forward * 1.5f; // -forward เพราะโมเดลหน้า -Z
        Vector3 realSpawn = new Vector3(spawnPos.x, spawnPos.y + 1.7f, spawnPos.z);
        Quaternion spawnRot = Quaternion.LookRotation(-transform.forward);   // ให้หันไปด้านหน้า

        GameObject bullett = Instantiate(bullet, realSpawn, spawnRot);
        bullett.GetComponent<Bullet>().attackDamage = AttackDamage;
        bullett.GetComponent<Bullet>().owner = this;

        Rigidbody rb = bullett.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = -transform.forward * bulletSpeed; // ยิงไปด้านหน้า

        Destroy(bullett, 0.7f); // กำหนด lifetime
        yield return null;
    }

    private void RotateTowardsTarget(Enemy target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-direction); // -direction ให้ Ballista หน้าไป enemy
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Add(enemy);
            EnemyInArea = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            if (Input.GetKeyDown(KeyCode.R) && playerInArea && Health != MaxHealth) Fix();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Remove(enemy);
            if (enemiesInRange.Count == 0) EnemyInArea = false;
        }
    }
}
