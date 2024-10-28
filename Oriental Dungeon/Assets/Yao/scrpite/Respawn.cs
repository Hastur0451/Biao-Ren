using UnityEngine;
using TMPro;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] private TextMeshPro savePointText;
    [SerializeField] private string savePointMessage = "Press E to save checkpoint";
    [SerializeField] private float checkRadius = 2f;

    private bool playerInRange = false;
    private Transform playerTransform;
    private Vector2 currentRespawnPoint;

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

        // 查找玩家并设置初始重生点
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            currentRespawnPoint = playerTransform.position;
            Debug.Log("Initial spawn point set at: " + currentRespawnPoint);
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
            currentRespawnPoint = transform.position;
            Debug.Log("Checkpoint saved at: " + currentRespawnPoint);

            if (savePointText != null)
            {
                savePointText.gameObject.SetActive(false);
            }
        }
    }

    public Vector2 GetCurrentRespawnPoint()
    {
        return currentRespawnPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}