using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // 音频剪辑
    public AudioClip playerAttackClip; // 玩家攻击音效
    public AudioClip playerHitClip; // 玩家受击音效
    public AudioClip enemyAttackClip; // 敌人攻击音效
    public AudioClip enemyHitClip; // 敌人受击音效
    public AudioClip backgroundMusic; // 背景音乐

    // AudioSource组件
    private AudioSource audioSource;

    void Awake()
    {
        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // 游戏开始时播放背景音乐
        PlayBackgroundMusic(0.5f); // 可以设置音量
    }

    // 播放玩家攻击音效
    public void PlayPlayerAttackSound()
    {
        audioSource.PlayOneShot(playerAttackClip);
    }

    // 播放玩家受击音效
    public void PlayPlayerHitSound()
    {
        audioSource.PlayOneShot(playerHitClip);
    }

    // 播放敌人攻击音效
    public void PlayEnemyAttackSound()
    {
        audioSource.PlayOneShot(enemyAttackClip);
    }

    // 播放敌人受击音效
    public void PlayEnemyHitSound()
    {
        audioSource.PlayOneShot(enemyHitClip);
    }

    // 播放背景音乐
    public void PlayBackgroundMusic(float volume = 1.0f) // 默认音量为1.0
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // 设置循环播放
            audioSource.volume = volume; // 设置音量
            audioSource.Play();
        }
    }

    // 停止背景音乐
    public void StopBackgroundMusic()
    {
        audioSource.Stop();
    }

    // 设置背景音乐音量
    public void SetBackgroundMusicVolume(float volume)
    {
        audioSource.volume = volume; // 设置音量
    }
}
