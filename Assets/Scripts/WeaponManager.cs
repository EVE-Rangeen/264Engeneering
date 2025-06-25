using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器管理器
/// 管理所有武器的初始数据和当前实例
/// </summary>
public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// 武器管理器单例实例
    /// </summary>
    public static WeaponManager instance;

    [Header("武器配置")]
    [SerializeField] private List<WeaponData> _weaponDataList = new List<WeaponData>();
    
    [Header("当前武器实例 (运行时自动生成)")]
    [SerializeField] private List<WeaponData> _currentWeapons = new List<WeaponData>();

    /// <summary>
    /// 获取所有武器初始数据（只读）
    /// </summary>
    public IReadOnlyList<WeaponData> WeaponDataList => _weaponDataList;

    /// <summary>
    /// 获取当前武器实例列表（只读）
    /// </summary>
    public IReadOnlyList<WeaponData> CurrentWeapons => _currentWeapons;

    /// <summary>
    /// 初始化单例
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 游戏开始时初始化武器
    /// </summary>
    void Start()
    {
        InitializeWeapons();
    }

    /// <summary>
    /// 初始化武器实例
    /// 从WeaponData列表创建对应的运行时副本
    /// </summary>
    private void InitializeWeapons()
    {
        _currentWeapons.Clear();

        foreach (WeaponData weaponData in _weaponDataList)
        {
            if (weaponData != null)
            {
                // 创建ScriptableObject的运行时副本
                WeaponData weaponInstance = Instantiate(weaponData);
                _currentWeapons.Add(weaponInstance);
                Debug.Log($"初始化武器: {weaponInstance.WeaponName}");
            }
            else
            {
                Debug.LogWarning("WeaponDataList中包含空的WeaponData引用");
            }
        }

        Debug.Log($"武器管理器初始化完成，共加载 {_currentWeapons.Count} 件武器");
    }

    /// <summary>
    /// 根据索引获取武器数据
    /// </summary>
    /// <param name="index">武器索引</param>
    /// <returns>武器数据，如果索引无效返回null</returns>
    public WeaponData GetWeaponData(int index)
    {
        if (index >= 0 && index < _weaponDataList.Count)
        {
            return _weaponDataList[index];
        }
        
        Debug.LogWarning($"无效的武器数据索引: {index}");
        return null;
    }

    /// <summary>
    /// 根据索引获取当前武器实例
    /// </summary>
    /// <param name="index">武器索引</param>
    /// <returns>武器实例，如果索引无效返回null</returns>
    public WeaponData GetCurrentWeapon(int index)
    {
        if (index >= 0 && index < _currentWeapons.Count)
        {
            return _currentWeapons[index];
        }
        
        Debug.LogWarning($"无效的当前武器索引: {index}");
        return null;
    }

    /// <summary>
    /// 根据武器名称获取武器数据
    /// </summary>
    /// <param name="weaponName">武器名称</param>
    /// <returns>武器数据，如果未找到返回null</returns>
    public WeaponData GetWeaponDataByName(string weaponName)
    {
        foreach (WeaponData weaponData in _weaponDataList)
        {
            if (weaponData != null && weaponData.WeaponName == weaponName)
            {
                return weaponData;
            }
        }
        
        Debug.LogWarning($"未找到名为 '{weaponName}' 的武器数据");
        return null;
    }

    /// <summary>
    /// 根据武器名称获取当前武器实例
    /// </summary>
    /// <param name="weaponName">武器名称</param>
    /// <returns>武器实例，如果未找到返回null</returns>
    public WeaponData GetCurrentWeaponByName(string weaponName)
    {
        foreach (WeaponData weapon in _currentWeapons)
        {
            if (weapon != null && weapon.WeaponName == weaponName)
            {
                return weapon;
            }
        }
        
        Debug.LogWarning($"未找到名为 '{weaponName}' 的当前武器实例");
        return null;
    }

    /// <summary>
    /// 添加新的武器数据到列表
    /// </summary>
    /// <param name="weaponData">要添加的武器数据</param>
    public void AddWeaponData(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogWarning("尝试添加空的武器数据");
            return;
        }

        if (!_weaponDataList.Contains(weaponData))
        {
            _weaponDataList.Add(weaponData);
            
            // 同时创建对应的武器实例
            WeaponData newWeapon = Instantiate(weaponData);
            _currentWeapons.Add(newWeapon);
            
            Debug.Log($"添加新武器: {weaponData.WeaponName}");
        }
        else
        {
            Debug.LogWarning($"武器数据 '{weaponData.WeaponName}' 已存在于列表中");
        }
    }

    /// <summary>
    /// 移除武器数据和对应的实例
    /// </summary>
    /// <param name="index">要移除的武器索引</param>
    public void RemoveWeapon(int index)
    {
        if (index >= 0 && index < _weaponDataList.Count)
        {
            string weaponName = _weaponDataList[index]?.WeaponName ?? "未知武器";
            _weaponDataList.RemoveAt(index);
            
            if (index < _currentWeapons.Count)
            {
                if (_currentWeapons[index] != null)
                {
                    DestroyImmediate(_currentWeapons[index]);
                }
                _currentWeapons.RemoveAt(index);
            }
            
            Debug.Log($"移除武器: {weaponName}");
        }
        else
        {
            Debug.LogWarning($"无效的武器移除索引: {index}");
        }
    }

    /// <summary>
    /// 重置所有当前武器到初始状态
    /// </summary>
    public void ResetAllWeaponsToOriginal()
    {
        for (int i = 0; i < _currentWeapons.Count && i < _weaponDataList.Count; i++)
        {
            if (_currentWeapons[i] != null && _weaponDataList[i] != null)
            {
                DestroyImmediate(_currentWeapons[i]);
                _currentWeapons[i] = Instantiate(_weaponDataList[i]);
            }
        }
        
        Debug.Log("所有武器已重置到初始状态");
    }

    /// <summary>
    /// 重置指定武器到初始状态
    /// </summary>
    /// <param name="index">武器索引</param>
    public void ResetWeaponToOriginal(int index)
    {
        if (index >= 0 && index < _currentWeapons.Count && index < _weaponDataList.Count)
        {
            if (_currentWeapons[index] != null && _weaponDataList[index] != null)
            {
                string weaponName = _currentWeapons[index].WeaponName;
                DestroyImmediate(_currentWeapons[index]);
                _currentWeapons[index] = Instantiate(_weaponDataList[index]);
                Debug.Log($"武器 '{weaponName}' 已重置到初始状态");
            }
        }
        else
        {
            Debug.LogWarning($"无效的武器重置索引: {index}");
        }
    }

    /// <summary>
    /// 获取武器总数
    /// </summary>
    /// <returns>武器总数</returns>
    public int GetWeaponCount()
    {
        return _weaponDataList.Count;
    }
} 