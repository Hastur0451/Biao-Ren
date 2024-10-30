using System.Collections;
using UnityEngine;

public class BossSwordSlash : MonoBehaviour
{
    public int damage = 1;                 // 刀光伤害值
    public float displayDuration = 0.5f;   // 刀光显示的持续时间
    public float cooldownTime = 2f;        // 刀光攻击冷却时间
    private bool isOnCooldown = false;     // 是否在冷却中

    private SpriteRenderer swordRenderer;
    private Collider2D swordCollider;

    private void Start()
    {
        swordRenderer = GetComponent<SpriteRenderer>();
        swordCollider = GetComponent<Collider2D>();

        // 初始状态：隐藏刀光并禁用碰撞体
        swordRenderer.enabled = false;
        swordCollider.enabled = false;
    }

    public void TriggerAttack()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isOnCooldown = true;

        // 显示刀光并启用碰撞体
        swordRenderer.enabled = true;
        swordCollider.enabled = true;

        yield return new WaitForSeconds(displayDuration);

        // 隐藏刀光并禁用碰撞体
        swordRenderer.enabled = false;
        swordCollider.enabled = false;

        // 等待冷却时间
        yield return new WaitForSeconds(cooldownTime);

        isOnCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否碰到玩家
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // 对玩家造成伤害
                Debug.Log("Player takes " + damage + " damage from BossSwordSlash!");
            }
        }
    }
}
