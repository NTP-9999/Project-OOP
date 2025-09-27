using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private bool playerInArea;
    [SerializeField] private bool canSleep = false;
    public bool CanSleep => canSleep;
    private List<Enemy> enemiesInArea = new List<Enemy>();

    private void Update()
    {
        if (enemiesInArea.Count > 0) canSleep = false;
        else if (enemiesInArea.Count <= 0) canSleep = true;
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
            if (playerInArea && canSleep && Input.GetKeyDown(KeyCode.E))
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
}
