using UnityEngine;

public class movecontrol : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveHorizontal;
    private bool isFacingLeft = true;
    private Animator animator;
    private bool canJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        HandleInput();
        HandleFlipping();
        UpdateAnimator();
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (isGrounded)
        {
            canJump = true;
        }
    }

    void HandleInput()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
        animator.SetTrigger("Jump");
    }

    void HandleFlipping()
    {
        if (moveHorizontal > 0 && isFacingLeft)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && !isFacingLeft)
        {
            Flip();
        }
    }

    void UpdateAnimator()
    {
        animator.SetBool("Run", Mathf.Abs(moveHorizontal) > 0.1f);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
    }

    void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        isFacingLeft = !isFacingLeft;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}