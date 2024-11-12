using UnityEngine;

public class YinYangTrigger : MonoBehaviour
{
    [SerializeField] private bool broadcastYang = true;  // true为阳广播，false为阴广播

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 获取所有相关对象
            var objects = FindObjectsOfType<YinYangObject>();
            var lights = FindObjectsOfType<ControlledLight2D>();

            if (broadcastYang)
            {
                // 发送阳广播
                foreach (var obj in objects)
                {
                    obj.OnYangBroadcast();
                }
                foreach (var light in lights)
                {
                    light.OnYangBroadcast();
                }
                Debug.Log("Yang Broadcast Triggered");
            }
            else
            {
                // 发送阴广播
                foreach (var obj in objects)
                {
                    obj.OnYinBroadcast();
                }
                foreach (var light in lights)
                {
                    light.OnYinBroadcast();
                }
                Debug.Log("Yin Broadcast Triggered");
            }
        }
    }
}