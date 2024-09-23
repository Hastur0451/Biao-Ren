using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public HealthManager healthSystem;
    public float invincibilityTime = 1f; // 受伤后的无敌时间

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isInvincible)
        {
            healthSystem.TakeDamage(amount);
            StartInvincibility();
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
    }

    public void Heal(int amount)
    {
        healthSystem.Heal(amount);
    }

    public int GetCurrentHealth()
    {
        return healthSystem.GetCurrentHealth();
    }
}
