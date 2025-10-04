using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour
{
    public static Resource Instance { get; private set; }

    [SerializeField] private float maxHealth = 3f;
    public float MaxHealth => maxHealth;
    [SerializeField] private float health = 3f;
    public float Health => health;
    [SerializeField] private float duration = 2f;
    public float Duration => duration;
    
    [SerializeField] private bool playerInRange;
    public bool PlayerInRange => playerInRange;
    
    [SerializeField] private ItemSO data;
    public ItemSO Data => data;

    private float lastHitTime = -Mathf.Infinity;
    
    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E) && Time.time - lastHitTime >= duration)
            {
                Player.Instance.Harvest(data);
                Hit();
                lastHitTime = Time.time;
            }
        }
    }
    
    private void Hit()
    {
        if(!playerInRange) return;
        
        if(playerInRange)
        {
            health--;
            if(health <= 0) Destroy();
        }
    }
    
    protected void Destroy()
    {
        Inventory.Instance.AddItemToInventory(data, Random.Range(1, 3));
        Destroy(gameObject);
    }
    
    private void OnTriggerStay(Collider other)
    {   
        if(other.TryGetComponent<Player>(out Player player))
        {
            playerInRange = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            playerInRange = false;
        }
    }
}
