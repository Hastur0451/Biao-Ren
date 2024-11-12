using UnityEngine;

public class Fireball : MonoBehaviour
{
    private int damage;
    private Transform target; // 当前目标
    private bool isReflected = false;

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    void Start()
    {
        // 初始目标为玩家
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target != null)
        {
            // 火球向目标移动
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * 5f * Time.deltaTime; // 假设速度为 5f，可以根据需要调整
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isReflected) // 检查是否碰到玩家，且火球未被反射
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                //player.TakeDamage(damage); // 对玩家造成伤害
            }
            Destroy(gameObject); // 碰撞后销毁火球
        }
        else if (other.CompareTag("SwordSlash") && !isReflected) // 如果火球碰到刀光且未反射过
        {
            ReflectFireball();
        }
        else if (other.CompareTag("Boss") && isReflected) // 如果火球碰到 Boss 且已反射
        {
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage); // 对 Boss 造成伤害
            }
            Destroy(gameObject); // 碰撞后销毁火球
        }
    }

    void ReflectFireball()
    {
        // 改变火球的目标为 Boss
        target = GameObject.FindGameObjectWithTag("Boss").transform;
        isReflected = true;
        Debug.Log("Fireball has been reflected!");
    }
}
