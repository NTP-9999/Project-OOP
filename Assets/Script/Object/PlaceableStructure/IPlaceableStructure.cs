using UnityEngine;

public interface IPlaceableStructure
{
    PlaceableStructureSO placeData{ get; }
    public PlaceableStructureSO PlaceData => placeData;
    RepairRecipeSO repairData { get; }
    public RepairRecipeSO RepairData => repairData;
    float maxHealth { get; }
    public float MaxHealth => maxHealth;
    float health { get; }
    public float Health => health;

    public void TakeDamage(float damage);
    public void Fix();
    public void DestroyStructure();
}
