using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftableUI : MonoBehaviour
{
    [SerializeField] private Button selectedButton;
    public CraftRecipeSO craftRecipe;
    [SerializeField] private TMP_Text itemName;

    public void Initialize(CraftRecipeSO recipe)
    {
        craftRecipe = recipe;
        itemName.text = craftRecipe.ResultItem.Name;

        selectedButton.onClick.RemoveAllListeners();
        selectedButton.onClick.AddListener(() => CraftTable.Instance.SelectedThis(craftRecipe));
    }
}
