using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDistance = 4f;
    public bool moveVertically = true;
    public bool reverseInitialDirection = false;
    public GameObject player;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool movingToEnd = true;
    private Transform playerOriginalParent;
    private bool isPlayerOn = false;
    private bool isPaused = false;

    private void Start()
    {
        CalculatePositions();
        if (player != null)
        {
            playerOriginalParent = player.transform.parent;
        }
        else
        {
            Debug.LogWarning("Player reference not set in inspector!");
        }
    }

    private void CalculatePositions()
    {
        startPosition = transform.position;
        if (moveVertically)
        {
            endPosition = startPosition + (reverseInitialDirection ? Vector3.down : Vector3.up) * moveDistance;
        }
        else
        {
            endPosition = startPosition + (reverseInitialDirection ? Vector3.right : Vector3.left) * moveDistance;
        }
    }

    private void Update()
    {
        if (!isPaused)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector3 targetPosition = movingToEnd ? endPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingToEnd = !movingToEnd;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player && !isPlayerOn)
        {
            SetPlayerParent(transform);
            isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player && isPlayerOn)
        {
            SetPlayerParent(playerOriginalParent);
            isPlayerOn = false;
        }
    }

    private void SetPlayerParent(Transform newParent)
    {
        if (player != null && player.transform.parent != newParent)
        {
            player.transform.SetParent(newParent, true);
        }
    }

    public void SetPauseState(bool pause)
    {
        isPaused = pause;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            CalculatePositions();
        }

        // 绘制移动路径
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);

        // 在起点和终点绘制球体
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 0.1f);

        // 绘制箭头指示初始移动方向
        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 arrowPos = startPosition + direction * (moveDistance / 2);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(arrowPos, direction * 0.5f);
    }
}