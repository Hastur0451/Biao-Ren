using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float knockbackRecoverySpeed = 5f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color hitColor = Color.red;
    private HealthManager healthManager;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private Transform player;
    private bool isPlayerInRange = false;
    private Animator animator;
    private Animator hitAnimator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Color originalColor;
    private Vector2 knockbackForce;
    private float lastAttackTime;
    private bool isFacingRight = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        hitAnimator = transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        healthManager = GetComponent<HealthManager>();

        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        lastAttackTime = -attackCooldown;

        // Subscribe to health change events
        if (healthManager != null)
        {
            healthManager.OnHealthChanged.AddListener(OnHealthChanged);
            healthManager.OnDeath.AddListener(OnEnemyDeath);
        }
        else
        {
            Debug.LogError("HealthManager component not found on the enemy!");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            isPlayerInRange = distanceToPlayer < detectionRange;

            if (isPlayerInRange)
            {
                if (distanceToPlayer <= attackRange)
                {
                    AttackPlayer();
                }
                else
                {
                    ChasePlayer();
                }
            }
            else
            {
                // Implement idle behavior here
            }
        }

        // Apply knockback recovery
        if (knockbackForce.magnitude > 0.1f)
        {
            knockbackForce = Vector2.Lerp(knockbackForce, Vector2.zero, knockbackRecoverySpeed * Time.deltaTime);
            rb.velocity = knockbackForce;
        }
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        // Handle health changes here if needed
        Debug.Log($"Enemy health changed: {currentHealth}/{maxHealth}");
    }

    private void OnEnemyDeath()
    {
        // Handle enemy death here
        Die();
    }
    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        // Check if we need to flip the sprite
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip the enemy's local scale
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            animator?.SetTrigger("Attack");

            // Check if player is still in range before applying damage
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                // Apply damage to player
                HealthManager playerHealth = player.GetComponent<HealthManager>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log($"Enemy attacked player for {attackDamage} damage. Player health: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
                }
                else
                {
                    Debug.LogWarning("Player does not have a HealthManager component");
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (healthManager != null)
        {
            healthManager.TakeDamage(damage);
            StartCoroutine(HitFlash());
            animator?.SetTrigger("Hurt");
            hitAnimator.SetTrigger("Hit");
        }
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(hitFlashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        animator?.SetTrigger("Die");
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        // You can add death effects here
        // Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 1f);
    }

    public void Knockback(Vector2 force)
    {
        knockbackForce = force;
        rb.velocity = knockbackForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (healthManager != null)
        {
            healthManager.OnHealthChanged.RemoveListener(OnHealthChanged);
            healthManager.OnDeath.RemoveListener(OnEnemyDeath);
        }
    }
}