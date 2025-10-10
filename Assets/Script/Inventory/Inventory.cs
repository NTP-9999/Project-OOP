using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.ReorderableList;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }


    [Header("Inventory")]
    [SerializeField] private List<InventoryItem> inventory = new();
    public InventoryItem[] Inventories => inventory.ToArray();
    private UnityEvent OnInventoryChanged;
    [SerializeField] private bool canAddItem = true;
    public bool CanAddItem => canAddItem;
    private int currentSelectedIndex = 0;
    private InventoryItem CurrentSelectedItem => inventory.Count > 0 ? inventory[currentSelectedIndex] : null;

    
    [Header("Inventory UI")]
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject inventoryUI;
    private GameObject inventoryItemUI;
    private List<GameObject> spawnedSlots = new();
    private GameObject itemUI;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.gray;
    [SerializeField] private bool openning = false;
    public bool Openning => openning;


    public void Awake()
    {
        inventoryItemUI = Resources.Load<GameObject>("UI/InventoryItemUI");

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        OnInventoryChanged = new UnityEvent();
        LoadUI();
        OnInventoryChanged.AddListener(LoadUI);
    }

    private void Update()
    {
        if (Player.Instance.CurrentHoldItem is ItemSO ItemSO)
        {
            Instantiate(ItemSO.Prefab, transform.position + transform.forward + Vector3.up, Quaternion.identity);
        }

        if (Input.GetMouseButtonDown(1)) CurrentSelectedItem?.Use(Player.Instance.CurrentHoldItem);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var firstItem = inventory.FirstOrDefault();
            firstItem?.Use(firstItem.Item);
        }
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleUI();

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && inventory.Count > 0)
        {
            if (scroll > 0) currentSelectedIndex--;
            else if (scroll < 0) currentSelectedIndex++;

            if (currentSelectedIndex < 0) currentSelectedIndex = inventory.Count - 1;
            if (currentSelectedIndex >= inventory.Count) currentSelectedIndex = 0;

            UpdateSelectedItemUI();
        }
    }

    private void ToggleUI()
    {
        inventoryUI.SetActive(!openning);
    }

    private void UpdateSelectedItemUI()
    {
        HighlightSelected(currentSelectedIndex);
    }


    public void AddItemToInventory(ItemSO item, int amount)
    {
        if (!canAddItem) return;

        foreach (var inv in inventory)
        {
            if (inv.Item == item)
            {
                inv.Amount += amount;
                return;
            }
        }

        inventory.Add(new InventoryItem(item, amount));
        OnInventoryChanged?.Invoke();
    }

    public int GetItemAmount(ItemSO item)
    {
        foreach (var inv in inventory)
        {
            if (inv.Item == item)
            {
                return inv.Amount;
            }
        }
        return 0;
    }

    public void RemoveItemFromInventory(ItemSO item, int amount)
    {
        if (!canAddItem) return;

        foreach (var inv in inventory)
        {
            if (inv.Item == item)
            {
                inv.Amount -= amount;
                if (inv.Amount <= 0) inventory.Remove(inv);
                OnInventoryChanged?.Invoke();
                return;
            }
        }
    }

    public ItemSO GetHoldItem()
    {
        if(CurrentSelectedItem == null) return null;

        ItemSO item = CurrentSelectedItem.Item;
        return item;
    }

    public void LoadUI()
    {
        foreach (Transform t in itemListParent)
            Destroy(t.gameObject);

        spawnedSlots.Clear();

        foreach (var inv in inventory)
        {
            itemUI = Instantiate(inventoryItemUI, itemListParent);

            itemUI.GetComponent<Image>().sprite = inv.Item.Icon;
            itemUI.GetComponentInChildren<TMP_Text>().text = $"x{inv.Amount}";

            spawnedSlots.Add(itemUI);
        }
        HighlightSelected(inventory.Count > 0 ? 0 : -1);
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

    [System.Serializable]
    public class InventoryItem
    {
        [SerializeField] private ItemSO item;
        public ItemSO Item => item;
        [SerializeField] private int amount;
        public int Amount { get => amount; set => amount = value; }

        public InventoryItem(ItemSO item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public void Use(ItemSO item)
        {
            if (item is FoodSO food) Player.Instance.Eat(food);
            else if (item is PlaceableStructureSO) Player.Instance.PlaceThing();

            Instance.RemoveItemFromInventory(item, 1);
        }

        public void Use()
        {
            ItemSO item = Player.Instance.CurrentHoldItem;
            Use(item);
        }
        
    }
}
