using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
    [SerializeField] private float playerHealth;
    public float PlayerHealth => playerHealth;
    [SerializeField] private ulong playermoney;
    public ulong PlayerMoney
    {
        get { Debug.Log("Someone Try to Get Money"); return playermoney; }
        set { OnMoneyChanged?.Invoke(1); Debug.Log("Someone Try to Set Money"); playermoney = value;}
    }
    [SerializeField] private int maxInventorySize = 9;
    public int MaxInventorySize => maxInventorySize;
    [SerializeField] private bool canAddItem { get => playerInventory.Count < maxInventorySize; }
    public bool CanAddItem => canAddItem;

    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;
    private Vector2 _moveDirection;
    [SerializeField] private InputActionReference move;

    [SerializeField] private List<Inventory> playerInventory = new List<Inventory>();
    public List<Inventory> PlayerInventory => playerInventory;
    private int currentSelectedIndex = 0;
    private Inventory CurrentSelectedItem => playerInventory.Count > 0 ? playerInventory[currentSelectedIndex] : null;

    [HideInInspector] public UnityEvent<int> OnMoneyChanged;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InventoryUI.Instance.LoadUI();
    }

    public void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(_moveDirection.x * moveSpeed, rb.linearVelocity.y, _moveDirection.y * moveSpeed);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var firstItem = playerInventory.FirstOrDefault();
            firstItem?.Use(firstItem.Item);
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && playerInventory.Count > 0)
        {
            if (scroll > 0) currentSelectedIndex--;
            else if (scroll < 0) currentSelectedIndex++;

            if (currentSelectedIndex < 0) currentSelectedIndex = playerInventory.Count - 1;
            if (currentSelectedIndex >= playerInventory.Count) currentSelectedIndex = 0;

            UpdateSelectedItemUI();
        }
    }

    private void UpdateMoney(int amount)
    {
        Debug.Log("Money Has Changed");
    }

    private void UpdateSelectedItemUI()
    {
        Debug.Log($"Selected Item: {CurrentSelectedItem?.Item.name ?? "None"}");
        InventoryUI.Instance.HighlightSelected(currentSelectedIndex);
    }

    public void AddItemToInventory(ItemData item, ulong amount)
    {
        if (!canAddItem) return;

        foreach (var inv in playerInventory)
        {
            if (inv.Item == item)
            {
                inv.Amount += amount;
                InventoryUI.Instance.LoadUI();
                return;
            }
        }

        playerInventory.Add(new Inventory(item, amount));
        InventoryUI.Instance.LoadUI();
    }

    public void RemoveItemFromInventory(ItemData item, ulong amount)
    {
        if (!canAddItem) return;

        foreach (var inv in playerInventory)
        {
            if (inv.Item == item)
            {
                inv.Amount -= amount;
                if(inv.Amount <= 0) playerInventory.Remove(inv);
                InventoryUI.Instance.LoadUI();
                return;
            }
        }
    }

    public void IncreaseMoney(ulong amount)
    {
        playermoney += amount;
        Debug.Log($"Increased money by {amount}. Total money: {playermoney}");
    }

    public void Heal(float amount)
    {
        amount += playerHealth;
        if (playerHealth > maxHealth) playerHealth = maxHealth;
    }

    [System.Serializable]
    public class Inventory
    {
        [SerializeField] private ItemData item;
        public ItemData Item => item;
        [SerializeField] private ulong amount;
        public ulong Amount { get => amount; set => amount = value; }

        public Inventory(ItemData item, ulong amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public void Use(ItemData item)
        {
            item.Use();
        }
        
    }
}
