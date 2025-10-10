using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/RepairRecipe")]
public class RepairRecipeSO : RecipeSO
{
    [SerializeField] private float repairDuration;
    public float RepairDuration => repairDuration;
}
