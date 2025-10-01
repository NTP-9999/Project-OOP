using System.Collections.Generic;
using UnityEngine;

public class FoodSO : ItemData
{
    [SerializeField] private ItemData data;
    public ItemData Data => data;
    [SerializeField] private List<StatsEffect> statsEffects = new();
    public List<StatsEffect> StatsEffects => statsEffects;

    public void Eat()
    {
        var player = Player.Instance;

        foreach (var effect in StatsEffects)
        {
            switch (effect.playerstats)
            {
                case StatsEffect.PlayerStats.health:
                    player.Health += effect.amount;
                    break;

                case StatsEffect.PlayerStats.hungry:
                    player.Hungry += (int)effect.amount;
                    break;
            }
        }
    }


    [System.Serializable]
    public class StatsEffect
    {
        public enum PlayerStats { health, hungry }
        public PlayerStats playerstats;
        public float amount;
    }
}