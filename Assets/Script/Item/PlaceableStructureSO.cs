using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlacableStructureItem", menuName = "Item/PlacableStructureItem")]
public class PlaceableStructureSO : ItemSO
{
    [SerializeField] private RepairRecipeSO repairRecipe;
    public RepairRecipeSO RepairRecipe => repairRecipe;
    [SerializeField] private List<RequireItem> craftRequireItem;
    public RequireItem[] CraftRequireItem => craftRequireItem.ToArray();
    
    [System.Serializable]
    public class RequireItem
    {
        public ItemSO item;
        public int amount;
    }
}
