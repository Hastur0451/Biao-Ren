using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDistance = 4f;
    public bool moveVertically = true;
    public bool reverseInitialDirection = false;
    public float waitTime = 1f;
    public float totalTime;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool movingToEnd = true;
    private float currentWaitTime;
    private Transform playerTransform;

    private void Start()
    {
        CalculatePositions();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.parent;
        currentWaitTime = waitTime;
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

        if (reverseInitialDirection)
        {
            SwapPositions();
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (currentWaitTime > 0)
        {
            currentWaitTime -= Time.deltaTime;
            return;
        }

        Vector3 targetPosition = movingToEnd ? endPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingToEnd = !movingToEnd;
            currentWaitTime = waitTime;
        }
    }

    private void SwapPositions()
    {
        Vector3 temp = startPosition;
        startPosition = endPosition;
        endPosition = temp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            other.transform.SetParent(playerTransform);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            CalculatePositions();
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);

        // Draw spheres at the start and end positions
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 0.1f);

        // Draw arrow to indicate initial movement direction
        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 arrowPos = startPosition + direction * (moveDistance / 2);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(arrowPos, direction * 0.5f);
    }
#endif
}