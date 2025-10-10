using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour
{
    public static Resource Instance { get; private set; }
    private float health = 6f;
    public float Health => health;
    
    [SerializeField] private bool playerInRange;
    public bool PlayerInRange => playerInRange;
    
    [SerializeField] private ResourceSO data;
    public ResourceSO Data => data;

    private float lastHitTime = -Mathf.Infinity;
    
    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E) && Time.time - lastHitTime >= data.HarvestDuration)
            {
                Player.Instance.Harvest(data);
                StartCoroutine(Hit());
                lastHitTime = Time.time;
            }
        }
    }
    
    private IEnumerator Hit()
    {
        if(!playerInRange) yield break;
        
        if(playerInRange)
        {
            health--;
            yield return new WaitForSeconds(data.HarvestDuration);
            Inventory.Instance.AddItemToInventory(data, 1);
            if(health <= 0) Destroy();
        }
    }
    
    private void Destroy()
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
