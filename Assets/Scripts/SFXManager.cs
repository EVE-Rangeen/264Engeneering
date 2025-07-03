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
}
