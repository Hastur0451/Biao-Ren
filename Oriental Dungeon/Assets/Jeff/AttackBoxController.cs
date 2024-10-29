using UnityEngine;
using System.Collections;

public class AttackBoxController : MonoBehaviour
{
    [Header("Normal Attack Sprites")]
    public SpriteRenderer swipe1;     // Will also contain the normal attack collider
    public SpriteRenderer swipe2;

    [Header("Heavy Attack Sprite")]
    public SpriteRenderer heavySwipe;  // Will also contain the heavy attack collider

    [Header("Normal Attack Timing")]
    public float swipe1Duration = 0.05f;
    public float swipe2Duration = 0.05f;
    public float swipeOverlapTime = 0.02f;

    [Header("Heavy Attack Timing")]
    public float heavySwipeDuration = 0.2f;
    public float heavySwipeFadeOutDuration = 0.1f;

    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float normalAttackAlpha = 1f;
    [Range(0f, 1f)]
    public float heavyAttackAlpha = 1f;

    [Header("Visual Feedback")]
    public bool scaleWithCharge = true;
    public Vector3 maxChargeScale = new Vector3(1.2f, 1.2f, 1f);

    private CharacterAttack characterAttack;
    private Vector3 originalScale;
    private Color originalSwipe1Color;
    private Color originalSwipe2Color;
    private Color originalHeavySwipeColor;
    private Coroutine currentSwipeRoutine;

    // Cache collider references
    private PolygonCollider2D normalAttackCollider;
    private PolygonCollider2D heavyAttackCollider;

    private void Start()
    {
        // Get required components
        characterAttack = GetComponentInParent<CharacterAttack>();

        // Get colliders from sprite objects
        normalAttackCollider = swipe1.GetComponent<PolygonCollider2D>();
        heavyAttackCollider = heavySwipe.GetComponent<PolygonCollider2D>();

        if (normalAttackCollider == null || heavyAttackCollider == null)
        {
            Debug.LogError("Missing colliders! Ensure PolygonCollider2D components are attached to Swipe1 and HeavySwipe objects.");
        }

        // Store original values
        originalScale = transform.localScale;
        originalSwipe1Color = swipe1.color;
        originalSwipe2Color = swipe2.color;
        originalHeavySwipeColor = heavySwipe.color;

        // Initially hide all sprites and disable colliders
        DisableAllEffects();

        // Subscribe to the charging event
        if (characterAttack != null)
        {
            characterAttack.OnChargingStateChanged += HandleChargingStateChanged;
        }
    }

    private void DisableAllEffects()
    {
        swipe1.enabled = false;
        swipe2.enabled = false;
        heavySwipe.enabled = false;
        if (normalAttackCollider != null) normalAttackCollider.enabled = false;
        if (heavyAttackCollider != null) heavyAttackCollider.enabled = false;
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

        DisableAllEffects();

        if (isHeavyAttack)
        {
            currentSwipeRoutine = StartCoroutine(PlayHeavyAttack(duration));
            if (heavyAttackCollider != null) heavyAttackCollider.enabled = true;
        }
        else
        {
            currentSwipeRoutine = StartCoroutine(PlayNormalAttackSequence(duration));
            if (normalAttackCollider != null) normalAttackCollider.enabled = true;
        }
    }

    private IEnumerator PlayNormalAttackSequence(float totalDuration)
    {
        // Set colors with appropriate alpha
        Color swipe1Color = originalSwipe1Color;
        Color swipe2Color = originalSwipe2Color;
        swipe1Color.a = normalAttackAlpha;
        swipe2Color.a = normalAttackAlpha;

        // Show first swipe
        swipe1.enabled = true;
        swipe1.color = swipe1Color;

        yield return new WaitForSeconds(swipe1Duration - swipeOverlapTime);

        // Show second swipe while first is still visible
        swipe2.enabled = true;
        swipe2.color = swipe2Color;

        yield return new WaitForSeconds(swipeOverlapTime);

        // Hide first swipe and its collider
        swipe1.enabled = false;
        if (normalAttackCollider != null) normalAttackCollider.enabled = false;

        yield return new WaitForSeconds(swipe2Duration);

        // Hide second swipe
        swipe2.enabled = false;

        // Wait for remaining duration if any
        float remainingTime = totalDuration - (swipe1Duration + swipe2Duration - swipeOverlapTime);
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        transform.localScale = originalScale;
    }

    private IEnumerator PlayHeavyAttack(float totalDuration)
    {
        // Show heavy attack sprite
        heavySwipe.enabled = true;
        Color heavyColor = originalHeavySwipeColor;
        heavyColor.a = heavyAttackAlpha;
        heavySwipe.color = heavyColor;

        // Keep the sprite visible for the main duration
        yield return new WaitForSeconds(totalDuration - heavySwipeFadeOutDuration);

        // Fade out the heavy attack sprite
        float startTime = Time.time;
        Color startColor = heavySwipe.color;
        while (Time.time < startTime + heavySwipeFadeOutDuration)
        {
            float progress = (Time.time - startTime) / heavySwipeFadeOutDuration;
            Color newColor = heavySwipe.color;
            newColor.a = Mathf.Lerp(startColor.a, 0f, progress);
            heavySwipe.color = newColor;
            yield return null;
        }

        // Cleanup
        heavySwipe.enabled = false;
        if (heavyAttackCollider != null) heavyAttackCollider.enabled = false;
        transform.localScale = originalScale;

        // Reset heavy attack sprite alpha for next use
        heavyColor.a = heavyAttackAlpha;
        heavySwipe.color = heavyColor;
    }

    private void OnValidate()
    {
        // Validate sprite references
        if (swipe1 == null)
            Debug.LogError("Swipe1 SpriteRenderer reference is missing!");
        if (swipe2 == null)
            Debug.LogError("Swipe2 SpriteRenderer reference is missing!");
        if (heavySwipe == null)
            Debug.LogError("HeavySwipe SpriteRenderer reference is missing!");

        // Check for colliders on sprite objects
        if (swipe1 != null && swipe1.GetComponent<PolygonCollider2D>() == null)
            Debug.LogError("Swipe1 object is missing PolygonCollider2D component!");
        if (heavySwipe != null && heavySwipe.GetComponent<PolygonCollider2D>() == null)
            Debug.LogError("HeavySwipe object is missing PolygonCollider2D component!");
    }
}