using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

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
        HandleMovement();   
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
        
    }


    private void Harvest(ItemData resource)
    {
        
    }


    private IEnumerator HarvestIE()
    {
        yield break;
    }


    private void Repair(RepairRecipe recipe)
    {
        
    }


    private IEnumerator RepairIE()
    {
        yield break;
    }


    private void PlaceThing()
    {
        
    }


    private void Sleep()
    {
        
    }


    private void HandleMovement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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

    public void Heal(float amount)
    {
        amount += health;
        if (health > maxHealth) health = maxHealth;
    }
}