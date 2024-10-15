using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float waitTime = 1f;
    public float totalTime = 2f;
    public Transform[] movePositions;

    private int currentPosIndex = 0;
    private float waitTimer;

    private void Start()
    {
        if (movePositions.Length == 0)
        {
            Debug.LogError("No move positions set for the platform.");
            enabled = false;
            return;
        }

        transform.position = movePositions[0].position;
        waitTimer = waitTime;
    }

    private void Update()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Transform targetPosition = movePositions[currentPosIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition.position) < 0.01f)
        {
            currentPosIndex = (currentPosIndex + 1) % movePositions.Length;
            waitTimer = waitTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        if (movePositions == null || movePositions.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < movePositions.Length; i++)
        {
            Vector3 currentPos = movePositions[i].position;
            Vector3 nextPos = movePositions[(i + 1) % movePositions.Length].position;
            Gizmos.DrawLine(currentPos, nextPos);
            Gizmos.DrawSphere(currentPos, 0.1f);
        }
    }
}