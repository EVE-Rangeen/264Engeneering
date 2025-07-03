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
            case "沙漏":
                HandleHourglassEffect(newLevel);
                break;
            case "血袋":
                HandleBloodBagEffect(newLevel);
                break;
            case "小石头":
                HandleSmallStoneEffect(newLevel);
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
                Debug.Log($"弹巢等级 {newLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理重靴特殊效果
    /// 每级：护甲+1，移动速度+0.05（最大等级5）
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleHeavyBootsEffect(int newLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (newLevel)
        {
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
                Debug.Log($"重靴等级 {newLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理安卡十字特殊效果
    /// 每级：生命回复+0.2（最大等级5）
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleAnkhCrossEffect(int newLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (newLevel)
        {
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
                Debug.Log($"安卡十字等级 {newLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理沙漏特殊效果
    /// 每级：增加8%冷却时间（最大等级5）
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleHourglassEffect(int newLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (newLevel)
        {
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
                Debug.Log($"沙漏等级 {newLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理血袋特殊效果
    /// 每级：增加10%持续时间（最大等级5）
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleBloodBagEffect(int newLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (newLevel)
        {
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
                Debug.Log($"血袋等级 {newLevel}：已达到最大等级或无效等级");
                break;
        }
    }

    /// <summary>
    /// 处理小石头特殊效果
    /// 每级：增加10%攻击力（最大等级5）
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleSmallStoneEffect(int newLevel)
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        switch (newLevel)
        {
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
                Debug.Log($"小石头等级 {newLevel}：已达到最大等级或无效等级");
                break;
        }
    }
} 