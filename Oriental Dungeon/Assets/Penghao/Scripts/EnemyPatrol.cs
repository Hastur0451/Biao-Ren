using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;  // A��
    [SerializeField] private Transform pointB;  // B��
    [SerializeField] private float patrolSpeed = 2f;  // Ѳ���ٶ�
    [SerializeField] private LayerMask groundLayer;   // �����

    private Transform targetPoint;  // ��ǰĿ��㣨A��B��
    private bool facingRight = true;  // �����Ƿ��泯��

    private void Start()
    {
        targetPoint = pointB;  // ��ʼĿ����B��
    }

    private void Update()
    {
        PatrolBetweenPoints();
    }

    private void PatrolBetweenPoints()
    {
        // �����ƶ���Ŀ���
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // ������˵���Ŀ��㣬�л�����һ����
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
            Flip();  // ��ת���˵ĳ���
        }
    }

    private void Flip()
    {
        // ��ת���˳���
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;  // ��תX��
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        // ���ӻ�A���B��
        Gizmos.color = Color.blue;
        if (pointA != null) Gizmos.DrawWireSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.position, 0.2f);
    }
}
