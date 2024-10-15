using UnityEngine;
using System.Collections;

public class PlatformPauseAbility : MonoBehaviour
{
    public float pauseDuration = 5f;
    public KeyCode activationKey = KeyCode.F;
    public AudioClip abilitySound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private bool isAbilityActive = false;
    private float cooldownTime = 0f;
    private AudioSource audioSource;
    private MovingPlatform2D[] allPlatforms;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;

        // ��ȡ���������е��ƶ�ƽ̨
        allPlatforms = FindObjectsOfType<MovingPlatform2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(activationKey) && !isAbilityActive && Time.time > cooldownTime)
        {
            StartCoroutine(ActivateAbility());
        }
    }

    IEnumerator ActivateAbility()
    {
        isAbilityActive = true;

        if (abilitySound != null)
        {
            audioSource.PlayOneShot(abilitySound, soundVolume);
        }

        // ��ͣ����ƽ̨
        SetAllPlatformsPaused(true);
        Debug.Log("Platforms paused");

        yield return new WaitForSeconds(pauseDuration);

        // �ָ�����ƽ̨
        SetAllPlatformsPaused(false);
        Debug.Log("Platforms resumed");

        isAbilityActive = false;
        cooldownTime = Time.time + pauseDuration;
    }

    void SetAllPlatformsPaused(bool isPaused)
    {
        foreach (var platform in allPlatforms)
        {
            platform.SetPauseState(isPaused);
        }
    }
}