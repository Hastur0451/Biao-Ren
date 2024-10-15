using UnityEngine;
using TMPro;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] private TextMeshPro savePointText;
    [SerializeField] private string savePointMessage = "Press E to save checkpoint";
    [SerializeField] private float checkRadius = 2f;

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
            // 保存玩家位置
            PlayerPrefs.SetFloat("SavedPosX", transform.position.x);
            PlayerPrefs.SetFloat("SavedPosY", transform.position.y);
            PlayerPrefs.Save();

            Debug.Log("Checkpoint saved at: " + transform.position);

            // 隐藏文本
            if (savePointText != null)
            {
                savePointText.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}