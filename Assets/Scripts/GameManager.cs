using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏管理器，负责管理游戏状态和流程
/// 2025-06-24 肖沐奇 创建
/// </summary>
public class GameManager : MonoBehaviour
{
    // 私有静态字段
    private static GameManager _instance;

    /// <summary>
    /// 单例实例
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// Unity 生命周期 - 初始化
    /// </summary>
    void Awake()
    {
        // 检查是否已经存在实例
        if (_instance != null && _instance != this)
        {
            // 如果已经存在其他实例，销毁当前对象
            Destroy(gameObject);
            return;
        }

        // 设置单例实例
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
