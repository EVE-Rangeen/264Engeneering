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
        switch (newLevel)
        {
            case 1:
                Debug.Log("猎魔手枪等级1：基础射击");
                break;
            case 2:
                Debug.Log("猎魔手枪等级2：提升射速");
                break;
            case 3:
                Debug.Log("猎魔手枪等级3：增加穿刺");
                break;
            case 4:
                Debug.Log("猎魔手枪等级4：双重射击");
                break;
            case 5:
                Debug.Log("猎魔手枪等级5：爆炸子弹");
                break;
            case 6:
                Debug.Log("猎魔手枪等级6：追踪弹药");
                break;
            case 7:
                Debug.Log("猎魔手枪等级7：终极连射");
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
        switch (newLevel)
        {
            case 1:
                Debug.Log("大剑等级1：基础挥砍");
                break;
            case 2:
                Debug.Log("大剑等级2：增加伤害");
                break;
            case 3:
                Debug.Log("大剑等级3：扩大范围");
                break;
            case 4:
                Debug.Log("大剑等级4：旋风斩击");
                break;
            case 5:
                Debug.Log("大剑等级5：震地波");
                break;
            case 6:
                Debug.Log("大剑等级6：剑气斩");
                break;
            case 7:
                Debug.Log("大剑等级7：终极剑舞");
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
        switch (newLevel)
        {
            case 1:
                Debug.Log("神罚小刀等级1：基础投掷");
                break;
            case 2:
                Debug.Log("神罚小刀等级2：增加数量");
                break;
            case 3:
                Debug.Log("神罚小刀等级3：回旋飞刀");
                break;
            case 4:
                Debug.Log("神罚小刀等级4：多重投射");
                break;
            case 5:
                Debug.Log("神罚小刀等级5：神圣之光");
                break;
            case 6:
                Debug.Log("神罚小刀等级6：追击模式");
                break;
            case 7:
                Debug.Log("神罚小刀等级7：天罚之刃");
                break;
            default:
                Debug.Log($"神罚小刀等级 {newLevel}：未定义效果");
                break;
        }
    }
} 