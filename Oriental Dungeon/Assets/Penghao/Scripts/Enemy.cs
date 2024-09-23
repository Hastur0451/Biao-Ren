using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;  // 侦测玩家的范围
    [SerializeField] private float movementSpeed = 2f;   // 移动速度
    private Transform player;  // 玩家对象
    private bool isPlayerInRange = false;  // 玩家是否在范围内
    private HealthManager healthManager;  // 敌人的健康管理系统

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthManager = GetComponent<HealthManager>();  // 获取敌人的HealthManager组件
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer < detectionRange)
            {
                isPlayerInRange = true;
                ChasePlayer();
            }
            else
            {
                isPlayerInRange = false;
            }
        }
    }

    private void ChasePlayer()
    {
        // 简单的追踪玩家逻辑
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        // 可视化侦测范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
