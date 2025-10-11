using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private bool playerInArea;
    [SerializeField] private List<Enemy> enemiesInArea = new();
    [SerializeField] private float enemyCheckRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool canSleep = false;
    public bool CanSleep => canSleep;

    private void Update()
    {
        canSleep = PlayerCanSleep();
    }

    private bool PlayerCanSleep()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, enemyCheckRadius, enemyLayer);
        return enemies.Length == 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            playerInArea = true;
        }
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInArea.Add(enemy);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            if (playerInArea && canSleep && Input.GetKeyDown(KeyCode.E) && DayNightCycle.Instance.IsNight)
            {
                player.Sleep(this);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            playerInArea = false;
        }
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInArea.Remove(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // สีเมื่อปลอดภัย (นอนได้)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyCheckRadius);

        // ถ้ามีศัตรูในระยะ (รันเกมอยู่)
        if (Application.isPlaying && !canSleep)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyCheckRadius);
        }
    }
}
