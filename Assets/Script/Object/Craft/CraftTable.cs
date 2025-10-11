using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftTable : MonoBehaviour
{
    public static CraftTable Instance { get; private set; }

    [SerializeField] private CraftRecipeSO currentSelectedRecipe;
    public CraftRecipeSO CurrentSelectedRecipe => currentSelectedRecipe;
    [SerializeField] private bool playerInArea;
    public bool PlayerInArea => playerInArea;


    [Header("UI")]
    [SerializeField] private Transform craftableContent;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private bool _openning = false;
    private GameObject craftableUI;

    [Header("Descriptiom")]
    [SerializeField] private GameObject craftDescriptionUI;
    [SerializeField] private Button craftButton;
    [SerializeField] private Image resultItemImage;
    [SerializeField] private TMP_Text resultItemName;
    [SerializeField] private Transform requireItemList;
    private GameObject requireItemUI;
    
    private void Start()
    {
        craftableUI = Resources.Load<GameObject>("UI/CraftableUI");
        requireItemUI = Resources.Load<GameObject>("UI/RequireItemUI");

        craftButton.onClick.AddListener(() => Craft());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            playerInArea = true;
        }
    }

    private void Update()
    {
        if (playerInArea && Input.GetKeyDown(KeyCode.T))
        {
            _openning = !_openning;
            craftUI.SetActive(_openning);
            Player.Instance.canAttack = !_openning;
            LoadUI();
        }
    }

    public void SelectedThis(CraftRecipeSO recipe)
    {
        currentSelectedRecipe = recipe;
        craftDescriptionUI.SetActive(true);
        LoadDescription(recipe);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            playerInArea = false;
            _openning = false;
            craftDescriptionUI.SetActive(false);
            craftUI.SetActive(false);
        }
    }

    private void LoadUI()
    {
        var allRecipes = Resources.LoadAll<CraftRecipeSO>("Recipes/Craft");

        foreach (Transform t in craftableContent) Destroy(t.gameObject);

        foreach (var recipes in allRecipes)
        {
            GameObject craftableObj = Instantiate(craftableUI, craftableContent);
            var craftable = craftableObj.GetComponent<CraftableUI>();
            craftable.Initialize(recipes);
            craftable.selectedButton.onClick.RemoveAllListeners();
            craftable.selectedButton.onClick.AddListener(() => SelectedThis(recipes));
            craftableObj.GetComponent<Image>().sprite = recipes.ResultItem.Icon;
        }
    }

    private void LoadDescription(CraftRecipeSO recipe)
    {
        resultItemImage.sprite = recipe.ResultItem.Icon;
        resultItemName.text = recipe.ResultItem.Name;

        foreach (Transform t in requireItemList) Destroy(t.gameObject);

        foreach (var requireItems in recipe.RequireItems)
        {
            GameObject requireItem = Instantiate(requireItemUI, requireItemList);
            requireItem.GetComponent<Image>().sprite = requireItems.Item.Icon;
            requireItem.GetComponentInChildren<TMP_Text>().text = $"x{requireItems.Amount}";
        }
    }

    private void Craft()
    {
        if (!CanCraft()) return;

        foreach (var requireItem in currentSelectedRecipe.RequireItems)
        {
            Inventory.Instance.RemoveItemFromInventory(requireItem.Item, requireItem.Amount);
        }

        Inventory.Instance.AddItemToInventory(currentSelectedRecipe.ResultItem, 1);
    }

    private bool CanCraft()
    {
        foreach (var requireItem in currentSelectedRecipe.RequireItems)
        {
            if (Inventory.Instance.GetItemAmount(requireItem.Item) < requireItem.Amount)
            {
                return false;
            }
        }

        return true;
    }
}
