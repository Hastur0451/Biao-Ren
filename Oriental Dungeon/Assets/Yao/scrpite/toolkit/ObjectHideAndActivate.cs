using UnityEngine;

public class AutoReactivateObject : MonoBehaviour
{
    // 需要重新激活的对象
    public GameObject targetObject;

    // 延迟时间（秒）
    public float delay = 5.0f;

    // 内部使用的计时器
    private float timer = 0.0f;
    private bool isCountingDown = false;

    void Update()
    {
        // 检查对象是否被隐藏且未在倒计时
        if (targetObject != null && !targetObject.activeInHierarchy && !isCountingDown)
        {
            // 开始倒计时
            isCountingDown = true;
            timer = delay;
        }

        // 如果正在倒计时
        if (isCountingDown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // 倒计时结束，激活对象
                targetObject.SetActive(true);
                isCountingDown = false;
            }
        }
    }
}
