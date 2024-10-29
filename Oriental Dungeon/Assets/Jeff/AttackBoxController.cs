using UnityEngine;
using System.Collections;

public class AttackBoxController : MonoBehaviour
{
    [Header("Sprite References")]
    public SpriteRenderer swipe1;
    public SpriteRenderer swipe2;

    [Header("Animation Timing")]
    public float swipe1Duration = 0.05f;
    public float swipe2Duration = 0.05f;
    public float swipeOverlapTime = 0.02f; // Time both swipes are visible together

    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float normalAttackAlpha = 1f;
    [Range(0f, 1f)]
    public float heavyAttackAlpha = 1f;

    [Header("Visual Feedback")]
    public bool scaleWithCharge = true;
    public Vector3 maxChargeScale = new Vector3(1.2f, 1.2f, 1f);

    private PolygonCollider2D hitboxCollider;
    private CharacterAttack characterAttack;
    private Vector3 originalScale;
    private Color originalSwipe1Color;
    private Color originalSwipe2Color;
    private Coroutine currentSwipeRoutine;

    private void Start()
    {
        // Get required components
        hitboxCollider = GetComponent<PolygonCollider2D>();
        characterAttack = GetComponentInParent<CharacterAttack>();

        // Store original values
        originalScale = transform.localScale;
        originalSwipe1Color = swipe1.color;
        originalSwipe2Color = swipe2.color;

        // Initially hide sprites and disable collider
        swipe1.enabled = false;
        swipe2.enabled = false;
        hitboxCollider.enabled = false;

        // Subscribe to the charging event
        if (characterAttack != null)
        {
            characterAttack.OnChargingStateChanged += HandleChargingStateChanged;
        }
    }

    private void OnEnable()
    {
        if (characterAttack != null)
        {
            characterAttack.OnChargingStateChanged += HandleChargingStateChanged;
        }
    }

    private void OnDisable()
    {
        if (characterAttack != null)
        {
            characterAttack.OnChargingStateChanged -= HandleChargingStateChanged;
        }
    }

    private void Update()
    {
        if (characterAttack != null && characterAttack.IsChargingHeavyAttack && scaleWithCharge)
        {
            float chargeProgress = characterAttack.ChargeProgress;
            transform.localScale = Vector3.Lerp(originalScale, maxChargeScale, chargeProgress);
        }
    }

    private void HandleChargingStateChanged(bool isCharging)
    {
        if (!isCharging)
        {
            transform.localScale = originalScale;
        }
    }

    public void ShowAttackEffect(bool isHeavyAttack, float duration)
    {
        if (currentSwipeRoutine != null)
        {
            StopCoroutine(currentSwipeRoutine);
        }

        currentSwipeRoutine = StartCoroutine(PlaySwipeSequence(isHeavyAttack, duration));
        hitboxCollider.enabled = true;
    }

    private IEnumerator PlaySwipeSequence(bool isHeavyAttack, float totalDuration)
    {
        // Set alpha based on attack type
        float targetAlpha = isHeavyAttack ? heavyAttackAlpha : normalAttackAlpha;

        // Set colors with appropriate alpha
        Color swipe1Color = originalSwipe1Color;
        Color swipe2Color = originalSwipe2Color;
        swipe1Color.a = targetAlpha;
        swipe2Color.a = targetAlpha;

        // Show first swipe
        swipe1.enabled = true;
        swipe1.color = swipe1Color;

        yield return new WaitForSeconds(swipe1Duration - swipeOverlapTime);

        // Show second swipe while first is still visible
        swipe2.enabled = true;
        swipe2.color = swipe2Color;

        yield return new WaitForSeconds(swipeOverlapTime);

        // Hide first swipe
        swipe1.enabled = false;

        yield return new WaitForSeconds(swipe2Duration);

        // Hide second swipe
        swipe2.enabled = false;

        // Wait for remaining duration if any
        float remainingTime = totalDuration - (swipe1Duration + swipe2Duration - swipeOverlapTime);
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // Cleanup
        hitboxCollider.enabled = false;
        transform.localScale = originalScale;
    }

    private void OnValidate()
    {
        // Ensure we have the required components and references
        if (swipe1 == null)
        {
            Debug.LogError("Swipe1 SpriteRenderer reference is missing!");
        }
        if (swipe2 == null)
        {
            Debug.LogError("Swipe2 SpriteRenderer reference is missing!");
        }
        if (GetComponent<PolygonCollider2D>() == null)
        {
            Debug.LogError("AttackBoxController requires a PolygonCollider2D component!");
        }
    }
}