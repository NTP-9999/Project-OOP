using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/NormalRecipe")]
public class RecipeSO : ScriptableObject
{
    [SerializeField] private List<RequireItem> requireItems = new();
    public RequireItem[] RequireItems => requireItems.ToArray();
    [SerializeField] private ItemSO resultItem;
    public ItemSO ResultItem => resultItem;


    [System.Serializable]
    public class RequireItem
    {
        [SerializeField] private ItemSO item;
        public ItemSO Item => item;
        [SerializeField] private int amount;
        public int Amount => amount;
    } 
}