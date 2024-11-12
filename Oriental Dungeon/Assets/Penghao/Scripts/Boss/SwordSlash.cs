using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    public int damage = 10; // 默认伤害值

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否碰到 Boss
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage); // 使用玩家的攻击力对 Boss 造成伤害
            }
        }

        // 检查是否碰到 Boss 的远程攻击物品（Weapon 或 Meteor）
        if (other.CompareTag("Weapon") || other.CompareTag("Meteor"))
        {
            // 随机生成一个0到1之间的浮点数
            float randomValue = Random.value;

            if (randomValue <= 0.50f) // 50% 的概率摧毁远程物品
            {
                Destroy(other.gameObject);
                Debug.Log("Detected Weapon or Meteor!");
            }
            else // 50% 的概率反弹回去
            {
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                other.gameObject.layer = 9;
                if (rb != null)
                {
                    // 将物品的速度方向反转，使其反弹回去
                    rb.velocity = -rb.velocity;
                }
            }
        }
    }
}
