using UnityEngine;
using System.Collections.Generic;

public class EnemyMovementController : MonoBehaviour
{
    public enum MovementMode
    {
        Patrol,
        Crawl
    }

    [SerializeField] private MovementMode currentMode = MovementMode.Patrol;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;

    [Header("Crawl Path Settings")]
    [SerializeField] private Vector2 crawlPathSize = new Vector2(1, 1);
    [SerializeField] private Vector2 crawlPathOffset = Vector2.zero;

    private Vector2 startPosition;
    private Vector2 leftPatrolPoint;
    private Vector2 rightPatrolPoint;
    private bool movingRight = true;
    private Rigidbody2D rb;

    // 爬行路径
    private List<Vector2> crawlPath = new List<Vector2>();
    private int currentPathIndex = 0;
    private bool crawlDirectionClockwise = true;

    private void Awake()
    {
        Flip();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnValidate()
    {
        UpdatePatrolPoints();
        if (currentMode == MovementMode.Crawl)
        {
            GenerateCrawlPath();
        }
    }

    private void UpdatePatrolPoints()
    {
        startPosition = transform.position;
        leftPatrolPoint = startPosition - new Vector2(patrolDistance / 2, 0);
        rightPatrolPoint = startPosition + new Vector2(patrolDistance / 2, 0);
    }

    private void Update()
    {
        switch (currentMode)
        {
            case MovementMode.Patrol:
                Patrol();
                break;
            case MovementMode.Crawl:
                Crawl();
                break;
        }
    }

    private void Patrol()
    {
        Vector2 targetPoint = movingRight ? rightPatrolPoint : leftPatrolPoint;
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            movingRight = !movingRight;
            Flip();
        }
    }

    private void Crawl()
    {
        if (crawlPath.Count == 0) return;

        Vector2 targetPoint = crawlPath[currentPathIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            currentPathIndex = crawlDirectionClockwise ? 
                (currentPathIndex + 1) % crawlPath.Count : 
                (currentPathIndex - 1 + crawlPath.Count) % crawlPath.Count;

            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        if (crawlPath.Count < 2) return;

        Vector2 nextPoint = crawlPath[(currentPathIndex + 1) % crawlPath.Count];
        Vector2 direction = (nextPoint - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void GenerateCrawlPath()
    {
        crawlPath.Clear();
        Vector2 centerOffset = transform.position + (Vector3)crawlPathOffset;

        // 生成矩形路径的四个角点
        crawlPath.Add(centerOffset + new Vector2(-crawlPathSize.x / 2, -crawlPathSize.y / 2));
        crawlPath.Add(centerOffset + new Vector2(-crawlPathSize.x / 2, crawlPathSize.y / 2));
        crawlPath.Add(centerOffset + new Vector2(crawlPathSize.x / 2, crawlPathSize.y / 2));
        crawlPath.Add(centerOffset + new Vector2(crawlPathSize.x / 2, -crawlPathSize.y / 2));

        // 设置初始位置和旋转
        if (crawlPath.Count > 0)
        {
            transform.position = crawlPath[0];
            UpdateRotation();
        }
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            UpdatePatrolPoints();
        }

        // 绘制巡逻路径
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftPatrolPoint, rightPatrolPoint);
        Gizmos.DrawSphere(leftPatrolPoint, 0.2f);
        Gizmos.DrawSphere(rightPatrolPoint, 0.2f);

        // 绘制爬行路径
        if (currentMode == MovementMode.Crawl)
        {
            Gizmos.color = Color.green;
            if (crawlPath.Count > 1)
            {
                for (int i = 0; i < crawlPath.Count; i++)
                {
                    Gizmos.DrawLine(crawlPath[i], crawlPath[(i + 1) % crawlPath.Count]);
                }
            }
            else
            {
                // 如果路径还未生成，显示预览
                Vector2 centerOffset = transform.position + (Vector3)crawlPathOffset;
                Vector2 topLeft = centerOffset + new Vector2(-crawlPathSize.x / 2, crawlPathSize.y / 2);
                Vector2 topRight = centerOffset + new Vector2(crawlPathSize.x / 2, crawlPathSize.y / 2);
                Vector2 bottomLeft = centerOffset + new Vector2(-crawlPathSize.x / 2, -crawlPathSize.y / 2);
                Vector2 bottomRight = centerOffset + new Vector2(crawlPathSize.x / 2, -crawlPathSize.y / 2);

                Gizmos.DrawLine(topLeft, topRight);
                Gizmos.DrawLine(topRight, bottomRight);
                Gizmos.DrawLine(bottomRight, bottomLeft);
                Gizmos.DrawLine(bottomLeft, topLeft);
            }
        }
    }

    public void SetMovementMode(MovementMode newMode)
    {
        currentMode = newMode;
        if (newMode == MovementMode.Crawl)
        {
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            GenerateCrawlPath();
        }
        else
        {
            rb.gravityScale = 1;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.rotation = Quaternion.identity;
        }
    }

    public void ToggleCrawlDirection()
    {
        crawlDirectionClockwise = !crawlDirectionClockwise;
    }
}