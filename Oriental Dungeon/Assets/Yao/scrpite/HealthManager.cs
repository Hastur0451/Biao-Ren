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

    private void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
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
            // ����Ȳ�����Ҳ���������������������������߼�
            gameObject.SetActive(false);
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
                respawnPosition = initialPosition;
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
            transform.position = initialPosition;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}