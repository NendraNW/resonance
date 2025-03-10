using UnityEngine;

public class NinjaMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 8f;
    public float rotationSpeed = 10f;

    private float currentSpeed;
    private bool isRunning;
    private bool isGrounded;
    private bool isJumping;

    public Transform groundCheck;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true; // Prevents unwanted physics rotation
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Ground Check
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        // Jump Logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        // Transition from Jumping to Idle when landing
        if (isJumping && isGrounded && !wasGrounded)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }

        // Update Animator Speed Parameter
        float movementMagnitude = rb.linearVelocity.magnitude / runSpeed;
        animator.SetFloat("Speed", movementMagnitude);
    }
}
