using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float attackDamage;
    [HideInInspector] public Weapon owner; // 👈 เพิ่มตัวแปรไว้เก็บเจ้าของกระสุน

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(attackDamage);

            // 👇 บอก Enemy ว่าโดนยิงจากใคร
            if (owner != null)
                enemy.OnAttackedBy(owner.transform);

            Destroy(gameObject);
        }
    }
}
