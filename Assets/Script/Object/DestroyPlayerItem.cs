using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "DestroyPlayer", menuName = "Item/DestroyPlayer")]
public class DestroyPlayerItem : ItemData
{
    public override void Use()
    {
        base.Use();
        Destroy(Player.Instance.gameObject);
    } 
}