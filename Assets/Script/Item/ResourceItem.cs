using UnityEngine;

[CreateAssetMenu(fileName = "ResourceItem", menuName = "Item/ResourceItem")]
public class ResourceItem : ItemData
{
    [SerializeField] private float duration = 3f;
    public float Duration => duration;
}
