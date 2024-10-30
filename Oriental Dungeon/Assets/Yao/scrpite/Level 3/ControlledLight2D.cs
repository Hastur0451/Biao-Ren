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

    [Header("Debug Visualization")]
    [SerializeField] private bool showDebugLines = true;
    [SerializeField] private Color outerRadiusColor = new Color(1f, 0f, 0f, 0.5f);
    [SerializeField] private Color innerRadiusColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private int circleSegments = 32;

    private Light2D light2D;
    private bool isInYinWorld = false;

    void Awake()
    {
        // 在Awake中初始化，确保最早设置
        SetupLight();
    }

    void OnEnable()
    {
        // 当物体启用时，确保光照状态正确
        UpdateLightState();
    }

    void SetupLight()
    {
        // 检查是否已经存在Light2D组件
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            light2D = gameObject.AddComponent<Light2D>();
        }

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

    private void OnDrawGizmos()
    {
        if (!showDebugLines) return;

        DrawCircle(transform.position, lightRadius, outerRadiusColor);
        if (useCustomFalloff)
        {
            DrawCircle(transform.position, lightRadius * innerRadiusRatio, innerRadiusColor);
        }
    }

    private void DrawCircle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;
        float angleStep = 360f / circleSegments;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius, 0);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius, 0);

            Gizmos.DrawLine(point1, point2);
        }
    }

    private void OnValidate()
    {
        if (light2D != null)
        {
            light2D.pointLightOuterRadius = lightRadius;
            light2D.color = lightColor;

            if (useCustomFalloff)
            {
                light2D.pointLightInnerRadius = lightRadius * innerRadiusRatio;
                light2D.falloffIntensity = 0.5f;
            }

            // 确保验证时也更新状态
            UpdateLightState();
        }
    }
}