using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 饰品管理器
/// 管理所有饰品的数据和实例
/// </summary>
public class AccessoryManager : MonoBehaviour
{
    public static AccessoryManager instance;
    
    [Header("饰品配置")]
    [SerializeField] private List<AccessoryData> _accessoryDataList = new List<AccessoryData>();
    
    [Header("当前饰品实例 (运行时自动生成)")]
    [SerializeField] private List<AccessoryData> _currentAccessories = new List<AccessoryData>();
    
    /// <summary>
    /// 获取所有饰品初始数据（只读）
    /// </summary>
    public List<AccessoryData> AccessoryDataList => _accessoryDataList;
    
    /// <summary>
    /// 获取当前饰品实例列表（只读）
    /// </summary>
    public List<AccessoryData> CurrentAccessories => _currentAccessories;
    
    void Awake()
    {
        instance = this;
    }
    
    
} 