using UnityEngine;

public class YinYangObject : MonoBehaviour
{
    [SerializeField] private bool isYangObject = true;  // true为阳物体，false为阴物体

    public void OnYangBroadcast()
    {
        // 如果是阳物体则激活，否则隐藏
        SetChildrenActive(isYangObject);
    }

    public void OnYinBroadcast()
    {
        // 如果是阴物体则激活，否则隐藏
        SetChildrenActive(!isYangObject);
    }

    private void SetChildrenActive(bool active)
    {
        // 激活/隐藏所有子物体
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }

        // 处理自身的渲染器和碰撞体
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = active;
        }
        foreach (var collider in GetComponentsInChildren<Collider2D>(true))
        {
            collider.enabled = active;
        }
    }
}