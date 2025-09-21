using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }


    [Header("Player Stats")]
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth = 100f;
    [SerializeField] private float health = 100f;
    public float Health => health = 100f;
    [SerializeField] private float stamina = 100f;
    public float Stamina => stamina = 100f;
    [SerializeField] private int hungry = 100;
    public int Hungry => hungry = 100;
    [SerializeField] private int fatigue = 3;
    public int Fatigue => fatigue = 3;
    public enum PlayerStats { health, fatigue, hungry, stamina }


    [SerializeField] private ulong playermoney;
    public ulong PlayerMoney
    {
        get { Debug.Log("Someone Try to Get Money"); return playermoney; }
        set { OnMoneyChanged?.Invoke(1); Debug.Log("Someone Try to Set Money"); playermoney = value; }
    }
    [HideInInspector] public UnityEvent<int> OnMoneyChanged;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private float jumpForce = 5f;
    public float JumpForce => jumpForce;
    [SerializeField] private bool isGrounded;
    public bool IsGrounded => isGrounded;
    [SerializeField] private bool canMove = true;
    public bool CanMove => canMove;


    [Header("Camera Setup")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;
    private float yaw;
    private float pitch;

    
    [SerializeField] private int maxInventorySize = 9;
    public int MaxInventorySize => maxInventorySize;
    [SerializeField] private bool canAddItem { get => playerInventory.Count < maxInventorySize; }
    public bool CanAddItem => canAddItem;
    [SerializeField] private List<Inventory> playerInventory = new List<Inventory>();
    public List<Inventory> PlayerInventory => playerInventory;
    private int currentSelectedIndex = 0;
    private Inventory CurrentSelectedItem => playerInventory.Count > 0 ? playerInventory[currentSelectedIndex] : null;


    private Rigidbody rb;


    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InventoryUI.Instance.LoadUI();

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = this.gameObject.transform;
            cinemachineCamera.LookAt = this.gameObject.transform;
        }
    }

    public void Update()
    {
        HandleMovement();

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

    private void HandleMovement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;
        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(move.x, velocity.y, move.z);

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            if (collision.contacts.Length > 0)
            {
                if (collision.contacts[0].normal.y > 0.5f)
                {
                    isGrounded = true;
                }
            }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = false;
    }

    public void IncreaseMoney(ulong amount)
    {
        playermoney += amount;
        Debug.Log($"Increased money by {amount}. Total money: {playermoney}");
    }

    public void Heal(float amount)
    {
        amount += health;
        if (health > maxHealth) health = maxHealth;
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
