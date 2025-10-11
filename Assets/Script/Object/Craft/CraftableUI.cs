using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftableUI : MonoBehaviour
{
    public Button selectedButton;
    [SerializeField] private TMP_Text itemName;

    public void Initialize(CraftRecipeSO recipe)
    {
        itemName.text = recipe.ResultItem.Name;
    }
}
