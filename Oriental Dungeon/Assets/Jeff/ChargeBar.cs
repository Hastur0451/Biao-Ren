using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    public Image fillImage;
    public float yOffset = 50f; // Vertical offset in screen space
    public Vector2 sizeDelta = new Vector2(100, 10); // Width and height of the charge bar

    [Header("Colors")]
    public Color chargingColor = Color.yellow;
    public Color chargeCompleteColor = Color.red;

    private CharacterAttack characterAttack;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Camera mainCamera;
    private bool wasInitiallyActive;

    private void Start()
    {
        wasInitiallyActive = gameObject.activeSelf;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterAttack = player.GetComponent<CharacterAttack>();
        }
        if (characterAttack == null)
        {
            Debug.LogError("CharacterAttack component not found on Player!");
            enabled = false;
            return;
        }

        characterAttack.OnChargingStateChanged += HandleChargingStateChanged;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        if (canvas == null)
        {
            Debug.LogError("Canvas not found in parents!");
            enabled = false;
            return;
        }

        if (fillImage == null)
        {
            Debug.LogError("Fill Image not assigned to ChargeBar!");
            enabled = false;
        }

        rectTransform.sizeDelta = sizeDelta;

        if (!wasInitiallyActive)
        {
            gameObject.SetActive(false);
        }

        fillImage.color = chargingColor;

        // Ensure the canvas is set to Overlay
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private void OnDestroy()
    {
        if (characterAttack != null)
        {
            characterAttack.OnChargingStateChanged -= HandleChargingStateChanged;
        }
    }

    private void HandleChargingStateChanged(bool isCharging)
    {
        gameObject.SetActive(isCharging);
        if (isCharging)
        {
            fillImage.color = chargingColor;
        }
    }

    private void LateUpdate()
    {
        if (characterAttack == null || !gameObject.activeSelf) return;

        float chargeProgress = characterAttack.ChargeProgress;
        fillImage.fillAmount = chargeProgress;

        if (chargeProgress >= 1f && fillImage.color != chargeCompleteColor)
        {
            fillImage.color = chargeCompleteColor;
        }

        // Update the position of the charge bar to stay above the player's head
        UpdateChargeBarPosition();
    }

    private void UpdateChargeBarPosition()
    {
        if (characterAttack != null && mainCamera != null)
        {
            // Convert player's world position to screen position
            Vector3 screenPos = mainCamera.WorldToScreenPoint(characterAttack.transform.position);

            // Adjust for screen space (Overlay Canvas uses screen space coordinates)
            screenPos.y += yOffset;

            // Convert screen position to local position within the canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, null, out Vector2 localPoint))
            {
                rectTransform.localPosition = localPoint;
            }
        }
    }
}