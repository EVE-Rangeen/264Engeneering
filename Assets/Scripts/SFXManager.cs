using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音效管理器，管理所有音效的播放。
/// 2025-06-26杜宜峰
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("音效索引")]
    [Tooltip("冲刺音效")]
    [SerializeField] private int _dashSFXIndex = 7;
    [Tooltip("击中音效")]
    [SerializeField] private int _hitSFXIndex = 8;
    [Tooltip("被击中音效")]
    [SerializeField] private int _hitBySFXIndex = 9;
    [Tooltip("拾取音效")]
    [SerializeField] private int _pickUpSFXIndex = 10;
    [Tooltip("升级音效")]
    [SerializeField] private int _levelUpSFXIndex = 11;
    [Tooltip("使用血瓶音效")]
    [SerializeField] private int _useHealthBottleSFXIndex = 12;
    [Tooltip("点击按钮音效")]
    [SerializeField] private int _clickButtonSFXIndex = 13;

    public AudioMixer _audioMixer;
    private void Awake()
    {
        instance = this;
    }

    //这个数组要存储所有音效 然后在需要播放的函数里面调用PlaySFX或者PlaySFXPitched函数
    //例如 ： SFXManager.instance.PlaySFXPitched(6);表示播放数组里面的第六个音效
    //AudioSource组件中的Output放AudioMixer 音乐放Music 音效放SFX 分别由两个Mixer分别控制 这两个又由Master控制
    public AudioSource[] soundEffects;


    //这个函数是用来播放音调不变的音效 在播放上一个相同音效的时候，会停止上一个音效然后继续播放这个音效
    //比如拾取音效当你连续拾取物品的时候 会先停止上一个拾取音效然后重新播放
    public void PlaySFX(int sfxToPlay)
    {
        soundEffects[sfxToPlay].Stop();
        soundEffects[sfxToPlay].Play();
    }

    //这个函数会随机调整音调 然后播放音效 适用于比如武器开火 敌人命中等音效
    public void PlaySFXPitched(int sfxToPlay)
    {
        soundEffects[sfxToPlay].pitch = Random.Range(.8f, 1.2f);

        PlaySFX(sfxToPlay);
    }

    // 控制总音量
    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("VolumeOfMaster", volume);
    }

    // 控制音乐音量
    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("VolumeOfMusic", volume);
    }

    // 控制音效音量
    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("VolumeOfSFX", volume);
    }

    // 以下是播放各种音效的函数
    /// <summary>
    /// 播放冲刺音效
    /// </summary>
    public void PlayDashSFX()
    {
        PlaySFX(_dashSFXIndex);
    }

    /// <summary>
    /// 播放击中音效
    /// </summary>
    public void PlayHitSFX()
    {
        PlaySFXPitched(_hitSFXIndex);
    }

    /// <summary>
    /// 播放被击中音效
    /// </summary>
    public void PlayHitBySFX()
    {
        PlaySFXPitched(_hitBySFXIndex);
    }

    /// <summary>
    /// 播放拾取音效
    /// </summary>
    public void PlayPickUpSFX()
    {
        PlaySFXPitched(_pickUpSFXIndex);
    }

    /// <summary>
    /// 播放升级音效
    /// </summary>
    public void PlayLevelUpSFX()
    {
        PlaySFX(_levelUpSFXIndex);
    }

    /// <summary>
    /// 播放使用血瓶音效
    /// </summary>
    public void PlayUseHealthBottleSFX()
    {
        PlaySFX(_useHealthBottleSFXIndex);
    }

    /// <summary>
    /// 播放点击按钮音效
    /// </summary>
    public void PlayClickButtonSFX()
    {
        PlaySFX(_clickButtonSFXIndex);
    }
}
