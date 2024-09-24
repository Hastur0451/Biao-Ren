using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;  // A点
    [SerializeField] private Transform pointB;  // B点
    [SerializeField] private float patrolSpeed = 2f;  // 巡逻速度
    [SerializeField] private LayerMask groundLayer;   // 地面层

    private Transform targetPoint;  // 当前目标点（A或B）
    private bool facingRight = true;  // 敌人是否面朝右

    private void Start()
    {
        targetPoint = pointB;  // 初始目标是B点
    }

    private void Update()
    {
        PatrolBetweenPoints();
    }

    private void PatrolBetweenPoints()
    {
        // 敌人移动到目标点
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // 如果敌人到达目标点，切换到另一个点
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
            Flip();  // 翻转敌人的朝向
        }
    }

    private void Flip()
    {
        // 反转敌人朝向
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;  // 翻转X轴
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        // 可视化A点和B点
        Gizmos.color = Color.blue;
        if (pointA != null) Gizmos.DrawWireSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.position, 0.2f);
    }
}
