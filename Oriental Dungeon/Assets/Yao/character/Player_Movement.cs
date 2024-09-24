using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float airControlFactor = 0.5f;

    [Header("跳跃设置")]
    public float maxJumpHeight = 4f;
    public float timeToJumpApex = 0.4f;
    public float jumpCooldown = 0.2f;

    [Header("地面检测")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);

    [Header("引用")]
    public Rigidbody2D rb;
    public bool IsMovementEnabled() => movementEnabled;

    private bool isGrounded;
    private bool canJump = true;
    private float jumpVelocity;
    private float gravity;
    private float lastJumpTime;
    private bool isJumping;
    private float jumpStartY;
    private bool movementEnabled = true;
    private float moveHorizontal;
    private bool isFacingRight = true;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        CalculateJumpParameters();
    }

    private void CalculateJumpParameters()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        rb.gravityScale = Mathf.Abs(gravity) / Physics2D.gravity.magnitude;
    }

    private void Update()
    {
        CheckGrounded();

        if (movementEnabled)
        {
            HandleMovementInput();
            HandleJumpInput();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void CheckGrounded()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            isJumping = false;
            if (Time.time - lastJumpTime >= jumpCooldown)
            {
                canJump = true;
            }
        }
    }

    private void HandleMovementInput()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (moveHorizontal > 0 && isFacingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && !isFacingRight)
        {
            Flip();
        }
    }

    private void ApplyMovement()
    {
        float currentMoveSpeed = isGrounded ? moveSpeed : moveSpeed * airControlFactor;
        rb.velocity = new Vector2(moveHorizontal * currentMoveSpeed, rb.velocity.y);
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            StartJump();
        }

        if (isJumping)
        {
            if (transform.position.y - jumpStartY >= maxJumpHeight || rb.velocity.y <= 0)
            {
                StopJump();
            }
        }
    }

    private void StartJump()
    {
        isJumping = true;
        canJump = false;
        lastJumpTime = Time.time;
        jumpStartY = transform.position.y;
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    private void StopJump()
    {
        isJumping = false;
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, 0));
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // Animation-related methods
    public bool IsMoving() => Mathf.Abs(moveHorizontal) > 0.1f;
    public bool IsGrounded() => isGrounded;
    public bool IsJumping() => isJumping;
}