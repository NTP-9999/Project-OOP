using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/RepairRecipe")]
public class RepairRecipeSO : CraftRecipeSO
{
    [SerializeField] private float repairDuration;
    public float RepairDuration => repairDuration;
}
