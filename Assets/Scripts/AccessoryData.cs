using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 饰品数据类
/// 包含饰品的基本属性和作用函数
/// </summary>
[System.Serializable]
public class AccessoryData
{
    [Header("饰品基本信息")]
    [SerializeField] private string _accessoryName = "新饰品";
    [SerializeField] private int _currentLevel = 0;
    [SerializeField] private int _maxLevel = 10;
    
    [Header("饰品作用函数")]
    [SerializeField] private UnityEvent _effectFunction;
    
    /// <summary>
    /// 饰品名称
    /// </summary>
    public string AccessoryName
    {
        get => _accessoryName;
        set => _accessoryName = value;
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
    
    /// <summary>
    /// 饰品作用函数
    /// </summary>
    public UnityEvent EffectFunction
    {
        get => _effectFunction;
        set => _effectFunction = value;
    }
    
    /// <summary>
    /// 执行饰品效果
    /// </summary>
    public void ExecuteEffect()
    {
        if (_effectFunction != null)
        {
            _effectFunction.Invoke();
        }
        else
        {
            Debug.LogWarning($"饰品 {_accessoryName} 的作用函数未设置！");
        }
    }
} 