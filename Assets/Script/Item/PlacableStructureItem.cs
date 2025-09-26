using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlacableStructureItem", menuName = "Item/PlacableStructureItem")]
public class PlacableStructureItem : ItemData
{
    [SerializeField] private List<RequireItem> requireItem;
    public RequireItem[] RequireItems => requireItem.ToArray();
    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;
    
[System.Serializable]
public class RequireItem
    {
        public ItemData item;
        public int amount;
    }
}
