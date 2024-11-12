using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float growDuration = 3f;           // 增长时间
    public float maxScale = 3f;               // 最大缩放
    public float rotationSpeed = 100f;        // 旋转速度
    public float fallSpeed = 10f;             // 坠落速度
    public float damage = 20f;                // 陨石伤害
    private bool hasDealtDamage = false;      // 检查是否已经造成伤害

    private Vector3 targetPosition;           // 玩家位置
    private float timer = 0f;

    public void Initialize(Vector3 bossPosition, Vector3 playerPosition)
    {
        // 设定目标位置为玩家的位置
        targetPosition = playerPosition;
        transform.position = bossPosition + new Vector3(0, 5f, 0); // 在Boss头顶生成
        transform.localScale = new Vector3(1f, 1f, 1f);            // 初始缩放为1
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 放大效果
        float scale = Mathf.Lerp(1f, maxScale, timer / growDuration);
        transform.localScale = new Vector3(scale, scale, 1f);

        // 旋转效果
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // 在增长时间结束后开始坠落
        if (timer >= growDuration)
        {
            FallToTarget();
        }
    }

    void FallToTarget()
    {
        // 以线性方式移动到玩家的位置，表现出坠落效果
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

        // 检查是否接近目标位置
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // 到达目标位置后销毁陨石
            Destroy(gameObject);
        }
    }

    // 检测与玩家的碰撞
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasDealtDamage && collision.CompareTag("Player"))
        {
            HealthManager playerHealth = collision.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // 使用 HealthManager 对玩家造成伤害
                hasDealtDamage = true;                // 标记已造成伤害，确保不会多次伤害玩家
                Destroy(gameObject);                  // 碰撞后立即销毁陨石
            }
        }
    }
}
