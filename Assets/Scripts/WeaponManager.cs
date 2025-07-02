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
    /// 获取当前武器实例列表（可读写）
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
    /// <param name="oldLevel">旧等级</param>
    public void ExecuteWeaponSpecialEffect(WeaponData weaponData, int oldLevel)
    {
        if (weaponData == null)
        {
            Debug.LogWarning("武器数据为空，无法执行特殊效果");
            return;
        }

        switch (weaponData.WeaponName)
        {
            case "猎魔手枪":
                HandleDemonHunterPistolEffect(oldLevel);
                break;
            case "大剑":
                HandleGreatSwordEffect(oldLevel);
                break;
            case "神罚小刀":
                HandleDivineKnifeEffect(oldLevel);
                break;
            case "精灵之火":
                HandleSpiritFireEffect(oldLevel);
                break;
            case "火焰魔杖":
                HandleFlameWandEffect(oldLevel);
                break;
            case "火球术":
                HandleFireballEffect(oldLevel);
                break;
            default:
                Debug.LogWarning($"未找到武器 '{weaponData.WeaponName}' 的特殊效果处理方法");
                break;
        }
    }

    /// <summary>
    /// 处理猎魔手枪特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleDemonHunterPistolEffect(int oldLevel)
    {
        // 获取猎魔手枪的当前武器实例（假设是第一个武器）
        WeaponData demonHunterPistol = _currentWeapons[0];
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("猎魔手枪从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加基础伤害4
                demonHunterPistol.Damage += 4f;
                break;
            case 2:
                // LV2: 减少冷却间隔-0.3
                demonHunterPistol.CooldownTime -= 0.3f;
                break;
            case 3:
                // LV3: 增加基础伤害4
                demonHunterPistol.Damage += 4f;
                break;
            case 4:
                // LV4: 减少冷却间隔-0.3
                demonHunterPistol.CooldownTime -= 0.3f;
                break;
            case 5:
                // LV5: 增加基础伤害4
                demonHunterPistol.Damage += 4f;
                break;
            case 6:
                // LV6: 减少冷却间隔-0.3
                demonHunterPistol.CooldownTime -= 0.3f;
                break;
            case 7:
                // LV7: 增加基础伤害4
                demonHunterPistol.Damage += 4f;
                break;
            default:
                Debug.Log($"猎魔手枪等级 {oldLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理大剑特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleGreatSwordEffect(int oldLevel)
    {
        // 获取大剑的当前武器实例（假设是第二个武器）
        WeaponData greatSword = _currentWeapons[1];
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("大剑从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加基础伤害5点，特殊攻击倍率+0.1
                greatSword.Damage += 5f;
                greatSword.SpecialAttackMultiplier += 0.1f;
                break;
            case 2:
                // LV2: 增加基础伤害5点
                greatSword.Damage += 5f;
                break;
            case 3:
                // LV3: 减小冷却时间0.2
                greatSword.CooldownTime -= 0.2f;
                break;
            case 4:
                // LV4: 增加基础伤害5点，特殊攻击倍率+0.1
                greatSword.Damage += 5f;
                greatSword.SpecialAttackMultiplier += 0.1f;
                break;
            case 5:
                // LV5: 增加基础伤害5点
                greatSword.Damage += 5f;
                break;
            case 6:
                // LV6: 减小冷却时间0.2
                greatSword.CooldownTime -= 0.2f;
                break;
            case 7:
                // LV7: 增加基础伤害5点，特殊攻击倍率+0.2
                greatSword.Damage += 5f;
                greatSword.SpecialAttackMultiplier += 0.2f;
                break;
            default:
                Debug.Log($"大剑等级 {oldLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理神罚小刀特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleDivineKnifeEffect(int oldLevel)
    {
        // 获取神罚小刀的当前武器实例（假设是第三个武器）
        WeaponData divineKnife = _currentWeapons[2];
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("神罚小刀从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            case 2:
                // LV2: 增大范围+1，基础伤害增加10点
                divineKnife.AttackRange += 1f;
                divineKnife.Damage += 10f;
                break;
            case 3:
                // LV3: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            case 4:
                // LV4: 增大范围+1，基础伤害增加10点
                divineKnife.AttackRange += 1f;
                divineKnife.Damage += 10f;
                break;
            case 5:
                // LV5: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            case 6:
                // LV6: 增大范围+1，基础伤害增加10点
                divineKnife.AttackRange += 1f;
                divineKnife.Damage += 10f;
                break;
            case 7:
                // LV7: 增加一个数量
                divineKnife.AttackCount += 1;
                break;
            default:
                Debug.Log($"神罚小刀等级 {oldLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理精灵之火特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleSpiritFireEffect(int oldLevel)
    {
        // 查找精灵之火武器实例
        WeaponData spiritFire = _currentWeapons.Find(w => w.WeaponName == "精灵之火");
        if (spiritFire == null)
        {
            Debug.LogWarning("未找到精灵之火武器实例");
            return;
        }
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("精灵之火从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 增加一个数量
                spiritFire.AttackCount += 1;
                break;
            case 2:
                // LV2: 攻击范围增加0.3，飞行速度增加0.5，射弹间隔减少0.2
                spiritFire.AttackRange += 0.3f;
                spiritFire.ProjectileSpeed += 0.5f;
                spiritFire.BulletInterval -= 0.2f;
                break;
            case 3:
                // LV3: 持续时间增加1，基础伤害增加7点
                spiritFire.Duration += 1f;
                spiritFire.Damage += 7f;
                break;
            case 4:
                // LV4: 增加一个数量
                spiritFire.AttackCount += 1;
                break;
            case 5:
                // LV5: 攻击范围增加0.3，飞行速度增加0.5
                spiritFire.AttackRange += 0.3f;
                spiritFire.ProjectileSpeed += 0.5f;
                break;
            case 6:
                // LV6: 持续时间增加1，基础伤害增加7点，射弹间隔减少0.2
                spiritFire.Duration += 1f;
                spiritFire.Damage += 7f;
                spiritFire.BulletInterval -= 0.2f;
                break;
            case 7:
                // LV7: 增加一个数量
                spiritFire.AttackCount += 1;
                break;
            default:
                Debug.Log($"精灵之火等级 {oldLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理火焰魔杖特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleFlameWandEffect(int oldLevel)
    {
        // 查找火焰魔杖武器实例
        WeaponData flameWand = _currentWeapons.Find(w => w.WeaponName == "火焰魔杖");
        if (flameWand == null)
        {
            Debug.LogWarning("未找到火焰魔杖武器实例");
            return;
        }
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("火焰魔杖从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 持续时间增加1s
                flameWand.Duration += 1f;
                break;
            case 2:
                // LV2: 增加基础伤害3，增大范围+1
                flameWand.Damage += 3f;
                flameWand.AttackRange += 1f;
                break;
            case 3:
                // LV3: 持续时间增加1s
                flameWand.Duration += 1f;
                break;
            case 4:
                // LV4: 增加基础伤害7
                flameWand.Damage += 7f;
                break;
            case 5:
                // LV5: 增加基础伤害3，增大范围+1
                flameWand.Damage += 3f;
                flameWand.AttackRange += 1f;
                break;
            case 6:
                // LV6: 持续时间增加1s
                flameWand.Duration += 1f;
                break;
            case 7:
                // LV7: 增加基础伤害7
                flameWand.Damage += 7f;
                break;
            default:
                Debug.Log($"火焰魔杖等级 {oldLevel}：未定义效果");
                break;
        }
    }

    /// <summary>
    /// 处理火球术特殊效果
    /// </summary>
    /// <param name="oldLevel">旧等级</param>
    private void HandleFireballEffect(int oldLevel)
    {
        // 查找火球术武器实例
        WeaponData fireball = _currentWeapons.Find(w => w.WeaponName == "火球术");
        if (fireball == null)
        {
            Debug.LogWarning("未找到火球术武器实例");
            return;
        }
        
        switch (oldLevel)
        {
            case 0:
                Debug.Log("火球术从等级0升级，应用初始效果");
                break;
            case 1:
                // LV1: 持续时间增加1s
                fireball.Duration += 1f;
                break;
            case 2:
                // LV2: 增加基础伤害5，增大范围+1
                fireball.Damage += 5f;
                fireball.AttackRange += 1f;
                break;
            case 3:
                // LV3: 减少冷却0.5s
                fireball.CooldownTime -= 0.5f;
                break;
            case 4:
                // LV4: 持续时间增加1s
                fireball.Duration += 1f;
                break;
            case 5:
                // LV5: 增加基础伤害5，增大范围+1
                fireball.Damage += 5f;
                fireball.AttackRange += 1f;
                break;
            case 6:
                // LV6: 减少冷却0.5s
                fireball.CooldownTime -= 0.5f;
                break;
            case 7:
                // LV7: 增加基础伤害7，减少冷却1s
                fireball.Damage += 7f;
                fireball.CooldownTime -= 1f;
                break;
            default:
                Debug.Log($"火球术等级 {oldLevel}：未定义效果");
                break;
        }
    }
} 