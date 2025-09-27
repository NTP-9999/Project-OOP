using UnityEngine;

public interface IPlaceableStructure
{
    PlaceableStructureItem placeData { get; }
    RepairRecipe repairData { get; }
    float Health { get; }
    float MaxHealth { get;}

    public void TakeDamage(float damage);
    public void Fix();
    public void DestroyStructure();
}
