using System.Collections.Generic;
using UnityEngine;

public class FoodItem : ItemData
{
    [SerializeField] private ItemData data;
    public ItemData Data => data;
    [SerializeField] private List<StatsEffect> statsEffects = new List<StatsEffect>();
    public List<StatsEffect> StatsEffects => statsEffects;

    [SerializeField] private GameObject foodPrefab;
    public GameObject FoodPrefab => foodPrefab;


    [System.Serializable]
    public class StatsEffect
    {
        public enum PlayerStats { health, hungry }
        public PlayerStats playerstats;
        public float amount;
    }
}