using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏计时器，使用单例模式管理游戏时间
/// </summary>
public class Timer : MonoBehaviour
{
    public static Timer instance;
    private float _gameStartTime;
    void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 获取格式化的游戏时间（分:秒格式）
    /// </summary>
    /// <returns>格式化的时间字符串，如 "05:30"</returns>
    public string GetTime()
    {
        float elapsedTime = Time.time - _gameStartTime;
        int totalSeconds = (int)elapsedTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        _gameStartTime = Time.time;
    }

    public void PauseTimer()
    {
        Time.timeScale = 0;
    }

    public void ResumeTimer()
    {
        Time.timeScale = 1;
    }   
}
