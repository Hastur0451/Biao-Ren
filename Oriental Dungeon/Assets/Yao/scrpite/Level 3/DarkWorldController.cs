using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class YinYangWorldSystem : MonoBehaviour
{
    [Header("Dark Effect")]
    [Range(0f, 1f)]
    [SerializeField] private float darkAmount = 0.85f;
    [SerializeField] private float transitionSpeed = 10f;
    [SerializeField] private Canvas darkCanvas;
    private Image darkPanel;

    void Start()
    {
        SetupDarkEffect();
    }

    void SetupDarkEffect()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("DarkCanvas");
        darkCanvas = canvasObj.AddComponent<Canvas>();
        darkCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        darkCanvas.sortingOrder = 999;

        // 添加CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建暗面板
        GameObject panelObj = new GameObject("DarkPanel");
        panelObj.transform.SetParent(darkCanvas.transform, false);

        darkPanel = panelObj.AddComponent<Image>();
        darkPanel.color = new Color(0, 0, 0, 0);

        // 设置全屏
        RectTransform rect = darkPanel.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        canvasObj.transform.SetParent(transform);
        Debug.Log("Dark effect system initialized");
    }

    public void OnYinBroadcast()
    {
        // 处理所有光源
        var lights = FindObjectsOfType<ControlledLight2D>();
        foreach (var light in lights)
        {
            light.OnYinBroadcast();
        }

        // 启动暗效果
        StartCoroutine(TransitionDarkness(darkAmount));
    }

    public void OnYangBroadcast()
    {
        // 处理所有光源
        var lights = FindObjectsOfType<ControlledLight2D>();
        foreach (var light in lights)
        {
            light.OnYangBroadcast();
        }

        // 关闭暗效果
        StartCoroutine(TransitionDarkness(0f));
    }

    private System.Collections.IEnumerator TransitionDarkness(float targetAlpha)
    {
        Color currentColor = darkPanel.color;
        float currentAlpha = currentColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime);
            darkPanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        darkPanel.color = new Color(0, 0, 0, targetAlpha);
    }
}