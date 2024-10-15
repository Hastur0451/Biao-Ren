using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    public float speed = 5f;
    public int maxHealth = 100;
    public float knockbackDuration = 0.5f;

    [Header("Death Effect")]
    public float flashDuration = 0.1f;
    public int numberOfFlashes = 3;
    public Color flashColor = Color.red;

    private int currentHealth;
    private Vector2 direction;
    private bool isHit;
    private bool isDead;
    private float knockbackTimer;
    private AnimatorStateInfo info;
    private Animator animator;
    private Animator hitAnimator;
    new private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        animator = GetComponent<Animator>();
        hitAnimator = transform.GetChild(0).GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        info = animator.GetCurrentAnimatorStateInfo(0);
        if (isHit)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isHit = false;
                rigidbody.velocity = Vector2.zero;
            }
        }
    }

    public void GetHit(Vector2 direction, int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(direction);
        }
    }

    private void ApplyKnockback(Vector2 direction)
    {
        transform.localScale = new Vector3(-direction.x, 1, 1);
        isHit = true;
        this.direction = direction;
        knockbackTimer = knockbackDuration;
        rigidbody.velocity = direction * speed;
        animator.SetTrigger("Hit");
        hitAnimator.SetTrigger("Hit");
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rigidbody.velocity = Vector2.zero;
        rigidbody.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(FlashAndDisappear());
    }

    private IEnumerator FlashAndDisappear()
    {
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration / 2);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration / 2);
        }

        float fadeTime = 0.5f;
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}