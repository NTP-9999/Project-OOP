using UnityEngine;

public class Base : MonoBehaviour, IEntity
{
    [Header("House Settings")]
    [SerializeField] private float maxHealth = 500f;
    private float currentHealth;

    public float Health
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // IEntity methods
    public void Move()
    {
        // House doesn’t move – empty implementation
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log($"🏠 House took {amount} damage! Remaining health: {Health}");

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("🏚️ The house has been destroyed! Game Over.");
        // TODO: Trigger game over screen, disable gameplay, etc.
    }

    // Optional: repair function for the player
    public void Repair(float amount)
    {
        Health += amount;
        Debug.Log($"🔧 House repaired by {amount}. Current health: {Health}");
    }
}
