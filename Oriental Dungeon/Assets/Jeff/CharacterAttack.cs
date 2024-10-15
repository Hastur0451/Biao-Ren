using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("Normal Attack Settings")]
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;
    public float normalAttackDuration = 0.2f;
    public float normalAttackDelay = 0.2f; // New field for normal attack delay

    [Header("Heavy Attack Settings")]
    public int heavyAttackDamage = 40;
    public float heavyAttackCooldown = 1f;
    public float heavyAttackChargeTime = 0.5f;
    public float heavyAttackDelay = 0.5f;
    public float heavyAttackDuration = 0.3f;
    public float knockbackForce = 10f;

    [Header("Audio")]
    public AudioClip normalAttackSound;
    public AudioClip heavyAttackSound;

    [Header("AttackSense Settings")]
    public float shakeTime = 0.1f;
    public int normalAttackHitPauseDuration = 3;
    public float normalAttackCameraShakeStrength = 0.05f;
    public int heavyAttackHitPauseDuration = 6;
    public float heavyAttackCameraShakeStrength = 0.1f;

    private float nextAttackTime = 0f;
    public float mouseHoldStartTime;
    private bool isChargingHeavyAttack = false;
    private bool isPerformingHeavyAttack = false;
    public event Action<bool> OnChargingStateChanged;

    private Animator animator;
    private AudioSource audioSource;
    private PolygonCollider2D hitbox;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

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

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hitbox = GetComponentInChildren<PolygonCollider2D>();

        if (hitbox == null)
        {
            Debug.LogError("PolygonCollider2D not found on child object!");
        }
        else
        {
            hitbox.enabled = false;
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
                        StartCoroutine(DelayedNormalAttack());
                    }
                    IsChargingHeavyAttack = false;
                }
            }
        }
    }

    private IEnumerator DelayedNormalAttack()
    {
        animator?.SetTrigger("Attack");
        yield return new WaitForSeconds(normalAttackDelay);
        ExecuteNormalAttack();
    }

    private void ExecuteNormalAttack()
    {
        PlaySound(normalAttackSound);
        StartCoroutine(PerformAttack(normalAttackDuration, false));
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
        PlaySound(heavyAttackSound);
        StartCoroutine(PerformAttack(heavyAttackDuration, true));
        nextAttackTime = Time.time + heavyAttackCooldown;
    }

    private IEnumerator PerformAttack(float duration, bool isHeavyAttack)
    {
        isPerformingHeavyAttack = isHeavyAttack;
        hitbox.enabled = true;
        hitEnemies.Clear(); // Clear the list of hit enemies at the start of each attack
        yield return new WaitForSeconds(duration);
        hitbox.enabled = false;
        isPerformingHeavyAttack = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other))
        {
            if (other.TryGetComponent<EnemyController>(out var enemyController))
            {
                HandleEnemyController(enemyController);
            }
            else if (other.TryGetComponent<NewEnemy>(out var newEnemy))
            {
                HandleNewEnemy(newEnemy);
            }

            hitEnemies.Add(other);
        }
    }

    private void HandleEnemyController(EnemyController enemyController)
    {
        int damage = isPerformingHeavyAttack ? heavyAttackDamage : attackDamage;
        enemyController.TakeDamage(damage);

        if (isPerformingHeavyAttack)
        {
            Vector2 knockbackDirection = (enemyController.transform.position - transform.position).normalized;
            enemyController.Knockback(knockbackDirection * knockbackForce);

            AttackSense.Instance.HitPause(heavyAttackHitPauseDuration);
            AttackSense.Instance.CameraShake(shakeTime, heavyAttackCameraShakeStrength);
        }
        else
        {
            AttackSense.Instance.HitPause(normalAttackHitPauseDuration);
            AttackSense.Instance.CameraShake(shakeTime, normalAttackCameraShakeStrength);
        }
    }

    private void HandleNewEnemy(NewEnemy newEnemy)
    {
        Vector2 hitDirection = (newEnemy.transform.position - transform.position).normalized;
        int damage = isPerformingHeavyAttack ? heavyAttackDamage : attackDamage;
        newEnemy.GetHit(hitDirection, damage);

        if (isPerformingHeavyAttack)
        {
            AttackSense.Instance.HitPause(heavyAttackHitPauseDuration);
            AttackSense.Instance.CameraShake(shakeTime, heavyAttackCameraShakeStrength);
        }
        else
        {
            AttackSense.Instance.HitPause(normalAttackHitPauseDuration);
            AttackSense.Instance.CameraShake(shakeTime, normalAttackCameraShakeStrength);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}