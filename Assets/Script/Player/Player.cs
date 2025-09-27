using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }


    [Header("Player Stats")]
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth = 100f;
    [SerializeField] private float health = 100f;
    public float Health => health = 100f;
    [SerializeField] private float maxStamina = 100f;
    public float MaxStamina => maxStamina;
    private float stamina = 100f;
    public float Stamina => stamina = 100f;
    [SerializeField] private int maxHungry = 100;
    public int MaxHungry => maxHungry;
    private int hungry = 100;
    public int Hungry => hungry;
    [SerializeField] private int maxFatigue = 3;
    public int MaxFatigue => maxFatigue;
    private int fatigue = 3;
    public int Fatigue => fatigue;
    [SerializeField] private float attackDamage = 10f;
    public float AttackDamage => attackDamage;
    [SerializeField] private float attackCooldown = 1f;
    public float AttackCooldown => attackCooldown;
    private float lastAttackTime = -Mathf.Infinity;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private float jumpForce = 5f;
    public float JumpForce => jumpForce;
    [SerializeField] private bool isGrounded;
    public bool IsGrounded => isGrounded;
    [SerializeField] private bool canMove = true;
    public bool CanMove => canMove;
    [SerializeField] private bool isDead;
    public bool IsDead => isDead;


    [Header("Camera Setup")]
    private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 0.8f;

    
    [HideInInspector]
    public ItemData CurrentHoldItem
    {
        get { return Inventory.Instance.GetHoldItem(); }
    }


    private Rigidbody rb;


    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Inventory.Instance.LoadUI();
    }

    public void Update()
    {
        if (canMove) HandleMovement();
        if (Input.GetMouseButtonDown(0)) Punch();
    }

    public void Eat(FoodItem food)
    {
        foreach (var effect in food.StatsEffects)
        {
            switch (effect.playerstats)
            {
                case FoodItem.StatsEffect.PlayerStats.health:
                    health += effect.amount;
                    if (health > maxHealth) health = maxHealth;
                    break;

                case FoodItem.StatsEffect.PlayerStats.hungry:
                    hungry += (int)effect.amount;
                    if (hungry > maxHungry) hungry = maxHungry;
                    break;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (!isDead) health -= amount;
        if (health <= 0) Die();
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
                if (entity is Enemy enemy) enemy.TakeDamage(attackDamage);
                if (entity is Animal animal) animal.TakeDamage(attackDamage);
            }
        }
        lastAttackTime = Time.time;
    }


    public void Harvest(ItemData resource) => StartCoroutine(HarvestIE(resource));


    private IEnumerator HarvestIE(ItemData resource)
    {
        if (resource is not ResourceItem resourceItem) yield break;

        canMove = false;

        Inventory.Instance.AddItemToInventory(resourceItem, 1);
        yield return new WaitForSeconds(resourceItem.Duration);

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
        if (CurrentHoldItem is not PlaceableStructureItem placeable) return;

        var placeObject = Instantiate(placeable.Prefab, transform.forward, Quaternion.identity);
        Inventory.Instance.RemoveItemFromInventory(placeable, 1);

        placeObject.TryGetComponent<IPlaceableStructure>(out var structure);
    }


    public void Sleep(Bed bed)
    {
        if (!bed.CanSleep) return;
        
        fatigue++;
        if(fatigue > maxFatigue) fatigue = maxFatigue;
    }


    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;
        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(move.x, velocity.y, move.z);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
}
