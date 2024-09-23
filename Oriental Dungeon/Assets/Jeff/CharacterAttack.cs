using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers;

    public Transform attackPoint;
    private float nextAttackTime = 0f;

    private movecontrol characterController;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<movecontrol>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) // 0 represents the left mouse button
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            // You would call a TakeDamage() method on the enemy here
            // enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    // To visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}