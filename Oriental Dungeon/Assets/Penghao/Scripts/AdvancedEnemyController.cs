using UnityEngine;
using System.Collections;

public class AdvancedEnemyController : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color hitColor = Color.red;

    private Transform targetPoint;
    private bool facingRight = true;
    private Transform player;
    private bool isPlayerDetected = false;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        targetPoint = pointB;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        DetectPlayer();

        if (isPlayerDetected)
        {
            ChasePlayer();
        }
        else
        {
            PatrolBetweenPoints();
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        isPlayerDetected = (playerCollider != null);
    }

    private void PatrolBetweenPoints()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
            Flip();
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        if ((direction.x > 0 && !facingRight) || (direction.x < 0 && facingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        // 可以在这里添加死亡动画或效果
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (pointA != null) Gizmos.DrawWireSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.position, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}