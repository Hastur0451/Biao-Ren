using UnityEngine;
using UnityEngine.UI;

public class DarkEffectTest : MonoBehaviour
{
    private Image darkPanel;
    private bool isDark = false;

    void Start()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("DarkCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasObj.AddComponent<CanvasScaler>();

        // 创建黑色面板
        GameObject panelObj = new GameObject("DarkPanel");
        panelObj.transform.SetParent(canvas.transform, false);

        darkPanel = panelObj.AddComponent<Image>();
        darkPanel.color = new Color(0, 0, 0, 0); // 开始时完全透明

        // 设置面板大小为全屏
        RectTransform rect = darkPanel.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        Debug.Log("Dark effect initialized");
    }

    void Update()
    {
        // 按空格键测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDark = !isDark;
            darkPanel.color = new Color(0, 0, 0, isDark ? 0.8f : 0);
            Debug.Log(isDark ? "Switching to dark" : "Switching to normal");
        }
    }
}