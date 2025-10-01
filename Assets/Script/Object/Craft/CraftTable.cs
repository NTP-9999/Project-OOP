using System;
using System.Security.Cryptography;
using UnityEngine;

public class CraftTable : MonoBehaviour
{
    [SerializeField] private Recipe currentSelectedRecipe;
    public Recipe CurrentSelectedRecipe => currentSelectedRecipe;
    [SerializeField] private bool playerInArea;
    public bool PlayerInArea => playerInArea;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            playerInArea = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            if (playerInArea && Input.GetKeyDown(KeyCode.G))
            {
                Craft();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            playerInArea = false;
        }
    }

    private void Craft()
    {
        if (!CanCraft()) return;

        foreach (var requireItem in currentSelectedRecipe.RequireItems)
        {
            Inventory.Instance.RemoveItemFromInventory(requireItem.Item, requireItem.Amount);
        }

        Inventory.Instance.AddItemToInventory(currentSelectedRecipe.ResultItem, 1);
    }

    private bool CanCraft()
    {
        foreach (var requireItem in currentSelectedRecipe.RequireItems)
        {
            if (Inventory.Instance.GetItemAmount(requireItem.Item) < requireItem.Amount)
            {
                return false;
            }
        }

        return true;
    }
}
