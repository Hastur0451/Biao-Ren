using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime); // 在指定时间后自动销毁飞镖
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthManager playerHealth = collision.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 对玩家造成1点伤害
            }
            Destroy(gameObject); // 碰撞后立即销毁飞镖
        } else if (collision.CompareTag("Enemy") && gameObject.layer==9)
        {
            collision.gameObject.GetComponent<BossController>().currentHealth -= 40;
            Destroy(gameObject); // 碰撞后立即销毁飞镖
        }
    }
}
