/// <summary>
/// 谈恩萁创建
/// 资源管理器
/// 用于管理血瓶和疾走等游戏资源
/// </summary>
using UnityEngine;

/// <summary>
/// 资源管理器
/// 负责管理血瓶、疾走等游戏中的消耗性资源
/// </summary>
public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager instance;

    [Header("血瓶资源")]
    [SerializeField] private int _currentHealthPotions = 0;     // 当前血瓶数量
    [SerializeField] private int _maxHealthPotions = 5;         // 最大血瓶数量
    [SerializeField] private int _healthPotionHealPercent = 30; // 每个血瓶回复的血量百分比
    
    [Header("血瓶图标")]
    [SerializeField] private Sprite[] _healthPotionIcons;       // 血瓶图标列表，索引对应血瓶数量

    [Header("疾走资源")]
    [SerializeField] private int _currentDashCharges = 0;       // 当前疾走次数
    [SerializeField] private int _maxDashCharges = 3;           // 最大疾走次数

    /// <summary>
    /// 当前血瓶数量
    /// </summary>
    public int CurrentHealthPotions => _currentHealthPotions;

    /// <summary>
    /// 最大血瓶数量
    /// </summary>
    public int MaxHealthPotions => _maxHealthPotions;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIController.instance.healthPotionText.text = $"血瓶: {_currentHealthPotions}/{_maxHealthPotions}";
        UIController.instance.dashText.text = $"疾走: {_currentDashCharges}/{_maxDashCharges}";
        
        // 初始化血瓶图标
        UpdateHealthPotionIcon();
    }

    /// <summary>
    /// 检查是否可以使用疾走
    /// </summary>
    /// <returns>是否能够使用疾走</returns>
    public bool CanUseDash()
    {
        if (_currentDashCharges > 0)
        {
            _currentDashCharges--;
            UIController.instance.dashText.text = $"疾走: {_currentDashCharges}/{_maxDashCharges}";
            return true;
        }
        return false;
    }

    /// <summary>
    /// 添加血瓶
    /// </summary>
    public void AddHealthPotions()
    {
        if (_currentHealthPotions < _maxHealthPotions)
        {
            _currentHealthPotions++;
            UIController.instance.healthPotionText.text = $"血瓶: {_currentHealthPotions}/{_maxHealthPotions}";
            
            // 更新血瓶图标
            UpdateHealthPotionIcon();
        }
    }

    /// <summary>
    /// 添加疾走次数
    /// </summary>
    public void AddDashCharges()
    {
        if (_currentDashCharges < _maxDashCharges)
        {
            _currentDashCharges++;
            UIController.instance.dashText.text = $"疾走: {_currentDashCharges}/{_maxDashCharges}";
        }
    }

    /// <summary>
    /// 使用血瓶
    /// </summary>
    /// <returns>使用血瓶</returns>
    public void UseHealthPotion()
    {
        if (_currentHealthPotions > 0)
        {
            _currentHealthPotions--;
            UIController.instance.healthPotionText.text = $"血瓶: {_currentHealthPotions}/{_maxHealthPotions}";
            
            // 更新血瓶图标
            UpdateHealthPotionIcon();
        }
    }

    /// <summary>
    /// 更新血瓶图标
    /// </summary>
    private void UpdateHealthPotionIcon()
    {
        int iconIndex = _currentHealthPotions;
        
        // 更新UI图标
        UIController.instance.healthPotionIcon.sprite = _healthPotionIcons[iconIndex];
    }

    /// <summary>
    /// 获取血瓶回复百分比
    /// </summary>
    /// <returns>血瓶回复百分比</returns>
    public int GetHealthPotionHealPercent()
    {
        return _healthPotionHealPercent;
    }
} 