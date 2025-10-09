using UnityEngine;

public class Animal : MonoBehaviour, IEntity
{
    [Header("Animal Info")]
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

        if (Health <= 0) Die();
    }

    public void Die()
    {
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
