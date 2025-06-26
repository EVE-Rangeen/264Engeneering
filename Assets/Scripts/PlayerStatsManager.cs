using UnityEngine;
using static ES3;
using System.Collections.Generic;

/// <summary>
/// 人物属性管理器
/// 管理玩家的各种属性和升级
/// </summary>
public class PlayerStatsManager : MonoBehaviour
{
    /// <summary>
    /// 人物属性管理器单例实例
    /// </summary>
    public static PlayerStatsManager instance;
    [SerializeField] private int _totalCoins = 0;

    [Header("属性升级数据")]
    [SerializeField] private List<AttributeUpgradeData> _upgradeableAttributes = new List<AttributeUpgradeData>();

    public int TotalCoins => _totalCoins;

    /// <summary>
    /// 获取所有可升级属性数据（只读）
    /// </summary>
    public IReadOnlyList<AttributeUpgradeData> UpgradeableAttributes => _upgradeableAttributes;

    /// <summary>
    /// 初始化单例
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LoadStatsFromSave();
        SaveAllData();
        
        // 更新UI显示
        if (StatsUIController.instance != null)
        {
            StatsUIController.instance.UpdateCoin();
            
            // 更新所有属性的等级和描述文本
            for (int i = 0; i < _upgradeableAttributes.Count; i++)
            {
                StatsUIController.instance.UpdateLevelTexts(i);
                StatsUIController.instance.UpdateDescriptionTexts(i);
            }
        }
    }

    /// <summary>
    /// 从存档加载所有属性数据
    /// </summary>
    private void LoadStatsFromSave()
    {
        try
        {
            // 加载金币数据
            _totalCoins = Load("TotalCoins", _totalCoins);
            
            // 加载每个属性的数据
            for (int i = 0; i < _upgradeableAttributes.Count; i++)
            {
                string levelKey = $"UpgradeableAttributes.Attribute_{i}.Level";
                int savedLevel = Load(levelKey, 0);
                _upgradeableAttributes[i].SetCurrentLevel(savedLevel);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"加载存档数据时出现错误: {e.Message}");
            Debug.LogWarning("将使用默认值初始化所有属性");
        }
    }

    /// <summary>
    /// 保存所有数据（包括基础属性和可升级属性）
    /// </summary>
    private void SaveAllData()
    {
        try
        {
            // 保存金币数据
            Save("TotalCoins", _totalCoins);
            
            // 保存属性总数
            Save("UpgradeableAttributes.Count", _upgradeableAttributes.Count);
            
            // 为每个属性单独保存数据
            for (int i = 0; i < _upgradeableAttributes.Count; i++)
            {
                var attribute = _upgradeableAttributes[i];
                string baseKey = $"UpgradeableAttributes.Attribute_{i}";
                
                // 保存属性等级
                Save($"{baseKey}.Level", attribute.CurrentLevel);
                
                // 保存当前等级对应的UpgradeValues
                float currentUpgradeValue = attribute.UpgradeValues[attribute.CurrentLevel];
                Save($"{baseKey}.CurrentUpgradeValue", currentUpgradeValue);
                
                // 保存属性名称（用于调试和数据识别）
                Save($"{baseKey}.Name", attribute.AttributeName);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存数据时出现错误: {e.Message}");
        }
    }

    /// <summary>
    /// 花费金币
    /// </summary>
    /// <param name="amount">要花费的金币数量</param>
    /// <returns>是否花费成功</returns>
    public bool SpendCoins(int amount)
    {
        if (_totalCoins >= amount)
        {
            _totalCoins -= amount;
            
            try
            {
                Save("TotalCoins", _totalCoins);
                Debug.Log($"花费金币: {amount}, 剩余: {_totalCoins}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"保存金币数据时出现错误: {e.Message}");
                // 如果保存失败，回滚金币变更
                _totalCoins += amount;
                return false;
            }
            
            return true;
        }
        
        Debug.LogWarning($"金币不足！当前: {_totalCoins}, 需要: {amount}");
        return false;
    }

    /// <summary>
    /// 升级指定索引的属性
    /// </summary>
    /// <param name="attributeIndex">属性索引</param>
    /// <returns>是否升级成功</returns>
    public bool UpgradeAttribute(int attributeIndex)
    {
        if (attributeIndex >= 0 && attributeIndex < _upgradeableAttributes.Count)
        {
            var attribute = _upgradeableAttributes[attributeIndex];
            
            // 检查是否已达到最高等级
            if (attribute.IsMaxLevel)
            {
                Debug.LogWarning($"属性 {attribute.AttributeName} 已达到最高等级！");
                return false;
            }
            
            // 获取升级费用
            if (attribute.CurrentLevel < attribute.UpgradeCosts.Count)
            {
                int upgradeCost = attribute.UpgradeCosts[attribute.CurrentLevel];
                
                // 检查金币是否足够
                if (SpendCoins(upgradeCost))
                {
                    // 升级成功，使用内部方法修改等级
                    _upgradeableAttributes[attributeIndex].SetCurrentLevel(attribute.CurrentLevel + 1);
                    
                    // 保存升级后的属性数据
                    SaveAllData();
                    
                    return true;
                }
                else
                {
                    Debug.LogWarning($"金币不足！升级 {attribute.AttributeName} 需要 {upgradeCost} 金币");
                    return false;
                }
            }
        }
        
        return false;
    }
} 