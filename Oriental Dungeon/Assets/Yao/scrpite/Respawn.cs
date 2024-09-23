using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RespawnSystem : MonoBehaviour
{
    public static RespawnSystem Instance;

    [SerializeField] private TextMeshPro savePointText;
    [SerializeField] private string savePointMessage = "Press E to save checkpoint";
    [SerializeField] private float checkRadius = 2f;

    private List<Transform> respawnPoints = new List<Transform>();
    private Transform lastSavedPoint;
    private Transform playerTransform;
    private bool isNearSavePoint = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("RespawnSystem Awake");
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Respawn system will not function correctly.");
        }

        if (savePointText != null)
        {
            savePointText.gameObject.SetActive(false);
            Debug.Log("SavePointText initialized and hidden");
        }
        else
        {
            Debug.LogError("SavePointText (TextMeshPro) is not assigned!");
        }
    }

    private void Update()
    {
        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            CheckForSavePoint();
        }
    }

    private void CheckForSavePoint()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerTransform.position, checkRadius);
        bool foundSavePoint = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("SavePoint"))
            {
                foundSavePoint = true;
                break;
            }
        }

        if (foundSavePoint != isNearSavePoint)
        {
            isNearSavePoint = foundSavePoint;
            if (isNearSavePoint)
            {
                Debug.Log("Player entered save point area.");
                if (savePointText != null)
                {
                    savePointText.gameObject.SetActive(true);
                    savePointText.text = savePointMessage;
                }
            }
            else
            {
                Debug.Log("Player left save point area.");
                if (savePointText != null)
                {
                    savePointText.gameObject.SetActive(false);
                }
            }
        }

        if (isNearSavePoint && Input.GetKeyDown(KeyCode.E))
        {
            SaveCheckpoint();
        }
    }

    private void SaveCheckpoint()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerTransform.position, checkRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("SavePoint"))
            {
                Vector3 savePosition = hitCollider.transform.position;
                savePosition.z = 0f; // 确保保存的位置Z轴为0
                lastSavedPoint = new GameObject("SavedPoint").transform;
                lastSavedPoint.position = savePosition;
                Debug.Log("Checkpoint saved at: " + lastSavedPoint.position);
                return;
            }
        }
        Debug.LogWarning("Tried to save checkpoint, but no save point found nearby.");
    }

    public Vector3 GetLastSavedPosition()
    {
        if (lastSavedPoint != null)
        {
            Vector3 savedPosition = lastSavedPoint.position;
            savedPosition.z = 0f; // 再次确保返回的位置Z轴为0
            return savedPosition;
        }
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, checkRadius);
        }
    }
}