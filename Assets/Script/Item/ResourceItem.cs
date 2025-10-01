using UnityEngine;

[CreateAssetMenu(fileName = "ResourceItem", menuName = "Item/ResourceItem")]
public class ResourceSO : ItemSO
{
    [SerializeField] private float duration = 3f;
    public float Duration => duration;
}
