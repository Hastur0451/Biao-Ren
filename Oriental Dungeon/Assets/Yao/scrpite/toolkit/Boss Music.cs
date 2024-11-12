using UnityEngine;

public class TriggerSwitcher2D : MonoBehaviour
{
    [Header("要隐藏的物体")]
    public GameObject objectToHide1;
    public GameObject objectToHide2;

    [Header("要激活的物体")]
    public GameObject objectToActivate;

    [Header("触发设置")]
    [Tooltip("可以触发的对象标签")]
    public string triggerTag = "Player";  // 默认检测Player标签

    [Tooltip("是否需要检查标签")]
    public bool checkTag = true;

    private void Start()
    {
        // 确保要激活的物体在开始时是隐藏的
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }

        // 检查物体引用是否存在
        if (objectToHide1 == null || objectToHide2 == null)
        {
            Debug.LogWarning("有物体未被赋值!");
        }

        // 检查是否有Box Collider 2D组件
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogWarning("缺少BoxCollider2D组件!");
        }
        else if (!boxCollider.isTrigger)
        {
            Debug.LogWarning("BoxCollider2D的isTrigger属性未启用!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果需要检查标签且标签不匹配，则返回
        if (checkTag && !other.CompareTag(triggerTag))
        {
            return;
        }

        // 隐藏指定的两个物体
        if (objectToHide1 != null)
        {
            objectToHide1.SetActive(false);
        }

        if (objectToHide2 != null)
        {
            objectToHide2.SetActive(false);
        }

        // 激活指定的物体
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }

    // 可以被其他脚本调用的公共方法
    public void TriggerSwitch()
    {
        // 隐藏指定的两个物体
        if (objectToHide1 != null)
        {
            objectToHide1.SetActive(false);
        }

        if (objectToHide2 != null)
        {
            objectToHide2.SetActive(false);
        }

        // 激活指定的物体
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}