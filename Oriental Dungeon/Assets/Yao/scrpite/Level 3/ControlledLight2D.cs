using UnityEngine.Rendering.Universal;
using UnityEngine;

public class ControlledLight2D : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private float lightRadius = 3f;
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private bool useCustomFalloff = true;
    [Range(0f, 1f)]
    [SerializeField] private float innerRadiusRatio = 0.5f;

    [Header("Color Settings")]
    [SerializeField] private Color lightColor = Color.white;

    [SerializeField] private Light2D light2D; // 直接引用Light2D组件

    private bool isInYinWorld = false;

    void Awake()
    {
        // 检查是否已手动添加Light2D组件
        if (light2D == null)
        {
            Debug.LogError("请在编辑器中手动将Light2D组件引用到ControlledLight2D脚本上");
            return;
        }

        // 设置光源属性
        SetupLight();
    }

    void OnEnable()
    {
        UpdateLightState();
    }

    void SetupLight()
    {
        // 设置基本属性
        light2D.lightType = Light2D.LightType.Point;
        light2D.pointLightOuterRadius = lightRadius;
        light2D.color = lightColor;
        light2D.lightOrder = 1;

        if (useCustomFalloff)
        {
            light2D.pointLightInnerRadius = lightRadius * innerRadiusRatio;
            light2D.falloffIntensity = 0.5f;
        }

        // 初始化时设置强度为0
        light2D.intensity = 0;
    }

    private void UpdateLightState()
    {
        if (light2D != null)
        {
            light2D.intensity = isInYinWorld ? lightIntensity : 0;
            Debug.Log($"Light state updated - IsInYinWorld: {isInYinWorld}, Intensity: {light2D.intensity}");
        }
    }

    public void OnYinBroadcast()
    {
        isInYinWorld = true;
        UpdateLightState();
        Debug.Log("Yin broadcast received - Light should be on");
    }

    public void OnYangBroadcast()
    {
        isInYinWorld = false;
        UpdateLightState();
        Debug.Log("Yang broadcast received - Light should be off");
    }

    public void SetLightRadius(float radius)
    {
        lightRadius = radius;
        if (light2D != null)
        {
            light2D.pointLightOuterRadius = radius;
            if (useCustomFalloff)
            {
                light2D.pointLightInnerRadius = radius * innerRadiusRatio;
            }
        }
    }

    public void SetLightColor(Color color)
    {
        lightColor = color;
        if (light2D != null)
        {
            light2D.color = color;
        }
    }
}
