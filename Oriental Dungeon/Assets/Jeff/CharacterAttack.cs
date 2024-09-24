using System;
using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("Normal Attack Settings")]
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;

    [Header("Heavy Attack Settings")]
    public float heavyAttackRange = 0.7f;
    public int heavyAttackDamage = 40;
    public float heavyAttackCooldown = 1f;
    public float heavyAttackChargeTime = 0.5f;
    public float heavyAttackDelay = 0.5f; // Delay before heavy attack is executed
    public float knockbackForce = 10f;

    [Header("General Settings")]
    public LayerMask enemyLayers;
    public Transform attackPoint;

    [Header("Audio")]
    public AudioClip normalAttackSound;
    public AudioClip heavyAttackSound;

    private float nextAttackTime = 0f;
    public float mouseHoldStartTime;
    private bool isChargingHeavyAttack = false;
    public event Action<bool> OnChargingStateChanged;

    public bool IsChargingHeavyAttack
    {
        get => isChargingHeavyAttack;
        private set
        {
            if (isChargingHeavyAttack != value)
            {
                isChargingHeavyAttack = value;
                OnChargingStateChanged?.Invoke(value);
            }
        }
    }
    public float ChargeProgress => IsChargingHeavyAttack ? Mathf.Clamp01((Time.time - mouseHoldStartTime) / heavyAttackChargeTime) : 0f;

    private PlayerController characterController;
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        characterController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (attackPoint == null)
        {
            Debug.LogError("Attack point is not assigned in the inspector!");
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseHoldStartTime = Time.time;
                IsChargingHeavyAttack = true;
            }
            else if (Input.GetMouseButton(0) && IsChargingHeavyAttack)
            {
                // Continue charging
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (IsChargingHeavyAttack)
                {
                    if (ChargeProgress >= 1f)
                    {
                        StartCoroutine(DelayedHeavyAttack());
                    }
                    else
                    {
                        NormalAttack();
                    }
                    IsChargingHeavyAttack = false;
                }
            }
        }
    }

    void NormalAttack()
    {
        animator.SetTrigger("Attack");
        PlaySound(normalAttackSound);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            ApplyDamage(enemy, attackDamage, 0);
        }

        nextAttackTime = Time.time + attackCooldown;
    }

    private IEnumerator DelayedHeavyAttack()
    {
        animator?.SetTrigger("HeavyAttack");
        yield return new WaitForSeconds(heavyAttackDelay);
        ExecuteHeavyAttack();
    }

    private void ExecuteHeavyAttack()
    {
        Debug.Log("Heavy Attack");
        //animator?.SetTrigger("HeavyAttack");
        PlaySound(heavyAttackSound);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, heavyAttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            ApplyDamage(enemy, heavyAttackDamage, knockbackForce);
        }

        nextAttackTime = Time.time + heavyAttackCooldown;
    }

    void ApplyDamage(Collider2D enemy, int damage, float knockback)
    {
        Debug.Log("Hit " + enemy.name);
        EnemyController enemyComponent = enemy.GetComponent<EnemyController>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage);
            if (knockback > 0)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemyComponent.Knockback(knockbackDirection * knockback);
            }
        }
        else
        {
            Debug.LogWarning("Hit object " + enemy.name + " doesn't have an Enemy component!");
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, heavyAttackRange);
    }
}