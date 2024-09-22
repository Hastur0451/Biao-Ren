using UnityEngine;

public class HealthModifier : MonoBehaviour
{
    [System.Serializable]
    public class HealthModification
    {
        public bool enabled = false;
        public int amount = 0;
    }

    [Tooltip("��Ŀ������˺�")]
    public HealthModification damage;

    [Tooltip("����Ŀ��")]
    public HealthModification healing;

    [Tooltip("����Ŀ����������ֵ")]
    public HealthModification maxHealthIncrease;

    [Tooltip("ָ�����޸���Ӧ��Ӱ��ı�ǩ")]
    public string targetTag = "Player";

    [Tooltip("�Ƿ��ڴ��������ٴ˶���")]
    public bool destroyAfterUse = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            HealthManager healthManager = collision.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                ApplyHealthModifications(healthManager);

                if (destroyAfterUse)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void ApplyHealthModifications(HealthManager healthManager)
    {

        if (maxHealthIncrease.enabled && maxHealthIncrease.amount > 0)
        {
            int currentMaxHealth = healthManager.GetMaxHealth();
            healthManager.SetMaxHealth(currentMaxHealth + maxHealthIncrease.amount);
        }

        if (damage.enabled && damage.amount > 0)
        {
            healthManager.TakeDamage(damage.amount);
        }

        if (healing.enabled && healing.amount > 0)
        {
            healthManager.Heal(healing.amount);
        }
    }

    // ������Scene��ͼ�п��ӻ�������
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f); // ��͸���ķۺ�ɫ
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(collider.offset, collider.bounds.size);
        }
    }
}