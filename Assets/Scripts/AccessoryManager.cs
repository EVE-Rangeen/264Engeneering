/// <summary>
/// 谈恩萁创建
/// </summary>
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
    
    void Start()
    {
        CreateAccessoryInstances();
    }
    
    /// <summary>
    /// 创建饰品实例
    /// 将配置列表中的饰品数据复制到当前饰品实例列表中
    /// </summary>
    private void CreateAccessoryInstances()
    {
        _currentAccessories.Clear();
        
        foreach (var accessoryData in _accessoryDataList)
        {
            // 创建新的饰品实例，复制配置数据
            AccessoryData newInstance = new AccessoryData();
            newInstance.AccessoryName = accessoryData.AccessoryName;
            newInstance.AccessoryIcon = accessoryData.AccessoryIcon;
            newInstance.AccessoryDescriptions = accessoryData.AccessoryDescriptions;
            newInstance.CurrentLevel = accessoryData.CurrentLevel;
            newInstance.MaxLevel = accessoryData.MaxLevel;
            
            _currentAccessories.Add(newInstance);
        }
    }

    /// <summary>
    /// 执行饰品特殊效果
    /// </summary>
    /// <param name="accessoryData">饰品数据</param>
    /// <param name="newLevel">新等级</param>
    public void ExecuteAccessorySpecialEffect(AccessoryData accessoryData, int newLevel)
    {
        if (accessoryData == null)
        {
            Debug.LogWarning("饰品数据为空，无法执行特殊效果");
            return;
        }

        switch (accessoryData.AccessoryName)
        {
            case "弹巢":
                HandleAmmoNestEffect(newLevel);
                break;
            case "重靴":
                HandleHeavyBootsEffect(newLevel);
                break;
            case "安卡十字":
                HandleAnkhCrossEffect(newLevel);
                break;
            default:
                Debug.LogWarning($"未找到饰品 '{accessoryData.AccessoryName}' 的特殊效果处理方法");
                break;
        }
    }

    /// <summary>
    /// 处理弹巢特殊效果
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleAmmoNestEffect(int newLevel)
    {
        switch (newLevel)
        {
            case 1:
                Debug.Log("弹巢等级1：增加弹药容量");
                break;
            case 2:
                Debug.Log("弹巢等级2：提升装填速度");
                break;
            case 3:
                Debug.Log("弹巢等级3：弹药回复");
                break;
            case 4:
                Debug.Log("弹巢等级4：特殊弹药");
                break;
            case 5:
                Debug.Log("弹巢等级5：连发模式");
                break;
            case 6:
                Debug.Log("弹巢等级6：弹药共享");
                break;
            case 7:
                Debug.Log("弹巢等级7：无限弹药");
                break;
            default:
                Debug.Log($"弹巢等级 {newLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理重靴特殊效果
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleHeavyBootsEffect(int newLevel)
    {
        switch (newLevel)
        {
            case 1:
                Debug.Log("重靴等级1：增加移动稳定性");
                break;
            case 2:
                Debug.Log("重靴等级2：踩踏伤害");
                break;
            case 3:
                Debug.Log("重靴等级3：冲刺攻击");
                break;
            case 4:
                Debug.Log("重靴等级4：震地践踏");
                break;
            case 5:
                Debug.Log("重靴等级5：钢铁之足");
                break;
            case 6:
                Debug.Log("重靴等级6：雷霆践踏");
                break;
            case 7:
                Debug.Log("重靴等级7：毁灭冲击");
                break;
            default:
                Debug.Log($"重靴等级 {newLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理安卡十字特殊效果
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleAnkhCrossEffect(int newLevel)
    {
        switch (newLevel)
        {
            case 1:
                Debug.Log("安卡十字等级1：生命祝福");
                break;
            case 2:
                Debug.Log("安卡十字等级2：缓慢回复");
                break;
            case 3:
                Debug.Log("安卡十字等级3：神圣护盾");
                break;
            case 4:
                Debug.Log("安卡十字等级4：复活之力");
                break;
            case 5:
                Debug.Log("安卡十字等级5：圣光治愈");
                break;
            case 6:
                Debug.Log("安卡十字等级6：不死之身");
                break;
            case 7:
                Debug.Log("安卡十字等级7：永恒守护");
                break;
            default:
                Debug.Log($"安卡十字等级 {newLevel}：未定义效果");
                break;
        }
    }
} 