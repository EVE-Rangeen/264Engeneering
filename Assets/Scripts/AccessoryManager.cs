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
    /// <param name="oldLevel">旧等级</param>
    public void ExecuteAccessorySpecialEffect(AccessoryData accessoryData, int oldLevel)
    {
        if (accessoryData == null)
        {
            Debug.LogWarning("饰品数据为空，无法执行特殊效果");
            return;
        }

        switch (accessoryData.AccessoryName)
        {
            case "弹巢":
                HandleAmmoNestEffect(oldLevel);
                break;
            case "重靴":
                HandleHeavyBootsEffect(oldLevel);
                break;
            case "安卡十字":
                HandleAnkhCrossEffect(oldLevel);
                break;
            case "沙漏":
                HandleHourglassEffect(oldLevel);
                break;
            case "血袋":
                HandleBloodBagEffect(oldLevel);
                break;
            case "小石头":
                HandleSmallStoneEffect(oldLevel);
                break;
            default:
                Debug.LogWarning($"未找到饰品 '{accessoryData.AccessoryName}' 的特殊效果处理方法");
                break;
        }
    }

    /// <summary>
    /// 处理弹巢特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleAmmoNestEffect(int oldLevel)
    {
        switch (oldLevel)
        {
            case 0:
                Debug.Log("弹巢从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加射弹数量+1
                PlayerAttributeManager.instance.PlayerComponent.ProjectileAmountIncrement += 1;
                break;
            case 2:
                // LV2: 增加射弹数量+1
                PlayerAttributeManager.instance.PlayerComponent.ProjectileAmountIncrement += 1;
                break;
            case 3:
                // LV3: 增加射弹数量+1（最大等级）
                PlayerAttributeManager.instance.PlayerComponent.ProjectileAmountIncrement += 1;
                break;
            default:
                Debug.Log($"弹巢等级 {oldLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理重靴特殊效果
    /// 每级：护甲+1，移动速度+0.05（最大等级5）
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleHeavyBootsEffect(int oldLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("重靴从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 护甲+1，移动速度+0.05
                player.Armor += 1;
                player.MoveSpeed += 0.05f;
                break;
            case 2:
                // LV2: 护甲+1，移动速度+0.05
                player.Armor += 1;
                player.MoveSpeed += 0.05f;
                break;
            case 3:
                // LV3: 护甲+1，移动速度+0.05
                player.Armor += 1;
                player.MoveSpeed += 0.05f;
                break;
            default:
                Debug.Log($"重靴等级 {oldLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理安卡十字特殊效果
    /// 每级：生命回复+0.2（最大等级5）
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleAnkhCrossEffect(int oldLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("安卡十字从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 生命回复+0.2
                player.Recovery += 0.2f;
                break;
            case 2:
                // LV2: 生命回复+0.2
                player.Recovery += 0.2f;
                break;
            case 3:
                // LV3: 生命回复+0.2
                player.Recovery += 0.2f;
                break;
            case 4:
                // LV4: 生命回复+0.2
                player.Recovery += 0.2f;
                break;
            case 5:
                // LV5: 生命回复+0.2（最大等级）
                player.Recovery += 0.2f;
                break;
            default:
                Debug.Log($"安卡十字等级 {oldLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理沙漏特殊效果
    /// 每级：增加8%冷却时间（最大等级5）
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleHourglassEffect(int oldLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("沙漏从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加8%冷却时间
                player.CooldownReductionFactor += 0.08f;
                break;
            case 2:
                // LV2: 增加8%冷却时间
                player.CooldownReductionFactor += 0.08f;
                break;
            case 3:
                // LV3: 增加8%冷却时间
                player.CooldownReductionFactor += 0.08f;
                break;
            case 4:
                // LV4: 增加8%冷却时间
                player.CooldownReductionFactor += 0.08f;
                break;
            case 5:
                // LV5: 增加8%冷却时间（最大等级）
                player.CooldownReductionFactor += 0.08f;
                break;
            default:
                Debug.Log($"沙漏等级 {oldLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理血袋特殊效果
    /// 每级：增加10%持续时间（最大等级5）
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleBloodBagEffect(int oldLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("血袋从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加10%持续时间
                player.WeaponDurationFactor += 0.1f;
                break;
            case 2:
                // LV2: 增加10%持续时间
                player.WeaponDurationFactor += 0.1f;
                break;
            case 3:
                // LV3: 增加10%持续时间
                player.WeaponDurationFactor += 0.1f;
                break;
            case 4:
                // LV4: 增加10%持续时间
                player.WeaponDurationFactor += 0.1f;
                break;
            case 5:
                // LV5: 增加10%持续时间（最大等级）
                player.WeaponDurationFactor += 0.1f;
                break;
            default:
                Debug.Log($"血袋等级 {oldLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理小石头特殊效果
    /// 每级：增加10%攻击力（最大等级5）
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleSmallStoneEffect(int oldLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("小石头从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加10%攻击力
                player.PowerFactor += 0.1f;
                break;
            case 2:
                // LV2: 增加10%攻击力
                player.PowerFactor += 0.1f;
                break;
            case 3:
                // LV3: 增加10%攻击力
                player.PowerFactor += 0.1f;
                break;
            case 4:
                // LV4: 增加10%攻击力
                player.PowerFactor += 0.1f;
                break;
            case 5:
                // LV5: 增加10%攻击力（最大等级）
                player.PowerFactor += 0.1f;
                break;
            default:
                Debug.Log($"小石头等级 {oldLevel}：已达到最大等级或无效等级");
                break;
        }
    }
} 