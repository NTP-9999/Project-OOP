using System.Collections.Generic;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField] private ItemData data;
    public ItemData Data => data;
    [SerializeField] private List<StatsEffect> statsEffects = new List<StatsEffect>();
    public List<StatsEffect> StatsEffects => statsEffects;

    [SerializeField] private GameObject foodPrefab;
    public GameObject FoodPrefab => foodPrefab;

    public enum PlayerStats { health, fatigue, hungry, stamina }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log($"Picked up item: {data.Name}");
            Player.Instance.AddItemToInventory(data, 1);
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class StatsEffect
    {
        public PlayerStats playerstats;
        public float amount;
    }
}
