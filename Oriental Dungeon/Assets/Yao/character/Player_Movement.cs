using UnityEngine;
using System.Linq;

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
    public Vector2 groundCheckSize = new Vector2(0.9f, 0.2f);
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    public LayerMask[] jumpableLayers; // 改为Layer数组

    [Header("引用")]
    public Rigidbody2D rb;

    [Header("调试")]
    public bool showDebugGizmos = true;

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

        Debug.Log($"Position: {transform.position}, IsGrounded: {isGrounded}, CanJump: {canJump}, IsJumping: {isJumping}");
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void CheckGrounded()
    {
        isGrounded = IsGroundedCheck();

        if (isGrounded)
        {
            isJumping = false;
            if (Time.time - lastJumpTime >= jumpCooldown)
            {
                canJump = true;
            }
        }
        
        Debug.Log($"Ground Check Result: {isGrounded}");
    }

    private bool IsGroundedCheck()
    {
        Vector2 boxCenter = (Vector2)transform.position + groundCheckOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, groundCheckSize, 0f);

        Debug.Log($"OverlapBoxAll found {hits.Length} colliders");

        foreach (Collider2D hit in hits)
        {
            int hitLayer = hit.gameObject.layer;
            Debug.Log($"Checking collider: {hit.name} on layer {LayerMask.LayerToName(hitLayer)}");
            
            if (IsLayerInJumpableLayers(hitLayer))
            {
                Debug.Log($"Ground detected: {hit.name} on layer {LayerMask.LayerToName(hitLayer)}");
                return true;
            }
        }

        Debug.Log("No ground detected. Jumpable layers: " + GetJumpableLayersString());
        return false;
    }

        private bool IsLayerInJumpableLayers(int layer)
    {
        return jumpableLayers.Any(layerMask => ((1 << layer) & layerMask.value) != 0);
    }

    private string GetJumpableLayersString()
    {
        return string.Join(", ", jumpableLayers
            .Select(layerMask => GetLayerMaskName(layerMask))
            .Where(name => !string.IsNullOrEmpty(name)));
    }

    private string GetLayerMaskName(LayerMask layerMask)
    {
        int layerNumber = (int)Mathf.Log(layerMask.value, 2);
        if (layerNumber >= 0 && layerNumber < 32)
        {
            return LayerMask.LayerToName(layerNumber);
        }
        return "Invalid Layer";
    }

    private void HandleMovementInput()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (moveHorizontal < 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveHorizontal > 0 && isFacingRight)
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
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump button pressed");
            if (isGrounded && canJump)
            {
                StartJump();
            }
            else
            {
                Debug.Log($"Jump failed. IsGrounded: {isGrounded}, CanJump: {canJump}");
            }
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
        Debug.Log("Jump started!");
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
        if (!showDebugGizmos) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 boxCenter = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireCube(boxCenter, groundCheckSize);

        // 绘制角色边界框
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);

        // 显示检测框的具体位置和大小
        Debug.Log($"Ground check box: Center = {boxCenter}, Size = {groundCheckSize}");
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
    public bool IsMovementEnabled() => movementEnabled;
}