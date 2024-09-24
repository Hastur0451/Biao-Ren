using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers;
    public Transform attackPoint;

    [Header("Audio")]
    public AudioClip attackSound;

    private float nextAttackTime = 0f;
    private PlayerController characterController;
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        characterController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (attackPoint == null)
        {
            Debug.LogError("Attack point is not assigned in the inspector!");
        }
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

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}