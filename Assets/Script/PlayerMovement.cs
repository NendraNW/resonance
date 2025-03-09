using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb; // Use Rigidbody for 3D physics
    private Transform spriteTransform; // Reference to the 2D sprite child

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck; // Empty object at feet for ground detection
    public LayerMask groundLayer; // Assign "Ground" layer in Inspector

    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        animator = GetComponentInChildren<Animator>(); // Get animator from child
        rb = GetComponent<Rigidbody>(); // Use Rigidbody (not 2D)
        spriteTransform = transform.GetChild(0); // Get the first child (2D sprite)

        rb.freezeRotation = true; // Prevent rotation
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        // Get input for movement (A = -1, D = 1)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Move the player (using Rigidbody for smooth movement)
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);

        // Flip sprite based on movement direction
        if (moveInput != 0)
        {
            spriteTransform.localScale = new Vector3(moveInput < 0 ? -1 : 1, 1, 1);
            animator.SetBool("IsRunningBool", true);
        }
        else
        {
            animator.SetBool("IsRunningBool", false);
        }
    }

    void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
            animator.SetTrigger("JumpTrigger");
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("AttackTrigger");
        }
    }

    void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsFalling", rb.linearVelocity.y < 0 && !isGrounded);
    }
}
