using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/RepairRecipe")]
public class RepairRecipe : Recipe
{
    [SerializeField] private float repairDuration;
    public float RepairDuration => repairDuration;
}
