using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float growDuration = 3f; // 增长时间
    public float maxScale = 3f;     // 最大缩放
    public float rotationSpeed = 100f; // 旋转速度
    public float damage = 20f;      // 陨石伤害
    private bool hasDealtDamage = false; // 检查是否已经造成伤害

    private Vector3 targetPosition; // 玩家位置
    private float timer = 0f;

    public void Initialize(Vector3 targetPos)
    {
        targetPosition = targetPos;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 放大效果
        float scale = Mathf.Lerp(1f, maxScale, timer / growDuration);
        transform.localScale = new Vector3(scale, scale, 1f);

        // 旋转效果
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // 时间结束后坠落
        if (timer >= growDuration)
        {
            FallToTarget();
        }
    }

    void FallToTarget()
    {
        // 坠落动画，可以使用线性移动来表现坠落效果
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime);

        // 可以添加一个碰撞检测来判断是否击中玩家
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // 坠落到目标位置，触发伤害并销毁
            Destroy(gameObject); // 如果需要不同的效果可以在击中后销毁
        }
    }

    // 检测与玩家的碰撞
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasDealtDamage && collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                //player.TakeDamage(damage);
                hasDealtDamage = true; // 标记已造成伤害，确保不会多次伤害玩家
                Destroy(gameObject);   // 碰撞后立即销毁陨石
            }
        }
    }
}
