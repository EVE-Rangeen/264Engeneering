using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 属性升级数据类
/// 存储单个属性的升级相关信息
/// </summary>
[System.Serializable]
public class AttributeUpgradeData
{
    [Header("基础信息")]
    [SerializeField] private string _attributeName;          // 属性名称
    [SerializeField] private string _attributeDescription;   // 属性描述
    
    [Header("等级信息")]
    [SerializeField] private int _currentLevel;              // 当前等级
    [SerializeField] private int _maxLevel;                  // 最高等级
    
    [Header("升级消耗")]
    [SerializeField] private List<int> _upgradeCosts = new List<int>();        // 每级升级费用列表
    
    [Header("升级数值")]
    [SerializeField] private List<float> _upgradeValues = new List<float>();   // 每级属性数值列表

    /// <summary>
    /// 属性名称
    /// </summary>
    public string AttributeName => _attributeName;

    /// <summary>
    /// 属性描述
    /// </summary>
    public string AttributeDescription => _attributeDescription;

    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel => _currentLevel;

    /// <summary>
    /// 最高等级
    /// </summary>
    public int MaxLevel => _maxLevel;

    /// <summary>
    /// 每级升级费用列表
    /// </summary>
    public IReadOnlyList<int> UpgradeCosts => _upgradeCosts;

    /// <summary>
    /// 每级属性数值列表
    /// </summary>
    public IReadOnlyList<float> UpgradeValues => _upgradeValues;

    /// <summary>
    /// 是否已达到最高等级
    /// </summary>
    public bool IsMaxLevel => _currentLevel >= _maxLevel;

    /// <summary>
    /// 设置当前等级（仅供PlayerStatsManager使用）
    /// </summary>
    /// <param name="level">新的等级值</param>
    internal void SetCurrentLevel(int level)
    {
        _currentLevel = Mathf.Clamp(level, 0, _maxLevel);
    }
} 