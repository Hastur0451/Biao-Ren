using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;          // 移动速度
    public float airControlFactor = 0.5f; // 空中控制系数

    [Header("跳跃设置")]
    public float initialJumpForce = 10f;  // 初始跳跃力
    public float maxJumpHeight = 4f;      // 最大跳跃高度
    public float jumpDuration = 0.5f;     // 跳跃持续时间
    public float jumpCooldown = 0.2f;     // 跳跃冷却时间

    [Header("地面检测")]
    public float groundCheckRadius = 0.1f;  // 地面检测半径
    public LayerMask groundLayer;           // 地面层
    public string upwardPlatformTag = "UpwardPlatform"; // 向上平台标签

    [Header("引用")]
    public Rigidbody2D rb;

    private bool isGrounded;
    private bool canJump = true;
    private bool isJumping;
    private float jumpStartTime;
    private float jumpStartY;
    private float lastJumpTime;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        UpdateDebugInfo();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.down * 0.5f, groundCheckRadius, groundLayer);

        if (isGrounded && Time.time - lastJumpTime >= jumpCooldown)
        {
            canJump = true;
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float currentMoveSpeed = moveSpeed;

        if (!isGrounded)
            currentMoveSpeed *= airControlFactor;

        Vector2 movement = new Vector2(moveHorizontal * currentMoveSpeed, rb.velocity.y);
        rb.velocity = movement;

        // 处理向上平台
        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius, groundLayer);
            if (hit.collider != null && hit.collider.CompareTag(upwardPlatformTag))
            {
                transform.Translate(Vector2.up * Time.deltaTime);
            }
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            StartJump();
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            ContinueJump();
        }

        if (Input.GetKeyUp(KeyCode.Space) || transform.position.y - jumpStartY >= maxJumpHeight)
        {
            StopJump();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        canJump = false;
        jumpStartY = transform.position.y;
        jumpStartTime = Time.time;
        lastJumpTime = Time.time;
        rb.velocity = new Vector2(rb.velocity.x, initialJumpForce);
    }

    private void ContinueJump()
    {
        float jumpProgress = (Time.time - jumpStartTime) / jumpDuration;
        if (jumpProgress < 1f && transform.position.y - jumpStartY < maxJumpHeight)
        {
            float jumpForce = Mathf.Lerp(initialJumpForce, 0, jumpProgress);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else
        {
            StopJump();
        }
    }

    private void StopJump()
    {
        isJumping = false;
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, 0));
    }

    private void UpdateDebugInfo()
    {
        string debugInfo = $"isGrounded: {isGrounded}, canJump: {canJump}, isJumping: {isJumping}\n";
        debugInfo += $"Position: {transform.position}, Velocity: {rb.velocity}\n";
        Debug.Log(debugInfo);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.5f, groundCheckRadius);
    }
}