using UnityEngine;

public class PlayerYinYangTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;
    private bool lastBroadcastWasYang = false;  // false表示上次是阴广播(默认)，true表示上次是阳广播

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            // 获取所有YinYangObject
            var objects = FindObjectsOfType<YinYangObject>();

            // 根据上次的广播状态决定这次广播什么
            if (lastBroadcastWasYang)
            {
                // 如果上次是阳，这次广播阴
                foreach (var obj in objects)
                {
                    obj.OnYinBroadcast();
                }
                lastBroadcastWasYang = false;
            }
            else
            {
                // 如果上次是阴，这次广播阳
                foreach (var obj in objects)
                {
                    obj.OnYangBroadcast();
                }
                lastBroadcastWasYang = true;
            }
        }
    }
}