using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private bool respawnOnDeath = true;
    [SerializeField] private float respawnDelay = 2f; // 重生延迟时间

    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnRespawn;

    private Vector3 initialPosition;
    private CharacterController2D characterController;
    private Rigidbody2D rb;
    private AnimationController animationController;

    private void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;

        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();

        // 禁用角色控制器和刚体
        if (characterController != null)
            characterController.SetMovementEnabled(false);
        if (rb != null)
            rb.velocity = Vector2.zero;

        if (animationController != null)
            animationController.TriggerDeathAnimation();

        if (respawnOnDeath)
        {
            StartCoroutine(RespawnAfterDelay());
        }
        else if (destroyOnDeath)
        {
            Destroy(gameObject, respawnDelay); // 延迟销毁，给动画播放的时间
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        // 重置血量
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // 重置位置
        Vector3 respawnPosition;
        if (RespawnSystem.Instance != null)
        {
            respawnPosition = RespawnSystem.Instance.GetLastSavedPosition();
            if (respawnPosition == Vector3.zero)
            {
                Debug.LogWarning("No valid respawn point found. Using initial position.");
                respawnPosition = initialPosition;
            }
        }
        else
        {
            Debug.LogWarning("RespawnSystem not found. Using initial position.");
            respawnPosition = initialPosition;
        }
        respawnPosition.z = 0f;
        transform.position = respawnPosition;

        // 重新启用角色控制器
        if (characterController != null)
            characterController.SetMovementEnabled(true);

        // 触发重生动画
        if (animationController != null)
            animationController.TriggerRespawnAnimation();

        // 触发重生事件
        OnRespawn?.Invoke();

        Debug.Log("Player respawned at: " + transform.position);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}