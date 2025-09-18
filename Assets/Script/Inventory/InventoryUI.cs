using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [SerializeField] private Transform itemListParent;
    private GameObject inventoryItemUI;
    private List<GameObject> spawnedSlots = new List<GameObject>();
    private GameObject itemUI;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.gray;

    public void Awake()
    {
        inventoryItemUI = Resources.Load<GameObject>("UI/InventoryItemUI");

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadUI()
    {
        var player = Player.Instance;

        foreach (Transform t in itemListParent)
            Destroy(t.gameObject);

        spawnedSlots.Clear();

        foreach (var inv in player.PlayerInventory)
        {
            itemUI = Instantiate(inventoryItemUI, itemListParent);

            itemUI.GetComponent<Image>().sprite = inv.Item.Icon;
            itemUI.GetComponentInChildren<TMP_Text>().text = $"x{inv.Amount}";

            spawnedSlots.Add(itemUI);
        }
        HighlightSelected(player.PlayerInventory.Count > 0 ? 0 : -1);
    }

    public void HighlightSelected(int index)
    {
        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            var img = spawnedSlots[i].GetComponent<Image>();
            if (img == null) continue;

            img.color = (i == index) ? highlightColor : normalColor;
        }
    }
}
