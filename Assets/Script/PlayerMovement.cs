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
    private float moveInputX;
    private float moveInputZ;
    private int facingDirection = 1; // 1 = Right, -1 = Left

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
        // Get input for movement
        moveInputX = Input.GetAxisRaw("Horizontal"); // A (-1) | D (1)
        moveInputZ = Input.GetAxisRaw("Vertical");   // W (1) | S (-1)

        // Move the player (A & D move left/right, W & S move forward/back)
        rb.linearVelocity = new Vector3(moveInputX * moveSpeed, rb.linearVelocity.y, moveInputZ * moveSpeed);

        // Separate Left/Right flipping from Forward/Backward scaling
        if (moveInputX != 0)
        {
            facingDirection = (moveInputX > 0) ? 1 : -1; // Flip for left/right movement
        }

        // Flip sprite based on left/right movement (A & D)
        spriteTransform.localScale = new Vector3(facingDirection, Mathf.Abs(spriteTransform.localScale.y), Mathf.Abs(spriteTransform.localScale.z));

        // Scale sprite for forward/backward movement (W & S)
        float scaleFactor = (moveInputZ != 0) ? 1.2f : 1.0f; // Slightly bigger when moving forward
        spriteTransform.localScale = new Vector3(spriteTransform.localScale.x, scaleFactor, scaleFactor);

        // Set running animation when moving in any direction
        if (moveInputX != 0 || moveInputZ != 0)
        {
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
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
