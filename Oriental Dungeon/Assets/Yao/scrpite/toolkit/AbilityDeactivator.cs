using UnityEngine;

public class AbilityDeactivator : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string playerTag = "Player";
    public GameObject objectToDeactivate;

    [Header("Behavior Settings")]
    public bool destroySelfOnDeactivate = true;
    public bool enableBobbing = true;

    [Header("Movement Settings")]
    public float bobSpeed = 1f;
    public float bobHeight = 0.5f;

    [Header("Audio Settings")]
    public AudioClip deactivationSound;

    private Vector3 startPosition;
    private float bobTime;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && deactivationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (enableBobbing)
        {
            PerformBobbing();
        }
    }

    private void PerformBobbing()
    {
        bobTime += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(bobTime) * bobHeight;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            DeactivateObject();
        }
    }

    private void DeactivateObject()
    {
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log($"GameObject '{objectToDeactivate.name}' has been deactivated!");
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to deactivate!");
        }

        PlayDeactivationSound();

        if (destroySelfOnDeactivate)
        {
            Destroy(gameObject);
        }
        else if (enableBobbing)
        {
            enableBobbing = false;
            transform.position = startPosition;
        }
    }

    private void PlayDeactivationSound()
    {
        if (deactivationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deactivationSound);
        }
    }

    public void SetObjectToDeactivate(GameObject obj)
    {
        objectToDeactivate = obj;
    }
}