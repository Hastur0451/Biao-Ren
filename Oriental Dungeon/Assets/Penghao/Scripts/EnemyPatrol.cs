using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;

    private Vector2 startPosition;
    private Vector2 leftPatrolPoint;
    private Vector2 rightPatrolPoint;
    private bool movingRight = true;

    private void Awake()
    {
        UpdatePatrolPoints();
        Flip();
    }

    private void UpdatePatrolPoints()
    {
        startPosition = transform.position;
        leftPatrolPoint = startPosition - new Vector2(patrolDistance / 2, 0);
        rightPatrolPoint = startPosition + new Vector2(patrolDistance / 2, 0);
    }

    private void Update()
    {
        Patrol();
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

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            UpdatePatrolPoints();
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftPatrolPoint, rightPatrolPoint);
        Gizmos.DrawSphere(leftPatrolPoint, 0.2f);
        Gizmos.DrawSphere(rightPatrolPoint, 0.2f);
    }
}