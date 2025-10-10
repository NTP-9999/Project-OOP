using System.Collections;
using Unity.VisualScripting;
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


    [Header("Stats Settings")]
    [SerializeField] private float secoundPerHungry = 60f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaDrainRate = 2f;
    private float hungryTimer;
    private float staminaRegenTimer = 0f;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private float jumpForce = 5f;
    public float JumpForce => jumpForce;
    [SerializeField] private bool isGrounded;
    public bool IsGrounded => isGrounded;
    [SerializeField] private bool canMove = true;
    public bool CanMove => canMove;
    [SerializeField] private bool isRunning;
    public bool IsRunning => isRunning;


    [Header("Camera Setup")]
    [SerializeField] private float rotationSpeed = 0.8f;
    [SerializeField] private float mouseSensitivity = 0.8f;
    [SerializeField] private Transform tpsCameraPivot;


    [Header("Other")]
    private GameObject bloodEffectPrefab;

    
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
        bloodEffectPrefab = Resources.Load<GameObject>("Effects/BloodFX");
    }

    public void Update()
    {
        if (canMove) HandleMovement();
        if (Input.GetMouseButtonDown(0)) StartCoroutine(Punch());
        if (Health > maxHealth) Health = maxHealth;
        if (stamina > maxStamina) stamina = maxStamina;
        if (Hungry > maxHungry) Hungry = maxHungry;
        if (fatigue > maxFatigue) fatigue = maxFatigue;

        hungryTimer += Time.deltaTime;
        if (hungryTimer >= secoundPerHungry)
        {
            hungryTimer = 0f;
            Hungry--;
            if (Hungry <= 0) Die();
        }

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

    private IEnumerator Punch()
    {
        if (Time.time - lastAttackTime < attackCooldown) yield break;

        animator.SetTrigger("Punch");
        rb.linearVelocity = Vector3.zero;
        canMove = false;

        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        float rayDistance = 1f;

        Debug.DrawRay(rayOrigin, transform.forward * rayDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, rayDistance))
        {
            IEntity entity = hit.collider.GetComponent<IEntity>() ?? hit.collider.GetComponentInParent<IEntity>();
            if (entity != null)
            {
                yield return new WaitForSeconds(0.8f);

                GameObject bloodFX = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

                entity.TakeDamage(attackDamage);
                Debug.Log($"👊 Player punched {entity} for {attackDamage} damage");

                Destroy(bloodFX, 1.5f);
            }
        }
        lastAttackTime = Time.time;
        yield return new WaitForSeconds(1f);
        canMove = true;
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
        if (!canMove) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", isRunning ? moveZ * 2f : moveZ);

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

        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && moveZ > 0)
        {
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed * 1.5f, velocity.y, moveDir.z * moveSpeed * 1.5f);
            stamina -= staminaDrainRate * Time.deltaTime;
            staminaRegenTimer = 0f;

            if (stamina < 0) stamina = 0;
            
            isRunning = true;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, velocity.y, moveDir.z * moveSpeed);

            if (stamina < maxStamina)
            {
                if (staminaRegenTimer < 0.77f)
                {
                    staminaRegenTimer += Time.deltaTime;
                }
                else
                {
                    stamina += staminaRegenRate * Time.deltaTime;
                    if (stamina > maxStamina) stamina = maxStamina;
                }
            }
            isRunning = false;
            animator.SetBool("IsRunning", false);
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
