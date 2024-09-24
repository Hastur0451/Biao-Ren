using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color hitColor = Color.red;

    private int currentHealth;
    private Transform player;
    private bool isPlayerInRange = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer < detectionRange)
            {
                isPlayerInRange = true;
                ChasePlayer();
            }
            else
            {
                isPlayerInRange = false;
            }
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // 触发受伤闪烁效果
        StartCoroutine(HitFlash());

        // 可以在这里添加受伤动画
        // animator.SetTrigger("Hurt");

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
        // 可以在这里添加死亡动画
        // animator.SetTrigger("Die");

        // 禁用敌人的行为
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // 可以在这里添加一些死亡效果，比如粒子系统
        // Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 延迟销毁对象，给动画播放的时间
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}