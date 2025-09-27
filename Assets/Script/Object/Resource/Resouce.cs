using UnityEngine;
using System.Collections;

public class Resouce : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    public float MaxHealth => maxHealth;
    [SerializeField] private float health = 3f;
    public float Health => health;
    [SerializeField] private float duration = 2f;
    public float Duration => duration;
    
    [SerializeField] private bool playerInRange;
    public bool PlayerInRange => playerInRange;
    
    [SerializeField] private ItemData data;
    public ItemData Data => data;

    private float lastHitTime = -Mathf.Infinity;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInRange && Time.time - lastHitTime >= duration)
            {
                Player.Instance.Harvest(data);
                Hit();
                lastHitTime = Time.time;
            }
        }
    }
    
    protected void Hit()
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
