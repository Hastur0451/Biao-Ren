using UnityEngine;
using TMPro;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] private TextMeshPro savePointText;
    [SerializeField] private string savePointMessage = "Press E to save checkpoint";
    [SerializeField] private float checkRadius = 2f;

    // 使用静态变量来存储当前会话中的复活点位置
    private static Vector2 savedPosition;
    private static bool hasCheckpoint = false;
    private static Vector2 initialPosition;
    private static bool hasInitialPosition = false;

    private bool playerInRange = false;
    private Transform playerTransform;

    private void Start()
    {
        if (savePointText != null)
        {
            savePointText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("SavePointText (TextMeshPro) is not assigned for " + gameObject.name);
        }

        // 如果还没记录初始位置，记录玩家初始位置
        if (!hasInitialPosition)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                initialPosition = player.transform.position;
                hasInitialPosition = true;
                Debug.Log("Initial position set at: " + initialPosition);
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SaveCheckpoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerInRange = true;
            if (savePointText != null)
            {
                savePointText.gameObject.SetActive(true);
                savePointText.text = savePointMessage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (savePointText != null)
            {
                savePointText.gameObject.SetActive(false);
            }
        }
    }

    private void SaveCheckpoint()
    {
        if (playerTransform != null)
        {
            // 保存当前复活点位置
            savedPosition = transform.position;
            hasCheckpoint = true;
            Debug.Log("Checkpoint saved at: " + savedPosition);

            if (savePointText != null)
            {
                savePointText.gameObject.SetActive(false);
            }
        }
    }

    // 获取重生位置
    public static Vector2 GetRespawnPosition()
    {
        // 如果有保存的复活点，使用复活点
        if (hasCheckpoint)
        {
            return savedPosition;
        }
        // 否则使用初始位置
        else if (hasInitialPosition)
        {
            return initialPosition;
        }
        // 如果都没有，返回原点（不应该发生）
        return Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}