using UnityEngine;

public class PlayerYinYangTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.R;
    private bool lastBroadcastWasYang = false;
    private YinYangWorldSystem worldSystem;

    private void Start()
    {
        worldSystem = FindObjectOfType<YinYangWorldSystem>();
        if (worldSystem == null)
        {
            Debug.LogWarning("YinYangWorldSystem not found!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (lastBroadcastWasYang)
            {
                worldSystem?.OnYinBroadcast();
                BroadcastToObjects(false);
                lastBroadcastWasYang = false;
            }
            else
            {
                worldSystem?.OnYangBroadcast();
                BroadcastToObjects(true);
                lastBroadcastWasYang = true;
            }
        }
    }

    private void BroadcastToObjects(bool isYang)
    {
        var objects = FindObjectsOfType<YinYangObject>();
        foreach (var obj in objects)
        {
            if (isYang)
                obj.OnYangBroadcast();
            else
                obj.OnYinBroadcast();
        }
    }
}