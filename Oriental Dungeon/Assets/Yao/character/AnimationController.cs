using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private CharacterController2D characterController;
    private HealthManager healthManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController2D>();
        healthManager = GetComponent<HealthManager>();

        if (healthManager != null)
        {
            healthManager.OnDeath.AddListener(TriggerDeathAnimation);
            healthManager.OnRespawn.AddListener(TriggerRespawnAnimation);
        }
    }

    private void Update()
    {
        if (characterController != null && characterController.IsMovementEnabled())
        {
            UpdateMovementAnimations();
        }
    }

    private void UpdateMovementAnimations()
    {
        animator.SetBool("Run", characterController.IsMoving() && characterController.IsGrounded());
        
        if (characterController.IsJumping())
        {
            animator.SetTrigger("Jump");
        }

        animator.SetBool("IsGrounded", characterController.IsGrounded());
    }

    public void TriggerDeathAnimation()
    {
        animator.SetTrigger("Die");
    }

    public void TriggerRespawnAnimation()
    {
        animator.SetTrigger("Respawn");
        // 重置其他动画状态
        animator.SetBool("Run", false);
        animator.SetBool("IsGrounded", true);
    }
}