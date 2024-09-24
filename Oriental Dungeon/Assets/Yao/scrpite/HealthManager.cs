using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private bool respawnOnDeath = true;

    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;

    private Vector3 initialPosition;
    private Animator animator;
    private PlayerController moveControl;
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;

        // 获取Animator、PlayerController、Rigidbody2D组件
        animator = GetComponent<Animator>();
        moveControl = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die(); // 触发死亡逻辑
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
        OnDeath?.Invoke(); // 触发死亡事件

        // 禁用角色移动并设置刚体速度为0
        moveControl.enabled = false;
        rb.velocity = Vector2.zero;

        // 触发死亡动画
        animator.SetTrigger("Die");

        // 在动画播放完成时调用处理死亡逻辑
        StartCoroutine(WaitForDeathAnimation());
    }

    private System.Collections.IEnumerator WaitForDeathAnimation()
    {
        // 等待当前播放的动画状态机进入 "Die" 状态，并且动画播放完成
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die") ||
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // 动画播放结束后进行处理：销毁或重生
        if (destroyOnDeath)
        {
            Destroy(gameObject); // 销毁对象
        }
        else if (respawnOnDeath)
        {
            Respawn(); // 调用重生逻辑
        }
        else
        {
            gameObject.SetActive(false); // 设置为不活跃状态
        }
    }

    private void Respawn()
    {
        if (RespawnSystem.Instance != null)
        {
            Vector3 respawnPosition = RespawnSystem.Instance.GetLastSavedPosition();
            if (respawnPosition == Vector3.zero)
            {
                Debug.LogWarning("No valid respawn point found. Using initial position.");
                respawnPosition = initialPosition; // 如果没有有效的重生点，则使用初始位置
            }
            respawnPosition.z = 0f; // 确保Z轴为0
            transform.position = respawnPosition;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log("Player respawned at: " + transform.position);
        }
        else
        {
            Debug.LogError("RespawnSystem not found. Respawning at initial position.");
            transform.position = initialPosition; // 如果没有RespawnSystem，使用初始位置重生
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        // 恢复角色的移动控制
        moveControl.enabled = true;
    }

    // 获取当前血量
    public int GetCurrentHealth() => currentHealth;

    // 获取最大血量
    public int GetMaxHealth() => maxHealth;
}
