using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour, IEntity
{
    [Header("House Settings")]
    [SerializeField] private float maxHealth = 500f;
    private float currentHealth;

    public float Health
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthUI();
        }
    }

    [Header("UI Settings")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject healthBarUI;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // IEntity methods
    public void Move()
    {
        // House doesn’t move – empty implementation
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        UpdateHealthUI();
        Debug.Log($"🏠 House took {amount} damage! Remaining health: {Health}");

        if (Health <= 0)
        {
            Die();
            healthBarUI.SetActive(false);
        }
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
