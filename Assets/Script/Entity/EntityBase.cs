using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IEntity
{
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    [SerializeField] private float health = 100f;
    public float Health { get => health; set => health = value; }

    public abstract void Move();

    public virtual void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0) Die();
    }

    public virtual void Die()
    {
        Debug.Log($"{name} died.");
        Destroy(gameObject);
    }
}
