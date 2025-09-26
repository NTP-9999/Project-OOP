using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public abstract class ItemData : ScriptableObject
{
    public string Name => _name;
    [SerializeField] private string _name;
    public Sprite Icon => _icon;
    [SerializeField] public Sprite _icon;
    public ulong CoinToIncrease => _coinToIncrease;
    [SerializeField] private ulong _coinToIncrease;
}
