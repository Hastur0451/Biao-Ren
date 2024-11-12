using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [Tooltip("要检测的目标物体")]
    public GameObject targetObject;

    [Tooltip("要激活的物体")]
    public GameObject objectToActivate;

    [Tooltip("是否在开始时隐藏要激活的物体")]
    public bool hideOnStart = true;

    private void Start()
    {
        // 检查引用是否设置
        if (targetObject == null || objectToActivate == null)
        {
            Debug.LogWarning("目标物体或要激活的物体未设置!");
            return;
        }

        // 如果设置了在开始时隐藏
        if (hideOnStart)
        {
            objectToActivate.SetActive(false);
        }
    }

    void Update()
    {
        // 检查目标物体是否已被销毁
        if (targetObject == null)
        {
            // 激活指定物体
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            // 这个脚本的任务已完成，可以禁用它
            enabled = false;
        }
    }
}