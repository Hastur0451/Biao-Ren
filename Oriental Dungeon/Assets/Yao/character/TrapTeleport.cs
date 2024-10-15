using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapAndInvincibilitySystem : MonoBehaviour
{
    public LayerMask groundLayer;  // 地面层
    public LayerMask trapLayer;    // 陷阱层
    public GameObject trapTrigger; // 用于检测陷阱的触发器对象
    public float invincibilityDuration = 2f; // 无敌时间持续时间
    public float blinkInterval = 0.1f; // 闪烁间隔

    private Vector3 lastSafePosition;  // 最后的安全位置
    private bool isInvincible = false; // 是否处于无敌状态
    private List<Renderer> characterRenderers = new List<Renderer>(); // 角色的所有渲染器组件

    private void Start()
    {
        if (trapTrigger == null)
        {
            Debug.LogError("Trap trigger GameObject is not assigned!");
        }
        lastSafePosition = transform.position; // 初始化最后的安全位置

        // 获取角色所有的渲染器组件
        GetAllRenderers(transform);

        if (characterRenderers.Count == 0)
        {
            Debug.LogError("No Renderer components found in the character or its children!");
        }
    }

    private void GetAllRenderers(Transform parent)
    {
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null)
        {
            characterRenderers.Add(renderer);
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            GetAllRenderers(parent.GetChild(i));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            // 更新最后的安全位置
            lastSafePosition = transform.position;
        }
        else if (!isInvincible && ((1 << other.gameObject.layer) & trapLayer) != 0)
        {
            StartCoroutine(TriggerInvincibility());
        }
    }

    private IEnumerator TriggerInvincibility()
    {
        isInvincible = true;
        Debug.Log("触发陷阱！进入无敌状态");

        // 传送到最后的安全位置
        //transform.position = lastSafePosition;
        //Debug.Log("Player teleported to safe position: " + lastSafePosition);

        // 开始闪烁效果
        StartCoroutine(BlinkEffect());

        // 等待无敌时间结束
        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("无敌状态结束");

        // 确保所有渲染器在无敌状态结束后可见
        SetRenderersEnabled(true);
    }

    private IEnumerator BlinkEffect()
    {
        float endTime = Time.time + invincibilityDuration;
        while (Time.time < endTime)
        {
            SetRenderersEnabled(!characterRenderers[0].enabled);
            yield return new WaitForSeconds(blinkInterval);
        }
        SetRenderersEnabled(true);
    }

    private void SetRenderersEnabled(bool enabled)
    {
        foreach (var renderer in characterRenderers)
        {
            renderer.enabled = enabled;
        }
    }

    // 在 Unity 编辑器中可视化陷阱检测范围
    private void OnDrawGizmos()
    {
        if (trapTrigger != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(trapTrigger.transform.position, trapTrigger.GetComponent<Collider2D>().bounds.size);
        }
    }
}