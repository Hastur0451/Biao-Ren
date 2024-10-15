using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ��Ƶ����
    public AudioClip playerAttackClip; // ��ҹ�����Ч
    public AudioClip playerHitClip; // ����ܻ���Ч
    public AudioClip enemyAttackClip; // ���˹�����Ч
    public AudioClip enemyHitClip; // �����ܻ���Ч
    public AudioClip backgroundMusic; // ��������

    // AudioSource���
    private AudioSource audioSource;

    void Awake()
    {
        // ��ȡAudioSource���
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // ��Ϸ��ʼʱ���ű�������
        PlayBackgroundMusic(0.5f); // ������������
    }

    // ������ҹ�����Ч
    public void PlayPlayerAttackSound()
    {
        audioSource.PlayOneShot(playerAttackClip);
    }

    // ��������ܻ���Ч
    public void PlayPlayerHitSound()
    {
        audioSource.PlayOneShot(playerHitClip);
    }

    // ���ŵ��˹�����Ч
    public void PlayEnemyAttackSound()
    {
        audioSource.PlayOneShot(enemyAttackClip);
    }

    // ���ŵ����ܻ���Ч
    public void PlayEnemyHitSound()
    {
        audioSource.PlayOneShot(enemyHitClip);
    }

    // ���ű�������
    public void PlayBackgroundMusic(float volume = 1.0f) // Ĭ������Ϊ1.0
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // ����ѭ������
            audioSource.volume = volume; // ��������
            audioSource.Play();
        }
    }

    // ֹͣ��������
    public void StopBackgroundMusic()
    {
        audioSource.Stop();
    }

    // ���ñ�����������
    public void SetBackgroundMusicVolume(float volume)
    {
        audioSource.volume = volume; // ��������
    }
}
