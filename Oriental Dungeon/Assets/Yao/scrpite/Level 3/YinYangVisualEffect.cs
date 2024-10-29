using UnityEngine;
using UnityEngine.UI;

public class YinYangDarkEffect : MonoBehaviour
{
    [Header("Darkness Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float darkAmount = 0.8f;  // 阴界暗度
    [SerializeField] private float transitionSpeed = 10f;  // 切换速度

    private Image darkPanel;
    private float targetAlpha = 0f;

    void Start()
    {
        SetupDarkEffect();
    }

    void SetupDarkEffect()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("DarkCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;  // 确保在最上层

        // 添加CanvasScaler以适应不同分辨率
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建黑色面板
        GameObject panelObj = new GameObject("DarkPanel");
        panelObj.transform.SetParent(canvas.transform, false);

        darkPanel = panelObj.AddComponent<Image>();
        darkPanel.color = new Color(0, 0, 0, 0);  // 初始完全透明

        // 设置全屏
        RectTransform rect = darkPanel.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        // 将Canvas设为这个物体的子物体
        canvasObj.transform.SetParent(transform);

        Debug.Log("Dark effect system initialized");
    }

    // 切换到阴界
    public void OnYinBroadcast()
    {
        targetAlpha = darkAmount;
    }

    // 切换到阳界
    public void OnYangBroadcast()
    {
        targetAlpha = 0f;
    }

    void Update()
    {
        if (darkPanel != null)
        {
            // 平滑过渡
            Color currentColor = darkPanel.color;
            float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * transitionSpeed);
            darkPanel.color = new Color(0, 0, 0, newAlpha);
        }
    }
}