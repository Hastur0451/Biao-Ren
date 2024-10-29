using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float airControlFactor = 0.5f;

    [Header("跳跃设置")]
    public float maxJumpHeight = 4f;
    public float timeToJumpApex = 0.4f;
    public float jumpCooldown = 0.1f;

    [Header("地面检测")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundedRememberTime = 0.1f;
    public float groundCheckRadius = 0.1f;

    [Header("引用")]
    public Rigidbody2D rb;

    [Header("调试")]
    public bool showDebugGizmos = true;

    private bool isGrounded;
    private float groundedRemember;
    private bool canJump = true;
    private float jumpVelocity;
    private float gravity;
    private float lastJumpTime;
    private bool isJumping;
    private float jumpStartY;
    private bool movementEnabled = true;
    private float moveHorizontal;
    private bool isFacingRight = true;

    public int playerHealth = 100; // 玩家生命值
    public int bossDamage = 20; // Boss 对玩家造成的伤害

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        CalculateJumpParameters();

        if (groundCheck == null)
            Debug.LogError("Ground Check transform is not assigned!");
    }

    private void CalculateJumpParameters()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        rb.gravityScale = Mathf.Abs(gravity) / Physics2D.gravity.magnitude;
    }

    private void Update()
    {
        UpdateGroundedState();

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

    private void UpdateGroundedState()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            groundedRemember = groundedRememberTime;
        }
        else
        {
            groundedRemember -= Time.deltaTime;
        }

        if (isGrounded && !isJumping)
        {
            canJump = true;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
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
        if (Input.GetButtonDown("Jump") && canJump && isGrounded)
        {
            StartJump();
        }

        if (isJumping && (transform.position.y - jumpStartY >= maxJumpHeight || rb.velocity.y <= 0))
        {
            StopJump();
        }

        if (!canJump && Time.time - lastJumpTime >= jumpCooldown)
        {
            canJump = true;
        }
    }

    private void StartJump()
    {
        isJumping = true;
        canJump = false;
        lastJumpTime = Time.time;
        jumpStartY = transform.position.y;
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        groundedRemember = 0;
    }

    private void StopJump()
    {
        isJumping = false;
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
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
        if (!showDebugGizmos || groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public bool IsMoving() => Mathf.Abs(moveHorizontal) > 0.1f;
    public bool IsGrounded() => isGrounded;
    public bool IsJumping() => isJumping;
    public bool IsMovementEnabled() => movementEnabled;

    // 检测与Boss的碰撞
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                TakeDamage(bossDamage); // 当碰到Boss时，玩家受到伤害
                Debug.Log("Player hit by Boss!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // 在这里添加玩家死亡的逻辑，例如重生或结束游戏
    }
}
