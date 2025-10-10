using UnityEngine;

public interface IPlaceableStructure
{
    PlaceableStructureSO placeData { get; }
    RepairRecipeSO repairData { get; }
    float Health { get; }
    float MaxHealth { get;}

    public void TakeDamage(float damage);
    public void Fix();
    public void DestroyStructure();
}
