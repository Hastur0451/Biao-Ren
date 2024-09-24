using UnityEngine;

public class AbilityActivator : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string playerTag = "Player";
    public GameObject objectToActivate;

    [Header("Behavior Settings")]
    public bool destroySelfOnActivate = true;
    public bool enableBobbing = true;

    [Header("Movement Settings")]
    public float bobSpeed = 1f;
    public float bobHeight = 0.5f;

    [Header("Audio Settings")]
    public AudioClip activationSound;

    private Vector3 startPosition;
    private float bobTime;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && activationSound != null)
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
            ActivateObject();
        }
    }

    private void ActivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"GameObject '{objectToActivate.name}' has been activated!");
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to activate!");
        }

        PlayActivationSound();

        if (destroySelfOnActivate)
        {
            Destroy(gameObject);
        }
        else if (enableBobbing)
        {
            enableBobbing = false;
            transform.position = startPosition;
        }
    }

    private void PlayActivationSound()
    {
        if (activationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }

    public void SetObjectToActivate(GameObject obj)
    {
        objectToActivate = obj;
    }
}