using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData data;
    public ItemData Data => data;

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log($"Picked up item: {data.Name}");
            Player.Instance.AddItemToInventory(data,1);
            Destroy(gameObject);
        }
    }
}
