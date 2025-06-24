using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏计时器，使用单例模式管理游戏时间
/// </summary>
public class Timer : MonoBehaviour
{
    /// <summary>
    /// 计时器单例实例
    /// </summary>
    private static Timer _instance;

    /// <summary>
    /// 获取计时器实例
    /// </summary>
    public static Timer Instance
    {
        get
        {
            if (_instance == null)
            {
                // 查找场景中是否已存在Timer对象
                _instance = FindObjectOfType<Timer>();

                // 如果场景中没有Timer对象，创建一个新的
                if (_instance == null)
                {
                    GameObject timerObject = new GameObject("Timer");
                    _instance = timerObject.AddComponent<Timer>();
                    DontDestroyOnLoad(timerObject);
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 游戏开始时间
    /// </summary>
    private float _gameStartTime;

    /// <summary>
    /// 是否已经初始化
    /// </summary>
    private bool _isInitialized = false;

    /// <summary>
    /// 组件唤醒时调用
    /// </summary>
    void Awake()
    {
        // 确保只有一个Timer实例存在
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTimer();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 游戏开始时调用
    /// </summary>
    void Start()
    {
        if (!_isInitialized)
        {
            InitializeTimer();
        }
    }

    /// <summary>
    /// 获取从游戏开始到当前的毫秒数
    /// </summary>
    /// <returns>经过的毫秒数</returns>
    public long GetTime()
    {
        if (!_isInitialized)
        {
            InitializeTimer();
        }

        float elapsedTime = Time.time - _gameStartTime;
        return (long)(elapsedTime * 1000); // 转换为毫秒
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        _gameStartTime = Time.time;
        _isInitialized = true;
    }

    /// <summary>
    /// 初始化计时器
    /// </summary>
    private void InitializeTimer()
    {
        _gameStartTime = Time.time;
        _isInitialized = true;
    }
}
