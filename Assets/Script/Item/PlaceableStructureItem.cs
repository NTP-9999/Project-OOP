using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlacableStructureItem", menuName = "Item/PlacableStructureItem")]
public class PlaceableStructureSO : ItemSO
{
    [SerializeField] private List<RequireItem> requireItem;
    public RequireItem[] RequireItems => requireItem.ToArray();
    
[System.Serializable]
public class RequireItem
    {
        public ItemSO item;
        public int amount;
    }
}
