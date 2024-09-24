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

    [Header("Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private Transform player;
    private bool isPlayerInRange = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Color originalColor;
    private Vector2 knockbackForce;
    private float lastAttackTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
        lastAttackTime = -attackCooldown;
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

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        // Flip sprite based on movement direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            //animator?.SetTrigger("Attack");

            // Apply damage to player
            HealthManager playerHealth = player.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(HitFlash());
        //animator?.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
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
        //animator?.SetTrigger("Die");
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
}