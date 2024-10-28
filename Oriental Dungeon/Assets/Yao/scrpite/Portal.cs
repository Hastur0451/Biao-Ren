using UnityEngine;
using TMPro;

// 单个传送门的脚本
public class TeleportGate : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private string gateID; // 门的唯一标识
    [SerializeField] private TeleportGate destinationGate; // 目标传送门
    [SerializeField] private TextMeshPro promptText;
    [SerializeField] private string teleportMessage = "Press E to teleport";
    [SerializeField] private float teleportRadius = 2f;

    [Header("Teleport Settings")]
    [SerializeField] private Vector2 exitOffset = new Vector2(1f, 0); // 玩家传送后的位置偏移

    private bool playerInRange = false;
    private Transform playerTransform;
    private bool canTeleport = true; // 防止传送循环的标志

    private void Start()
    {
        if (string.IsNullOrEmpty(gateID))
        {
            Debug.LogError($"Gate ID not set for {gameObject.name}");
        }

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"Prompt Text not assigned for {gameObject.name}");
        }

        if (destinationGate == null)
        {
            Debug.LogError($"Destination Gate not set for {gameObject.name}");
        }
    }

    private void Update()
    {
        if (playerInRange && canTeleport && Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerInRange = true;
            ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (promptText != null && destinationGate != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = teleportMessage;
        }
    }

    private void HidePrompt()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void TeleportPlayer()
    {
        if (playerTransform != null && destinationGate != null)
        {
            // 暂时禁用目标门的传送功能，防止传送循环
            destinationGate.DisableTeleport();

            // 计算目标位置（考虑偏移）
            Vector2 targetPosition = (Vector2)destinationGate.transform.position + exitOffset;

            // 传送玩家
            playerTransform.position = targetPosition;

            Debug.Log($"Teleported player from {gateID} to {destinationGate.GetGateID()}");

            // 隐藏提示文本
            HidePrompt();
        }
    }

    // 暂时禁用传送功能
    public void DisableTeleport()
    {
        canTeleport = false;
        Invoke("EnableTeleport", 0.5f); // 0.5秒后重新启用传送
    }

    private void EnableTeleport()
    {
        canTeleport = true;
    }

    public string GetGateID()
    {
        return gateID;
    }

    private void OnDrawGizmos()
    {
        // 绘制传送范围
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);

        // 绘制出口位置
        if (destinationGate != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destinationGate.transform.position);
        }

        // 绘制出口偏移
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + exitOffset, 0.2f);
    }
}

// 传送门管理器（可选，用于更方便地管理多个传送门）
public class TeleportManager : MonoBehaviour
{
    [System.Serializable]
    public class TeleportPair
    {
        public TeleportGate gateA;
        public TeleportGate gateB;
    }

    [SerializeField] private TeleportPair[] teleportPairs;

    private void Start()
    {
        ValidateGates();
    }

    private void ValidateGates()
    {
        foreach (var pair in teleportPairs)
        {
            if (pair.gateA == null || pair.gateB == null)
            {
                Debug.LogError("Incomplete teleport pair found in TeleportManager");
                continue;
            }
        }
    }
}