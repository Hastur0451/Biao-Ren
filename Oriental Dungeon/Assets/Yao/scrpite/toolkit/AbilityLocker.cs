using UnityEngine;

public class AbilityUnlocker : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string playerTag = "Player";
    public GameObject targetObject; // 要激活或不激活的游戏对象
    public enum UnlockAction { Activate, Deactivate }
    public UnlockAction actionOnUnlock = UnlockAction.Activate; // 解锁时执行的动作

    [Header("Behavior Settings")]
    public bool destroySelfOnUnlock = true; // 是否在解锁后销毁自身
    public bool enableBobbing = true; // 是否启用上下晃动

    [Header("Movement Settings")]
    public float bobSpeed = 1f;
    public float bobHeight = 0.5f;

    [Header("Audio Settings")]
    public AudioClip unlockSound;

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

        // 初始化目标对象的状态
        if (targetObject != null)
        {
            targetObject.SetActive(actionOnUnlock == UnlockAction.Deactivate);
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
            UnlockAbility();
        }
    }

    private void UnlockAbility()
    {
        if (targetObject != null)
        {
            // 执行相反的动作，然后设置为目标状态
            targetObject.SetActive(actionOnUnlock == UnlockAction.Deactivate);

            switch (actionOnUnlock)
            {
                case UnlockAction.Activate:
                    targetObject.SetActive(true);
                    Debug.Log($"GameObject '{targetObject.name}' has been activated!");
                    break;
                case UnlockAction.Deactivate:
                    Destroy(targetObject);
                    Debug.Log($"GameObject '{targetObject.name}' has been deactivated!");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No target GameObject assigned!");
        }

        PlayUnlockSound();

        if (destroySelfOnUnlock)
        {
            Destroy(gameObject);
        }
        else if (enableBobbing)
        {
            // 如果不销毁自身但启用了晃动，我们需要停止晃动
            enableBobbing = false;
            transform.position = startPosition; // 重置位置
        }
    }

    private void PlayUnlockSound()
    {
        if (unlockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }
    }

    // 公共方法，用于外部设置目标游戏对象和动作
    public void SetTargetObject(GameObject obj, UnlockAction action)
    {
        targetObject = obj;
        actionOnUnlock = action;

        // 设置初始状态
        if (targetObject != null)
        {
            targetObject.SetActive(action == UnlockAction.Deactivate);
        }
    }
}