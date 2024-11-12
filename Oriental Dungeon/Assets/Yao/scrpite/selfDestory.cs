using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    [Tooltip("要检测的目标物体")]
    public GameObject targetObject;

    void Update()
    {
        // 检查目标物体是否存在且被激活
        if (targetObject != null && targetObject.activeInHierarchy)
        {
            // 销毁自身
            Destroy(gameObject);
        }
    }
}