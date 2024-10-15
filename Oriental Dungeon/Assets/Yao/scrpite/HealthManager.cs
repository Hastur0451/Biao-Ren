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
    private Collider2D col;
    private AnimationController animationController;
    private Vector3 frozenPosition;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;

        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animationController = GetComponent<AnimationController>();
    }

    private void Update()
    {
        if (isDead)
        {
            transform.position = frozenPosition;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

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
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();

        // 冻结位置
        frozenPosition = transform.position;

        // 禁用角色控制器
        if (characterController != null)
            characterController.SetMovementEnabled(false);

        // 禁用刚体
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // 禁用碰撞体
        if (col != null)
            col.enabled = false;

        // 触发死亡动画
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
        isDead = false;

        // 重置血量
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // 获取保存的位置
        Vector3 respawnPosition = GetSavedPosition();
        respawnPosition.z = 0f;
        transform.position = respawnPosition;

        // 重新启用角色控制器
        if (characterController != null)
            characterController.SetMovementEnabled(true);

        // 重新启用刚体和碰撞体
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.zero;
        }
        if (col != null)
            col.enabled = true;

        // 触发重生动画
        if (animationController != null)
            animationController.TriggerRespawnAnimation();

        // 触发重生事件
        OnRespawn?.Invoke();

        Debug.Log("Player respawned at: " + transform.position);
    }

    private Vector3 GetSavedPosition()
    {
        float savedX = PlayerPrefs.GetFloat("SavedPosX", initialPosition.x);
        float savedY = PlayerPrefs.GetFloat("SavedPosY", initialPosition.y);
        return new Vector3(savedX, savedY, 0f);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}