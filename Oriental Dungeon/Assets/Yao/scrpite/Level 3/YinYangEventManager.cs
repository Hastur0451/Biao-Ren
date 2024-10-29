using UnityEngine;
using UnityEngine.Events;

public class YinYangEventManager : MonoBehaviour
{
    // 单例模式
    public static YinYangEventManager Instance { get; private set; }

    // 定义事件
    public UnityEvent onYinWorld = new UnityEvent();
    public UnityEvent onYangWorld = new UnityEvent();

    // 当前世界状态
    private bool isYinWorld = false;
    public bool IsYinWorld => isYinWorld;

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
    }

    public void ToggleWorld()
    {
        isYinWorld = !isYinWorld;
        if (isYinWorld)
        {
            onYinWorld.Invoke();
        }
        else
        {
            onYangWorld.Invoke();
        }
    }
}
