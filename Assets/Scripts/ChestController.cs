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
    /// 开启宝箱（总调用方法）
    /// 根据概率决定升级1项还是3项
    /// </summary>
    public void OpenChestWithProbability()
    {
        // 根据概率决定升级数量
        float randomValue = Random.Range(0f, 1f);
        int upgradeCount;
        
        if (randomValue <= _tripleUpgradeChance)
        {
            // 幸运！升级多个项目
            upgradeCount = _bonusUpgradeCount;
            Debug.Log($"宝箱开启！随机值: {randomValue:F3} ≤ 概率阈值: {_tripleUpgradeChance:F3} → 幸运奖励，将升级 {upgradeCount} 个项目");
        }
        else
        {
            // 普通奖励
            upgradeCount = _normalUpgradeCount;
            Debug.Log($"宝箱开启！随机值: {randomValue:F3} > 概率阈值: {_tripleUpgradeChance:F3} → 普通奖励，将升级 {upgradeCount} 个项目");
        }
        
        // 调用具体的开箱方法
        OpenChest(upgradeCount);
    }

    /// <summary>
    /// 开启宝箱
    /// </summary>
    /// <param name="upgradeCount">要升级的项目数量</param>
    public void OpenChest(int upgradeCount)
    {
        // 从武器和饰品池中随机选择项目进行升级
        List<UpgradeItem> selectedItems = SelectRandomUpgradeItems(upgradeCount);
        
        // 执行升级
        foreach (var item in selectedItems)
        {
            ExecuteUpgrade(item);
        }
    }

    /// <summary>
    /// 从武器和饰品池中随机选择指定数量的升级项目
    /// 参考 UpgradeManager.SelectRandomUpgradeItems 的逻辑
    /// </summary>
    /// <param name="count">要选择的项目数量</param>
    /// <returns>选中的升级项目列表</returns>
    private List<UpgradeItem> SelectRandomUpgradeItems(int count)
    {
        List<UpgradeItem> selectedItems = new List<UpgradeItem>();
        
        if (WeaponManager.instance == null || AccessoryManager.instance == null)
        {
            Debug.LogError("WeaponManager 或 AccessoryManager 实例不存在！");
            return selectedItems;
        }

        // 收集所有可升级的项目（排除满级项目）
        List<UpgradeItem> allUpgradeItems = new List<UpgradeItem>();
        
        // 添加未满级的武器
        var currentWeapons = WeaponManager.instance.CurrentWeapons;
        for (int i = 0; i < currentWeapons.Count; i++)
        {
            if (!currentWeapons[i].IsMaxLevel)
            {
                allUpgradeItems.Add(new UpgradeItem(currentWeapons[i], i));
            }
        }
        
        // 添加未满级的饰品
        var currentAccessories = AccessoryManager.instance.CurrentAccessories;
        for (int i = 0; i < currentAccessories.Count; i++)
        {
            if (!currentAccessories[i].IsMaxLevel)
            {
                allUpgradeItems.Add(new UpgradeItem(currentAccessories[i], i));
            }
        }
        
        if (allUpgradeItems.Count == 0)
        {
            Debug.Log("没有可升级的项目！");
            return selectedItems;
        }

        if (allUpgradeItems.Count < count)
        {
            Debug.LogWarning($"可升级项目数量不足 {count} 个，只有 {allUpgradeItems.Count} 个项目（已排除满级项目）");
            count = allUpgradeItems.Count;
        }

        // 随机打乱升级项目列表（参考 UpgradeManager 的逻辑）
        for (int i = 0; i < allUpgradeItems.Count; i++)
        {
            int randomIndex = Random.Range(i, allUpgradeItems.Count);
            UpgradeItem temp = allUpgradeItems[i];
            allUpgradeItems[i] = allUpgradeItems[randomIndex];
            allUpgradeItems[randomIndex] = temp;
        }

        // 选择前 count 个项目
        for (int i = 0; i < count; i++)
        {
            selectedItems.Add(allUpgradeItems[i]);
        }

        return selectedItems;
    }

    /// <summary>
    /// 执行升级
    /// 直接调用 UpgradeManager 的公有升级方法
    /// </summary>
    /// <param name="upgradeItem">要升级的项目</param>
    private void ExecuteUpgrade(UpgradeItem upgradeItem)
    {
        if (upgradeItem.ItemType == UpgradeItemType.Weapon)
        {
            UpgradeManager.instance.UpgradeWeapon(upgradeItem);
        }
        else if (upgradeItem.ItemType == UpgradeItemType.Accessory)
        {
            UpgradeManager.instance.UpgradeAccessory(upgradeItem);
        }
    }
} 