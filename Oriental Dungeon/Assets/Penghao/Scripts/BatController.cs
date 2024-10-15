using UnityEngine;

public class BatController : MonoBehaviour
{
    public Transform[] patrolPoints; // 巡逻的三个点
    public float patrolSpeed = 2f; // 巡逻速度
    public float chaseSpeed = 4f; // 追逐玩家的速度
    public float detectionRange = 5f; // 检测玩家的范围
    public float returnToPatrolDelay = 2f; // 玩家逃离后，蝙蝠返回巡逻的延迟
    public int health = 3; // 蝙蝠的血量
    public int damageToPlayer = 1; // 碰撞时对玩家造成的伤害
    public AudioClip hurtSound; // 受伤时的音效
    public Color hurtColor = Color.red; // 受伤时的颜色
    public float hurtDuration = 0.2f; // 受伤后恢复颜色的时间
    public string playerTag = "Player"; // 用于识别玩家的Tag

    private int currentPatrolIndex; // 当前巡逻点索引
    private Transform player; // 玩家
    private bool isChasing; // 是否在追逐玩家
    private bool playerInRange; // 玩家是否在范围内
    private SpriteRenderer spriteRenderer; // 控制蝙蝠的精灵渲染
    private AudioSource audioSource; // 音效播放器
    private Color originalColor; // 蝙蝠原来的颜色
    private bool returningToPatrol = false; // 是否在返回巡逻

    void Start()
    {
        currentPatrolIndex = 0;
        player = GameObject.FindGameObjectWithTag(playerTag).transform; // 找到带有"Player"标签的对象
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        originalColor = spriteRenderer.color; // 存储原始颜色
    }

    void Update()
    {
        if (health <= 0)
        {
            Die(); // 如果血量为0，蝙蝠死亡
            return;
        }

        // 检测玩家是否在范围内
        playerInRange = PlayerInSight();

        if (playerInRange)
        {
            // 如果玩家在范围内，开始追逐玩家
            isChasing = true;
            returningToPatrol = false;
            ChasePlayer();
        }
        else if (isChasing)
        {
            // 玩家逃出范围后，延迟返回巡逻
            isChasing = false;
            Invoke("ReturnToPatrol", returnToPatrolDelay);
        }
        else if (!returningToPatrol)
        {
            // 巡逻逻辑
            Patrol();
        }
    }

    // 巡逻逻辑
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // 蝙蝠的朝向
        FlipSprite(targetPoint.position);

        // 如果蝙蝠到达了巡逻点
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    // 追逐玩家逻辑
    void ChasePlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

            // 蝙蝠的朝向
            FlipSprite(player.position);
        }
    }

    // 返回巡逻
    void ReturnToPatrol()
    {
        returningToPatrol = false;
        Patrol();
    }

    // 检测玩家是否在范围内
    bool PlayerInSight()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange); // 获取范围内的所有对象

        foreach (var hit in hits)
        {
            if (hit.CompareTag(playerTag)) // 检查对象是否带有Player标签
            {
                return true; // 如果有玩家，返回true
            }
        }

        return false; // 没有玩家，返回false
    }

    // 蝙蝠朝向
    void FlipSprite(Vector3 targetPosition)
    {
        if (targetPosition.x < transform.position.x)
        {
            // 蝙蝠朝向左边
            spriteRenderer.flipX = false;
        }
        else
        {
            // 蝙蝠朝向右边
            spriteRenderer.flipX = true;
        }
    }

    // 当蝙蝠与玩家碰撞时
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否为玩家
        if (collision.CompareTag(playerTag))
        {
            // 获取玩家的HealthManager组件
            HealthManager playerHealth = collision.GetComponent<HealthManager>();

            // 如果玩家有HealthManager组件，则对玩家造成伤害
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer); // 玩家掉血
            }
        }
    }

    // 蝙蝠死亡
    void Die()
    {
        Destroy(gameObject); // 销毁蝙蝠对象
    }

    // 当蝙蝠受到伤害时调用
    public void TakeDamage(int damage)
    {
        health -= damage;
        PlayHurtSound();
        StartCoroutine(FlashHurtEffect());

        if (health <= 0)
        {
            Die();
        }
    }

    // 播放受伤音效
    void PlayHurtSound()
    {
        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
    }

    // 受伤效果（颜色变红然后恢复）
    System.Collections.IEnumerator FlashHurtEffect()
    {
        spriteRenderer.color = hurtColor; // 变红
        yield return new WaitForSeconds(hurtDuration); // 等待一段时间
        spriteRenderer.color = originalColor; // 恢复原来的颜色
    }

    // 可视化巡逻点
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        // 如果巡逻点设置了，绘制它们的位置
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    // 绘制巡逻点位置
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

                    // 绘制巡逻点之间的连线
                    if (i < patrolPoints.Length - 1)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
        }

        // 绘制玩家检测范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
