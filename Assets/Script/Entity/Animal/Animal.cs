using UnityEngine;

public class Animal : MonoBehaviour, IEntity
{
    [Header("Animal Info")]
    [SerializeField] private string id = "Animal01";   // unique ID if needed
    [SerializeField] private ItemSO itemDrop;          // item it drops
    [SerializeField] private int dropAmount = 1;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
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

    // --- IEntity Methods ---
    public void Move()
    {
        // Example: wander forward slowly (placeholder)
        transform.Translate(Vector3.forward * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log($"{id} took {amount} damage. Health left: {Health}");

        if (Health <= 0) Die();
    }

    public void Die()
    {
        Debug.Log($"{id} has died.");
        AddItem();
        Destroy(gameObject);
    }

    // --- Extra Method from Diagram ---
    public void AddItem()
    {
        if (itemDrop != null)
        {
            Inventory.Instance.AddItemToInventory(itemDrop, dropAmount);
            Debug.Log($"{dropAmount}x {itemDrop.name} added to inventory.");
        }
    }
}
