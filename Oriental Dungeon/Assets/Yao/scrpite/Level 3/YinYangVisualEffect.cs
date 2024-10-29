using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class YinYangVisualEffect : MonoBehaviour
{
    [Header("Post Processing")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private VolumeProfile yinProfile;
    [SerializeField] private VolumeProfile yangProfile;

    [Header("Global Light")]
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D globalLight;
    [SerializeField] private float yinWorldIntensity = 0.3f;
    [SerializeField] private float yangWorldIntensity = 1f;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.5f;
    private float currentTransitionTime = 0f;
    private bool isTransitioning = false;

    private void OnEnable()
    {
        YinYangEventManager.Instance.onYinWorld.AddListener(() => StartTransition(true));
        YinYangEventManager.Instance.onYangWorld.AddListener(() => StartTransition(false));
    }

    private void OnDisable()
    {
        if (YinYangEventManager.Instance != null)
        {
            YinYangEventManager.Instance.onYinWorld.RemoveListener(() => StartTransition(true));
            YinYangEventManager.Instance.onYangWorld.RemoveListener(() => StartTransition(false));
        }
    }

    private void StartTransition(bool toYinWorld)
    {
        isTransitioning = true;
        currentTransitionTime = 0f;

        // 切换后处理配置
        postProcessVolume.profile = toYinWorld ? yinProfile : yangProfile;
    }

    private void Update()
    {
        if (isTransitioning)
        {
            currentTransitionTime += Time.deltaTime;
            float t = currentTransitionTime / transitionDuration;

            if (t >= 1f)
            {
                isTransitioning = false;
                t = 1f;
            }

            // 平滑过渡光照强度
            float targetIntensity = YinYangEventManager.Instance.IsYinWorld ? yinWorldIntensity : yangWorldIntensity;
            float startIntensity = YinYangEventManager.Instance.IsYinWorld ? yangWorldIntensity : yinWorldIntensity;
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
        }
    }
}