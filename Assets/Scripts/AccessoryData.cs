/// <summary>
/// 谈恩萁创建
/// </summary>
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 饰品数据类
/// 包含饰品的基本属性和作用函数
/// </summary>
[System.Serializable]
public class AccessoryData
{
    [Header("饰品基本信息")]
    [SerializeField] private string _accessoryName = "新饰品";
    [SerializeField] private Sprite _accessoryIcon;
    [SerializeField, TextArea(2, 4)] private List<string> _accessoryDescriptions = new List<string> { "饰品描述" };
    [SerializeField] private int _currentLevel = 0;
    [SerializeField] private int _maxLevel = 10;
    
    /// <summary>
    /// 饰品名称
    /// </summary>
    public string AccessoryName
    {
        get => _accessoryName;
        set => _accessoryName = value;
    }
    
    /// <summary>
    /// 饰品图标
    /// </summary>
    public Sprite AccessoryIcon
    {
        get => _accessoryIcon;
        set => _accessoryIcon = value;
    }
    
    /// <summary>
    /// 饰品描述
    /// </summary>
    public List<string> AccessoryDescriptions
    {
        get => _accessoryDescriptions;
        set => _accessoryDescriptions = value;
    }
    
    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = Mathf.Min(value, _maxLevel);
    }
    
    /// <summary>
    /// 最大等级
    /// </summary>
    public int MaxLevel
    {
        get => _maxLevel;
        set => _maxLevel = Mathf.Max(1, value);
    }
    
    /// <summary>
    /// 饰品等级（兼容性属性，返回当前等级）
    /// </summary>
    public int Level
    {
        get => _currentLevel;
        set => CurrentLevel = value;
    }
    
    /// <summary>
    /// 是否已达到最大等级
    /// </summary>
    public bool IsMaxLevel => _currentLevel >= _maxLevel;
} 