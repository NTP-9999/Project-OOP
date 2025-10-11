using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float attackDamage;
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(attackDamage);
            Destroy(gameObject);
        }
    }
}