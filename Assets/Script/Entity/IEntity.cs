using UnityEngine;

public interface IEntity
{
    float Health { get; set; }

    void Move();
    void TakeDamage(float amount);
    void Die();
}
