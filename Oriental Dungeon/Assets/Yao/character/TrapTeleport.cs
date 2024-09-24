using UnityEngine;

public class TrapAndCheckpointSystem : MonoBehaviour
{
    public LayerMask groundLayer;  // 地面层
    public LayerMask trapLayer;    // 陷阱层
    public float checkGroundRadius = 0.1f;  // 检查地面的半径
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);  // 地面检查的偏移量

    private Vector3 lastSafePosition;  // 最后的安全位置
    private bool isOnGround;  // 玩家是否在地面上

    private void Update()
    {
        CheckGround();
        CheckTrap();
    }

    private void CheckGround()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        isOnGround = Physics2D.OverlapCircle(checkPosition, checkGroundRadius, groundLayer);

        if (isOnGround)
        {
            // 更新最后的安全位置
            lastSafePosition = transform.position;
        }
    }

    private void CheckTrap()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        bool isTouchingTrap = Physics2D.OverlapCircle(checkPosition, checkGroundRadius, trapLayer);

        if (isTouchingTrap)
        {
            TeleportToSafePosition();
        }
    }

    private void TeleportToSafePosition()
    {
        // 传送到最后的安全位置
        transform.position = lastSafePosition;
        Debug.Log("Player teleported to safe position: " + lastSafePosition);

        // 这里可以添加额外的效果，比如播放音效、粒子效果等
        // 例如：AudioSource.PlayOneShot(teleportSound);
    }

    // 在 Unity 编辑器中可视化地面和陷阱检测范围
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, checkGroundRadius);
    }
}