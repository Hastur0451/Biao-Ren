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

    private void Start()
    {
        currentHealth = maxHealth;
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

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else if (respawnOnDeath)
        {
            Respawn();
        }
        else
        {
            // 如果既不销毁也不重生，可以在这里添加其他逻辑
            gameObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        if (RespawnSystem.Instance != null)
        {
            Vector3 respawnPosition = RespawnSystem.Instance.GetLastSavedPosition();
            respawnPosition.z = 0f; // 确保Z轴为0
            transform.position = respawnPosition;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log("Player respawned at: " + transform.position);
        }
        else
        {
            Debug.LogError("RespawnSystem not found. Cannot respawn player.");
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}