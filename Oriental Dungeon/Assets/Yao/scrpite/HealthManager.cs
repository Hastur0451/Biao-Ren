using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    public UnityEvent<int, int> OnHealthChanged; // 参数：当前血量，最大血量
    public UnityEvent OnDeath;

    Animator animator;
    private movecontrol moveControl;
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        moveControl = GetComponent<movecontrol>();
        rb = GetComponent<Rigidbody2D>();
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
        //Destroy(gameObject);
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        moveControl.enabled = false;
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
