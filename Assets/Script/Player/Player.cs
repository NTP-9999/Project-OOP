using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }


    [Header("Player Stats")]
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth = 100f;
    public float Health = 100f;
    [SerializeField] private float maxStamina = 100f;
    public float MaxStamina => maxStamina;
    [SerializeField] private float stamina = 100f;
    public float Stamina => stamina = 100f;
    [SerializeField] private int maxHungry = 100;
    public int MaxHungry => maxHungry;
    public int Hungry = 100;
    [SerializeField] private int maxFatigue = 3;
    public int MaxFatigue => maxFatigue;
    [SerializeField] private int fatigue = 3;
    public int Fatigue => fatigue;
    [SerializeField] private float attackDamage = 10f;
    public float AttackDamage => attackDamage;
    [SerializeField] private float attackCooldown = 1f;
    public float AttackCooldown => attackCooldown;
    private float lastAttackTime = -Mathf.Infinity;
    [SerializeField] private bool isDead;
    public bool IsDead => isDead;


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
    [SerializeField] private float rotationSpeed = 0.8f;
    [SerializeField] private float mouseSensitivity = 0.8f;
    [SerializeField] private Transform tpsCameraPivot;

    
    [HideInInspector]
    public ItemSO CurrentHoldItem
    {
        get { return Inventory.Instance.GetHoldItem(); }
    }


    private Rigidbody rb;
    private Animator animator;


    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Inventory.Instance.LoadUI();
    }

    public void Update()
    {
        if (canMove) HandleMovement();
        if (Input.GetMouseButtonDown(0)) Punch();
        if (Health > maxHealth) Health = maxHealth;
        if (stamina > maxStamina) stamina = maxStamina;
        if (Hungry > maxHungry) Hungry = maxHungry;
        if (fatigue > maxFatigue) fatigue = maxFatigue;

        Debug.Log($"CurrentHoldItem is {CurrentHoldItem}");
        Debug.Log($"isGrounded is {isGrounded}");
    }

    public void Eat(FoodSO food)
    {
        food.Eat();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (!isDead) Health -= amount;
        if (Health <= 0) Die();
    }

    private void Die()
    {
        
    }


    private void Punch()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f))
        {
            if (hit.collider.TryGetComponent<IEntity>(out IEntity entity))
            {
                entity.TakeDamage(attackDamage);
            }
        }
        lastAttackTime = Time.time;
    }


    public void Harvest(ItemSO resource) => StartCoroutine(HarvestIE(resource));


    private IEnumerator HarvestIE(ItemSO resource)
    {
        if (resource is not ResourceSO resourceSO) yield break;

        canMove = false;

        yield return new WaitForSeconds(resourceSO.Duration);

        canMove = true;
    }


    public void Repair(Recipe recipe)
    {
        if (recipe is not RepairRecipe repairRecipe) return;

        canMove = false;

        foreach (var required in repairRecipe.RequireItems)
        {
            if (Inventory.Instance.GetItemAmount(required.Item) < required.Amount)
            {
                canMove = true;
                return;
            }
            else if (Inventory.Instance.GetItemAmount(required.Item) >= required.Amount)
            {
                Inventory.Instance.RemoveItemFromInventory(required.Item, required.Amount);
            }
        }
    }


    public void PlaceThing()
    {
        if (CurrentHoldItem is not PlaceableStructureSO placeable) return;

        var placeObject = Instantiate(placeable.Prefab, transform.forward, Quaternion.identity);
        Inventory.Instance.RemoveItemFromInventory(placeable, 1);

        placeObject.TryGetComponent<IPlaceableStructure>(out var structure);
    }


    public void Sleep(Bed bed)
    {
        fatigue++;
    }


    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveZ);

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, velocity.y, moveDir.z * moveSpeed);

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }
        animator.SetBool("IsGrounded", isGrounded);
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
}
