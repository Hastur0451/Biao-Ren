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

    private Light2D light2D;
    private bool isInYinWorld = false;

    void Start()
    {
        SetupLight();
    }

    void SetupLight()
    {
        light2D = gameObject.AddComponent<Light2D>();
        light2D.lightType = Light2D.LightType.Point;
        light2D.pointLightOuterRadius = lightRadius;
        light2D.intensity = 0; // 初始时关闭光源

        if (useCustomFalloff)
        {
            light2D.pointLightInnerRadius = lightRadius * innerRadiusRatio;
            light2D.falloffIntensity = 0.5f;
        }

        // 确保光照在最上层，但在UI层之下
        light2D.lightOrder = 1;
    }

    public void OnYinBroadcast()
    {
        isInYinWorld = true;
        light2D.intensity = lightIntensity;
    }

    public void OnYangBroadcast()
    {
        isInYinWorld = false;
        light2D.intensity = 0;
    }

    // 提供运行时调整方法
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
}
