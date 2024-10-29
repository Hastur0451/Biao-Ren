using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // 在指定时间后自动销毁飞镖
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                //player.TakeDamage(damage);
            }
            Destroy(gameObject); // 碰撞后立即销毁飞镖
        }
    }
}
