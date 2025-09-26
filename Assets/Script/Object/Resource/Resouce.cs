using UnityEngine;
using System.Collections;

public class Resouce : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    public float MaxHealth => maxHealth;
    [SerializeField] private float health = 3f;
    public float Health => health;
    [SerializeField] private float durations = 2f;
    public float Duration => durations;
    
    [SerializeField] private bool playerInRange;
    public bool PlayerInRange => playerInRange;
    
    [SerializeField] private ItemData data;
    public ItemData Data => data;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(playerInRange) Hit();
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
    
    private void OnTriggerStay(Collision other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            playerInRange = true;
        }
    }
    
    private void OnTriggerExit(Collision other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            playerInRange = false;
        }
    }
}
