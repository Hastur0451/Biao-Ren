using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.2f;
    public LayerMask GroundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveHorizontal;
    private bool isFacingRight = false;  // Since the character starts facing left
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (groundCheck == null)
        {
            Debug.LogError("Ground Check transform is not assigned in the inspector!");
        }
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, GroundLayer);

        // Get horizontal input
        moveHorizontal = Input.GetAxisRaw("Horizontal");

        // Check for jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Handle character facing direction
        if (moveHorizontal > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && isFacingRight)
        {
            Flip();
        }

        // Set the Run animation
        UpdateRunAnimation();
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement
        rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
        // Trigger jump animation
        animator.SetTrigger("Jump");
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing
        isFacingRight = !isFacingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void UpdateRunAnimation()
    {
        // Only set Run to true if the character is moving on the ground
        if (isGrounded)
        {
            animator.SetBool("Run", Mathf.Abs(moveHorizontal) > 0.1f);
        }
        else
        {
            // Ensure Run is set to false when in the air
            animator.SetBool("Run", false);
        }
    }

    // Optional: Visualize the ground check in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}