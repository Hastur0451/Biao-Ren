using UnityEngine;

public class HealthModifier : MonoBehaviour
{
    [System.Serializable]
    public class HealthModification
    {
        public bool enabled = false;
        public int amount = 0;
    }

    [Tooltip("对目标造成伤害")]
    public HealthModification damage;

    [Tooltip("治疗目标")]
    public HealthModification healing;

    [Tooltip("增加目标的最大生命值")]
    public HealthModification maxHealthIncrease;

    [Tooltip("指定该修改器应该影响的标签")]
    public string targetTag = "Player";

    [Tooltip("是否在触发后销毁此对象")]
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

    // 用于在Scene视图中可视化触发器
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f); // 半透明的粉红色
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(collider.offset, collider.bounds.size);
        }
    }
}