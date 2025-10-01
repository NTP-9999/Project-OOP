using UnityEngine;

public class Animal : EntityBase   // or : MonoBehaviour, IEntity if skipping EntityBase
{
    [SerializeField] private ItemSO itemDrop;   // Drag an Item ScriptableObject in the inspector

    public override void Move()
    {
        // Simple wander logic here
        Debug.Log($"{name} is moving.");
    }

    public void AddItem()
    {
        // Logic for adding the dropped item to player inventory, etc.
        Debug.Log($"{name} drops {itemDrop}");
    }
}
