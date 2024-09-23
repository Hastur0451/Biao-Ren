using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;  // �����ҵķ�Χ
    [SerializeField] private float movementSpeed = 2f;   // �ƶ��ٶ�
    private Transform player;  // ��Ҷ���
    private bool isPlayerInRange = false;  // ����Ƿ��ڷ�Χ��
    private HealthManager healthManager;  // ���˵Ľ�������ϵͳ

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthManager = GetComponent<HealthManager>();  // ��ȡ���˵�HealthManager���
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
        // �򵥵�׷������߼�
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        // ���ӻ���ⷶΧ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
