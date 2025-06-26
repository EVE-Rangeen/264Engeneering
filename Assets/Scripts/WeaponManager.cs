/// <summary>
/// 谈恩萁创建
/// </summary>
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
    public List<WeaponData> CurrentWeapons => _currentWeapons;

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
            }
            else
            {
                Debug.LogWarning("WeaponDataList中包含空的WeaponData引用");
            }
        }
    }

    /// <summary>
    /// 根据索引获取当前武器实例
    /// </summary>
    /// <param name="index">武器索引</param>
    /// <returns>武器实例，如果索引无效返回null</returns>
    public WeaponData GetSelectedWeapon(int index)
    {
        if (index >= 0 && index < _currentWeapons.Count)
        {
            return _currentWeapons[index];
        }
        
        Debug.LogWarning($"无效的当前武器索引: {index}");
        return null;
    }

    /// <summary>
    /// 执行武器特殊效果
    /// </summary>
    /// <param name="weaponData">武器数据</param>
    /// <param name="newLevel">新等级</param>
    public void ExecuteWeaponSpecialEffect(WeaponData weaponData, int newLevel)
    {
        if (weaponData == null)
        {
            Debug.LogWarning("武器数据为空，无法执行特殊效果");
            return;
        }

        switch (weaponData.WeaponName)
        {
            case "猎魔手枪":
                HandleDemonHunterPistolEffect(newLevel);
                break;
            case "大剑":
                HandleGreatSwordEffect(newLevel);
                break;
            case "神罚小刀":
                HandleDivineKnifeEffect(newLevel);
                break;
            default:
                Debug.LogWarning($"未找到武器 '{weaponData.WeaponName}' 的特殊效果处理方法");
                break;
        }
    }

    /// <summary>
    /// 处理猎魔手枪特殊效果
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleDemonHunterPistolEffect(int newLevel)
    {
        // 获取猎魔手枪的当前武器实例（假设是第一个武器）
        WeaponData demonHunterPistol = _currentWeapons[0];
        
        switch (newLevel)
        {
            case 1:
                // LV1: 增加基础伤害3
                demonHunterPistol.Damage += 3f;
                break;
            case 2:
                // LV2: 穿透增加2
                demonHunterPistol.Piercing += 2;
                break;
            case 3:
                // LV3: 增加基础伤害3
                demonHunterPistol.Damage += 3f;
                break;
            case 4:
                // LV4: 穿透增加2
                demonHunterPistol.Piercing += 2;
                break;
            case 5:
                // LV5: 增加基础伤害3，增加飞行速度50%
                demonHunterPistol.Damage += 3f;
                demonHunterPistol.ProjectileSpeed *= 1.5f;
                break;
            case 6:
                // LV6: 穿透增加2
                demonHunterPistol.Piercing += 2;
                break;
            case 7:
                // LV7: 增加基础伤害3，增加飞行速度50%
                demonHunterPistol.Damage += 3f;
                demonHunterPistol.ProjectileSpeed *= 1.5f;
                break;
            default:
                Debug.Log($"猎魔手枪等级 {newLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理大剑特殊效果
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleGreatSwordEffect(int newLevel)
    {
        // 获取大剑的当前武器实例（假设是第二个武器）
        WeaponData greatSword = _currentWeapons[1];
        
        switch (newLevel)
        {
            case 1:
                // LV1: 增加基础伤害5点，增大攻击范围10%
                greatSword.Damage += 5f;
                greatSword.AttackRange *= 1.1f;
                break;
            case 2:
                // LV2: 增加基础伤害5点
                greatSword.Damage += 5f;
                break;
            case 3:
                // LV3: 减小冷却时间0.2f
                greatSword.BulletInterval *= 0.2f;
                break;
            case 4:
                // LV4: 增加基础伤害5点，增大攻击范围10%
                greatSword.Damage += 5f;
                greatSword.AttackRange *= 1.1f;
                break;
            case 5:
                // LV5: 增加基础伤害5点
                greatSword.Damage += 5f;
                break;
            case 6:
                // LV6: 减小冷却时间0.2f
                greatSword.BulletInterval *= 0.2f;
                break;
            case 7:
                // LV7: 增加基础伤害5点，增大攻击范围10%
                greatSword.Damage += 5f;
                greatSword.AttackRange *= 1.1f;
                break;
            default:
                Debug.Log($"大剑等级 {newLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理神罚小刀特殊效果
    /// </summary>
    /// <param name="newLevel">新等级</param>
    private void HandleDivineKnifeEffect(int newLevel)
    {
        // 获取神罚小刀的当前武器实例（假设是第三个武器）
        WeaponData divineKnife = _currentWeapons[2];
        
        switch (newLevel)
        {
            case 1:
                // LV1: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            case 2:
                // LV2: 增大范围100%，基础伤害增加10点
                divineKnife.AttackRange *= 2.0f;
                divineKnife.Damage += 10f;
                break;
            case 3:
                // LV3: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            case 4:
                // LV4: 增大范围100%，基础伤害增加10点
                divineKnife.AttackRange *= 2.0f;
                divineKnife.Damage += 10f;
                break;
            case 5:
                // LV5: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            case 6:
                // LV6: 增大范围100%，基础伤害增加10点
                divineKnife.AttackRange *= 2.0f;
                divineKnife.Damage += 10f;
                break;
            case 7:
                // LV7: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            default:
                Debug.Log($"神罚小刀等级 {newLevel}：未定义效果");
                break;
        }
    }
} 