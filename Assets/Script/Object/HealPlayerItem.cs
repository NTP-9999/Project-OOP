using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "HealPlayer", menuName = "Item/HealPlayer")]
public class HealPlayerItem : ItemData
{
    [SerializeField] private float healAmont;
    public float HealAmont => healAmont;
    public override void Use()
    {
        base.Use();
        Player.Instance.Heal(healAmont);
    } 
}