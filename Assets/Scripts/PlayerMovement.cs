using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float sprintMultiplier = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Look")]
    public Transform cameraHolder;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    public Pickups pickupInRange;
    public float attackRange = 2f;
    public int attackDamage = 10;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction block;
    private InputAction dodgeAction;
    private InputAction interactAction;
    private InputAction hitAction;
    private InputAction punch;
    private InputAction kick;   

    private Animator animator;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];
        attackAction = playerInput.actions["Attack"];
        block = playerInput.actions["Block"];
        dodgeAction = playerInput.actions["Dodge"];
        interactAction = playerInput.actions["Interact"];
        hitAction = playerInput.actions["Hit"];
        punch = playerInput.actions["Punch"];
        kick = playerInput.actions["Kick"];
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        float currentSpeed = sprintAction.IsPressed() ? speed * sprintMultiplier : speed;
        Vector3 move = (transform.right * input.x) + (transform.forward * input.y);
        controller.Move(currentSpeed * Time.deltaTime * move);

        animator.SetFloat("Speed", input.magnitude * (sprintAction.IsPressed() ? sprintMultiplier : 1f));

        // jump
        if (jumpAction != null && jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping",true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Attack
        if (attackAction != null && attackAction.triggered)
        {
            animator.SetTrigger("Attack");
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);
            foreach (Collider enemy in hitEnemies)
            {
                EnemyAI ai = enemy.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.TakeDamage(attackDamage);
                }
            }

        }

        // Dodge
        if (dodgeAction != null && dodgeAction.triggered)
        {
            input = moveAction.ReadValue<Vector2>();
            animator.SetFloat("DodgeX", input.x);
            animator.SetFloat("DodgeY", input.y);
            animator.SetTrigger("Dodge");
        }

        // Hit
        if (hitAction != null && hitAction.triggered)
        {
            animator.SetTrigger("Hit");
        }

        // Punch
        if (punch != null && punch.triggered)
        {
            animator.SetTrigger("Punch");
        }

        // Kick
        if (kick != null && kick.triggered)
        {
            animator.SetTrigger("Kick");
        }

        // Block
        if (block != null && block.triggered)
        {
            animator.SetTrigger("Block");
            GetComponent<PlayerHealth>().isBlocking = true;
            Invoke(nameof(ResetBlock), 1f);
        }

        // Interact
        if (interactAction != null && interactAction.triggered && pickupInRange != null)
        {
            animator.SetTrigger("Interact");
            pickupInRange.Collect();
        }

        HandleLook();
    }

    public void ResetState()
    {
        velocity = Vector3.zero;
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ResetBlock()
    {
        GetComponent<PlayerHealth>().isBlocking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
