using UnityEngine;

public class BatController : MonoBehaviour
{
    public Transform[] patrolPoints; // Ѳ�ߵ�������
    public float patrolSpeed = 2f; // Ѳ���ٶ�
    public float chaseSpeed = 4f; // ׷����ҵ��ٶ�
    public float detectionRange = 5f; // �����ҵķ�Χ
    public float returnToPatrolDelay = 2f; // �����������𷵻�Ѳ�ߵ��ӳ�
    public int health = 3; // �����Ѫ��
    public int damageToPlayer = 1; // ��ײʱ�������ɵ��˺�
    public AudioClip hurtSound; // ����ʱ����Ч
    public Color hurtColor = Color.red; // ����ʱ����ɫ
    public float hurtDuration = 0.2f; // ���˺�ָ���ɫ��ʱ��
    public string playerTag = "Player"; // ����ʶ����ҵ�Tag

    private int currentPatrolIndex; // ��ǰѲ�ߵ�����
    private Transform player; // ���
    private bool isChasing; // �Ƿ���׷�����
    private bool playerInRange; // ����Ƿ��ڷ�Χ��
    private SpriteRenderer spriteRenderer; // ��������ľ�����Ⱦ
    private AudioSource audioSource; // ��Ч������
    private Color originalColor; // ����ԭ������ɫ
    private bool returningToPatrol = false; // �Ƿ��ڷ���Ѳ��

    void Start()
    {
        currentPatrolIndex = 0;
        player = GameObject.FindGameObjectWithTag(playerTag).transform; // �ҵ�����"Player"��ǩ�Ķ���
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        originalColor = spriteRenderer.color; // �洢ԭʼ��ɫ
    }

    void Update()
    {
        if (health <= 0)
        {
            Die(); // ���Ѫ��Ϊ0����������
            return;
        }

        // �������Ƿ��ڷ�Χ��
        playerInRange = PlayerInSight();

        if (playerInRange)
        {
            // �������ڷ�Χ�ڣ���ʼ׷�����
            isChasing = true;
            returningToPatrol = false;
            ChasePlayer();
        }
        else if (isChasing)
        {
            // ����ӳ���Χ���ӳٷ���Ѳ��
            isChasing = false;
            Invoke("ReturnToPatrol", returnToPatrolDelay);
        }
        else if (!returningToPatrol)
        {
            // Ѳ���߼�
            Patrol();
        }
    }

    // Ѳ���߼�
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // ����ĳ���
        FlipSprite(targetPoint.position);

        // ������𵽴���Ѳ�ߵ�
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    // ׷������߼�
    void ChasePlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

            // ����ĳ���
            FlipSprite(player.position);
        }
    }

    // ����Ѳ��
    void ReturnToPatrol()
    {
        returningToPatrol = false;
        Patrol();
    }

    // �������Ƿ��ڷ�Χ��
    bool PlayerInSight()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange); // ��ȡ��Χ�ڵ����ж���

        foreach (var hit in hits)
        {
            if (hit.CompareTag(playerTag)) // �������Ƿ����Player��ǩ
            {
                return true; // �������ң�����true
            }
        }

        return false; // û����ң�����false
    }

    // ������
    void FlipSprite(Vector3 targetPosition)
    {
        if (targetPosition.x < transform.position.x)
        {
            // ���������
            spriteRenderer.flipX = false;
        }
        else
        {
            // �������ұ�
            spriteRenderer.flipX = true;
        }
    }

    // �������������ײʱ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �����ײ�����Ƿ�Ϊ���
        if (collision.CompareTag(playerTag))
        {
            // ��ȡ��ҵ�HealthManager���
            HealthManager playerHealth = collision.GetComponent<HealthManager>();

            // ��������HealthManager���������������˺�
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer); // ��ҵ�Ѫ
            }
        }
    }

    // ��������
    void Die()
    {
        Destroy(gameObject); // �����������
    }

    // �������ܵ��˺�ʱ����
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

    // ����������Ч
    void PlayHurtSound()
    {
        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
    }

    // ����Ч������ɫ���Ȼ��ָ���
    System.Collections.IEnumerator FlashHurtEffect()
    {
        spriteRenderer.color = hurtColor; // ���
        yield return new WaitForSeconds(hurtDuration); // �ȴ�һ��ʱ��
        spriteRenderer.color = originalColor; // �ָ�ԭ������ɫ
    }

    // ���ӻ�Ѳ�ߵ�
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        // ���Ѳ�ߵ������ˣ��������ǵ�λ��
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    // ����Ѳ�ߵ�λ��
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

                    // ����Ѳ�ߵ�֮�������
                    if (i < patrolPoints.Length - 1)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
        }

        // ������Ҽ�ⷶΧ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
