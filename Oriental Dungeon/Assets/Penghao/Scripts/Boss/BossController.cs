using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;                   // 玩家位置
    public float attackRange = 3f;             // 近战攻击范围
    public float trackingRange = 6f;           // 追踪范围
    public float farRange = 10f;               // 远程攻击范围
    public float moveSpeed = 2f;               // Boss 移动速度
    public int bossAttackDamage = 20;          // Boss 造成的近战伤害
    public GameObject projectilePrefab;        // 普通投射物预制体
    public GameObject meteorPrefab;            // 陨石（范围攻击）预制体
    public GameObject[] skulls;                // 骷髅头对象数组
    public GameObject swordSlashPrefab;        // 刀光预制体
    public float projectileSpeed = 5f;         // 普通投射物速度
    public float cooldownTime = 5f;            // 远程攻击冷却时间
    public float slashCooldownTime = 2f;       // 刀光攻击冷却时间
    public float spreadAngle = 30f;            // 多重发射的扇形角度
    public int numberOfProjectiles = 3;        // 多重发射的投射物数量
    public float skullShowTime = 10f;          // 骷髅头显示时间
    public float skullCooldownTime = 15f;      // 骷髅头技能冷却时间
    public Animator animator;

    private bool isOnCooldown = false;
    private bool isSlashOnCooldown = false;
    private bool isMovingToPlayer = false;
    private bool facingLeft = true;

    private void Start()
    {
        // 确保所有骷髅头和刀光在游戏开始时是隐藏的
        foreach (var skull in skulls)
        {
            skull.SetActive(false);
        }
        if (swordSlashPrefab != null)
        {
            swordSlashPrefab.SetActive(false); // 初始隐藏刀光
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            AttackPlayerWithSwordSlash();
        }
        else if (distanceToPlayer <= trackingRange && distanceToPlayer > attackRange)
        {
            StartMovingToPlayer();
        }
        else if (distanceToPlayer <= farRange && !isOnCooldown && !isMovingToPlayer)
        {
            StartCoroutine(RandomRangedAttack());
        }
        else
        {
            StopMoving();
        }
    }

    private void StartMovingToPlayer()
    {
        isMovingToPlayer = true;
        animator.SetBool("isMoving", true);
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        FacePlayer();
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void StopMoving()
    {
        isMovingToPlayer = false;
        animator.SetBool("isMoving", false);
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && facingLeft)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && !facingLeft)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void AttackPlayerWithSwordSlash()
    {
        if (!isSlashOnCooldown && swordSlashPrefab != null)
        {
            StartCoroutine(SwordSlashAttack());
        }
    }

    private IEnumerator SwordSlashAttack()
    {
        isSlashOnCooldown = true;

        // 显示刀光
        swordSlashPrefab.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 刀光显示的持续时间，可根据需求调整

        // 隐藏刀光
        swordSlashPrefab.SetActive(false);

        yield return new WaitForSeconds(slashCooldownTime); // 刀光攻击冷却时间

        isSlashOnCooldown = false;
    }

    private IEnumerator RandomRangedAttack()
    {
        isOnCooldown = true;

        // 随机选择一种远程攻击
        float randomValue = Random.value;
        if (randomValue < 0.3f)
        {
            FireSingleProjectile(); // 30% 概率：发射单一投射物
        }
        else if (randomValue < 0.5f)
        {
            FireMultipleProjectiles(); // 20% 概率：多重发射
        }
        else if (randomValue < 0.75f)
        {
            CastMeteorAttack(); // 25% 概率：陨石攻击
        }
        else
        {
            ActivateSkullAttack(); // 25% 概率：骷髅头攻击
        }

        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    private void FireSingleProjectile()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
        Destroy(projectile, 5f);
        Debug.Log("Boss fires a single projectile!");
    }

    private void FireMultipleProjectiles()
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angleOffset = spreadAngle * (i - (numberOfProjectiles - 1) / 2.0f);
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
            Vector3 direction = rotation * (player.position - transform.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            Destroy(projectile, 5f);
        }
        Debug.Log("Boss fires multiple projectiles in a spread!");
    }

    private void CastMeteorAttack()
    {
        Vector3 targetPosition = player.position;
        GameObject meteor = Instantiate(meteorPrefab, targetPosition + new Vector3(0, 5f, 0), Quaternion.identity);
        Debug.Log("Boss casts a meteor attack!");
    }

    private void ActivateSkullAttack()
    {
        StartCoroutine(SkullAttackCoroutine());
    }

    private IEnumerator SkullAttackCoroutine()
    {
        foreach (var skull in skulls)
        {
            skull.SetActive(true);
        }
        yield return new WaitForSeconds(skullShowTime);

        foreach (var skull in skulls)
        {
            Vector3 direction = (player.position - skull.transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, skull.transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            Destroy(projectile, 5f);
            skull.SetActive(false);
        }

        Debug.Log("Boss uses skull attack!");
    }

    public void TakeDamage(int damage, bool isHeavyAttack = false)
    {
        // Optional: You can make the boss react differently to heavy vs normal attacks
        if (isHeavyAttack)
        {
            AttackSense.Instance.HitPause(6); // Same as heavyAttackHitPauseDuration
            AttackSense.Instance.CameraShake(0.1f, 0.1f); // Same as heavy attack values
        }
        else
        {
            AttackSense.Instance.HitPause(3); // Same as normalAttackHitPauseDuration
            AttackSense.Instance.CameraShake(0.1f, 0.05f); // Same as normal attack values
        }

        Debug.Log("Boss takes " + damage + " damage!");
    }

    private void Die()
    {
        Debug.Log("Boss has been defeated!");
        Destroy(gameObject);
    }
}
