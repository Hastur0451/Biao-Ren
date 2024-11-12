using UnityEngine;
using UnityEngine.Tilemaps;  // 添加 Tilemap 命名空间

public class Projectile : MonoBehaviour
{
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime); // 在指定时间后自动销毁飞镖
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查是否碰到玩家
        if (collision.CompareTag("Player"))
        {
            HealthManager playerHealth = collision.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 对玩家造成1点伤害
            }
            Destroy(gameObject); // 碰撞后立即销毁飞镖
        }
        // 检查是否碰到敌人
        else if (collision.CompareTag("Enemy") && gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<BossController>().currentHealth -= 40;
            Destroy(gameObject); // 碰撞后立即销毁飞镖
        }
        // 检查是否碰到 Tilemap
        else if (collision.GetComponent<TilemapCollider2D>() != null ||
                collision.GetComponent<CompositeCollider2D>() != null)
        {
            Destroy(gameObject); // 碰到 Tilemap 时销毁飞镖
        }
    }
}