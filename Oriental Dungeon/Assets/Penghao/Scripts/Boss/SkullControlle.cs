using System.Collections;
using UnityEngine;

public class SkullController : MonoBehaviour
{
    public GameObject fireballPrefab;     // 火球预制体
    public Transform player;              // 玩家引用
    public float fireballSpeed = 10f;     // 火球速度
    public float fireballInterval = 0.2f; // 火球发射间隔

    void OnEnable()
    {
        StartCoroutine(FireballRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FireballRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireballInterval);

            if (player != null)
            {
                // 获取玩家当前位置一次，作为火球的目标
                Vector3 targetPosition = player.position;
                Vector3 direction = (targetPosition - transform.position).normalized;

                // 实例化火球
                GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.velocity = direction * fireballSpeed; // 火球朝向初始位置
                }

                Destroy(fireball, 3f);  // 火球在3秒后销毁
            }
        }
    }
}
