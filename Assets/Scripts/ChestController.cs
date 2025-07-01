/// <summary>
/// 谈恩萁创建
/// 宝箱控制器
/// 用于管理游戏中的宝箱系统
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝箱控制器
/// 负责管理宝箱的生成、开启和奖励分发
/// </summary>
public class ChestController : MonoBehaviour
{
    public static ChestController instance;

    [Header("宝箱概率配置")]
    [SerializeField] private float _tripleUpgradeChance = 0.2f;  // 随机升级3项的概率（0-1之间）
    [SerializeField] private int _normalUpgradeCount = 1;        // 正常升级数量
    [SerializeField] private int _bonusUpgradeCount = 3;         // 幸运升级数量

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 开启宝箱
    /// </summary>
    public void OpenChest(int chestType = 0)
    {
        // TODO: 实现开启宝箱逻辑
    }
} 