using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    public Image fillImage;
    public float yOffset = 1.5f; // Adjust this to position the bar above the player's head
    public Vector2 sizeDelta = new Vector2(100, 10); // Width and height of the charge bar

    private CharacterAttack characterAttack;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Camera mainCamera;
    private bool wasInitiallyActive;

    private void Start()
    {
        // Store the initial active state
        wasInitiallyActive = gameObject.activeSelf;

        // Find the player in the scene
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

        // Subscribe to the charging state changed event
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

        // Set the size of the charge bar
        rectTransform.sizeDelta = sizeDelta;

        // Ensure the charge bar starts inactive if it wasn't initially active
        if (!wasInitiallyActive)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the script is destroyed
        if (characterAttack != null)
        {
            characterAttack.OnChargingStateChanged -= HandleChargingStateChanged;
        }
    }

    private void HandleChargingStateChanged(bool isCharging)
    {
        gameObject.SetActive(isCharging);
    }

    private void LateUpdate()
    {
        if (characterAttack == null || !gameObject.activeSelf) return;

        // Update the fill amount of the charge bar
        fillImage.fillAmount = characterAttack.ChargeProgress;

        // Update the position of the charge bar to stay above the player's head
        Vector3 worldPosition = characterAttack.transform.position + Vector3.up * yOffset;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // Convert screen position to local position within the canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }
}