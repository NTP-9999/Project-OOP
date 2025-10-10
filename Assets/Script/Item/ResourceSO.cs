using UnityEngine;

[CreateAssetMenu(fileName = "ResourceItem", menuName = "Item/ResourceItem")]
public class ResourceSO : ItemSO
{
    [SerializeField] private float harvestDuration = 3f;
    public float HarvestDuration => harvestDuration;
}
