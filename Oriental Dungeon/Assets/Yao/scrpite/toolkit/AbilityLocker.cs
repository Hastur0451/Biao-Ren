using UnityEngine;

public class AbilityUnlocker : MonoBehaviour
{
    [Header("Movement Settings")]
    public float bobSpeed = 1f;     // 上下晃动的速度
    public float bobHeight = 0.5f;  // 上下晃动的高度

    [Header("Ability Settings")]
    public string playerTag = "Player";
    public MonoBehaviour abilityScriptToActivate;  // 要激活的能力脚本

    [Header("Audio Settings")]
    public AudioClip unlockSound;  // 解锁音效

    private Vector3 startPosition;
    private float bobTime;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && unlockSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // 实现上下晃动
        bobTime += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(bobTime) * bobHeight;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            UnlockAbility();
        }
    }

    private void UnlockAbility()
    {
        if (abilityScriptToActivate != null)
        {
            abilityScriptToActivate.enabled = true;
            Debug.Log($"Ability {abilityScriptToActivate.GetType().Name} unlocked!");
        }
        else
        {
            Debug.LogWarning("No ability script assigned to unlock!");
        }

        PlayUnlockSound();
        Destroy(gameObject);
    }

    private void PlayUnlockSound()
    {
        if (unlockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }
    }

    // 公共方法，用于外部设置要激活的能力脚本
    public void SetAbilityToUnlock(MonoBehaviour abilityScript)
    {
        abilityScriptToActivate = abilityScript;
    }
}