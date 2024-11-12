using UnityEngine;
using System.Collections.Generic;

public class ItemDetector : MonoBehaviour
{
    [Tooltip("需要检测的物品")]
    public GameObject[] itemsToDetect;

    [Tooltip("当检测物品消失后要激活的物品")]
    public GameObject[] itemsToActivate;

    [Tooltip("检查频率(秒)")]
    public float checkInterval = 1f;

    [Tooltip("是否在开始时就检查")]
    public bool checkOnStart = true;

    [Tooltip("是否在物品消失后只激活一次")]
    public bool activateOnce = true;

    private bool hasActivated = false;
    private float timer = 0f;

    void Start()
    {
        // 确保数组不为空
        if (itemsToDetect == null || itemsToDetect.Length == 0)
        {
            Debug.LogWarning("没有设置需要检测的物品!");
            return;
        }

        if (itemsToActivate == null || itemsToActivate.Length == 0)
        {
            Debug.LogWarning("没有设置需要激活的物品!");
            return;
        }

        // 在开始时隐藏所有要激活的物品
        HideActivateItems();

        // 如果设置为开始时检查，则立即执行一次检查
        if (checkOnStart)
        {
            CheckItems();
        }
    }

    void Update()
    {
        // 如果设置为只激活一次且已经激活过，则返回
        if (hasActivated && activateOnce)
            return;

        // 计时器更新
        timer += Time.deltaTime;

        // 到达检查间隔时间后执行检查
        if (timer >= checkInterval)
        {
            CheckItems();
            timer = 0f;
        }
    }

    void CheckItems()
    {
        bool allItemsGone = true;

        // 检查所有需要检测的物品
        foreach (GameObject item in itemsToDetect)
        {
            // 如果有任何一个物品还存在，则设置标志为false
            if (item != null && item.activeInHierarchy)
            {
                allItemsGone = false;
                break;
            }
        }

        // 根据检查结果执行相应操作
        if (allItemsGone)
        {
            // 如果所有物品都消失了，激活指定物品
            ActivateItems();
        }
        else
        {
            // 如果有物品存在，但激活物体处于激活状态，则重新隐藏
            bool anyActivated = false;
            foreach (GameObject item in itemsToActivate)
            {
                if (item != null && item.activeInHierarchy)
                {
                    anyActivated = true;
                    break;
                }
            }

            // 只有在非"只激活一次"模式下，或者尚未激活过的情况下，才进行隐藏
            if (anyActivated && (!activateOnce || !hasActivated))
            {
                HideActivateItems();
                hasActivated = false;
            }
        }
    }

    void ActivateItems()
    {
        foreach (GameObject item in itemsToActivate)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }
        hasActivated = true;
    }

    void HideActivateItems()
    {
        foreach (GameObject item in itemsToActivate)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }
    }

    // 提供公共方法重置激活状态
    public void ResetActivation()
    {
        hasActivated = false;
        HideActivateItems();
    }

    // 提供公共方法手动检查物品
    public void ManualCheck()
    {
        CheckItems();
    }
}