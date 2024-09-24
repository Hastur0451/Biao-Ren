using UnityEngine;

public class DashAbility : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isDashing;
    private float dashTimeLeft;
    private float dashDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDash();
        }
    }

    public void StartDash(float direction)
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTimeLeft = dashDuration;
            dashDirection = direction;
        }
    }

    private void HandleDash()
    {
        if (dashTimeLeft > 0)
        {
            rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
            dashTimeLeft -= Time.fixedDeltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}