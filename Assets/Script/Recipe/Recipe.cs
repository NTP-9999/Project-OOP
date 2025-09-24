using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/NormalRecipe")]
public class Recipe : ScriptableObject
{
    [SerializeField] private List<RequireItem> requireItems = new List<RequireItem>();
    public RequireItem[] RequireItems => requireItems.ToArray();
    [SerializeField] private ItemData resultItem;
    public ItemData ResultItem => resultItem;


    [System.Serializable]
    public class RequireItem
    {
        private ItemData item;
        public ItemData Item => item;
        private int amount;
        public int Amount => amount;
    } 
}
