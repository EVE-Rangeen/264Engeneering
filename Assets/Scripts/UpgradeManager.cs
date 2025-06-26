using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 升级选项管理器
/// 用于管理游戏中的升级选项界面
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;
    [Header("升级选项对象")]
    [SerializeField] private GameObject _upgradeOption1;
    [SerializeField] private GameObject _upgradeOption2;
    [SerializeField] private GameObject _upgradeOption3;

    [Header("武器图标生成器")]
    [SerializeField] private WeaponIconGenerator _weaponIconGenerator;

    // 存储当前随机选择的3个武器
    private WeaponData _currentSelectedWeapon1;
    private WeaponData _currentSelectedWeapon2;
    private WeaponData _currentSelectedWeapon3;

    // 存储当前随机选择的3个武器在列表中的索引
    private int _currentSelectedIndex1;
    private int _currentSelectedIndex2;
    private int _currentSelectedIndex3;

    /// <summary>
    /// 升级选项1
    /// </summary>
    public GameObject UpgradeOption1 => _upgradeOption1;

    /// <summary>
    /// 升级选项2
    /// </summary>
    public GameObject UpgradeOption2 => _upgradeOption2;

    /// <summary>
    /// 升级选项3
    /// </summary>
    public GameObject UpgradeOption3 => _upgradeOption3;

    /// <summary>
    /// 当前选中的武器1
    /// </summary>
    public WeaponData CurrentSelectedWeapon1 => _currentSelectedWeapon1;

    /// <summary>
    /// 当前选中的武器2
    /// </summary>
    public WeaponData CurrentSelectedWeapon2 => _currentSelectedWeapon2;

    /// <summary>
    /// 当前选中的武器3
    /// </summary>
    public WeaponData CurrentSelectedWeapon3 => _currentSelectedWeapon3;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 从当前武器中随机选择3个武器并更新升级选项显示
    /// </summary>
    public void SelectRandomWeapons()
    {
        if (WeaponManager.instance == null)
        {
            Debug.LogError("WeaponManager实例不存在！");
            return;
        }

        var currentWeapons = WeaponManager.instance.CurrentWeapons;
        
        if (currentWeapons.Count < 3)
        {
            Debug.LogWarning($"当前武器数量不足3个，只有 {currentWeapons.Count} 个武器");
            return;
        }

        // 创建武器索引列表用于随机选择
        List<int> weaponIndices = new List<int>();
        for (int i = 0; i < currentWeapons.Count; i++)
        {
            weaponIndices.Add(i);
        }

        // 随机打乱索引列表
        for (int i = 0; i < weaponIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, weaponIndices.Count);
            int temp = weaponIndices[i];
            weaponIndices[i] = weaponIndices[randomIndex];
            weaponIndices[randomIndex] = temp;
        }

        // 选择前3个武器并分配给升级选项
        WeaponData selectedWeapon1 = currentWeapons[weaponIndices[0]];
        WeaponData selectedWeapon2 = currentWeapons[weaponIndices[1]];
        WeaponData selectedWeapon3 = currentWeapons[weaponIndices[2]];

        // 存储当前选择的武器
        _currentSelectedWeapon1 = selectedWeapon1;
        _currentSelectedWeapon2 = selectedWeapon2;
        _currentSelectedWeapon3 = selectedWeapon3;

        // 存储当前选择的武器在列表中的索引
        _currentSelectedIndex1 = weaponIndices[0];
        _currentSelectedIndex2 = weaponIndices[1];
        _currentSelectedIndex3 = weaponIndices[2];

        // 调用各个升级选项按钮的UpdateButtonDisplay方法
        if (_upgradeOption1 != null)
        {
            LevelUpSelectionButton button1 = _upgradeOption1.GetComponent<LevelUpSelectionButton>();
            if (button1 != null)
            {
                button1.UpdateButtonDisplay(selectedWeapon1);
            }
            else
            {
                Debug.LogWarning("升级选项1没有LevelUpSelectionButton组件");
            }
        }

        if (_upgradeOption2 != null)
        {
            LevelUpSelectionButton button2 = _upgradeOption2.GetComponent<LevelUpSelectionButton>();
            if (button2 != null)
            {
                button2.UpdateButtonDisplay(selectedWeapon2);
            }
            else
            {
                Debug.LogWarning("升级选项2没有LevelUpSelectionButton组件");
            }
        }

        if (_upgradeOption3 != null)
        {
            LevelUpSelectionButton button3 = _upgradeOption3.GetComponent<LevelUpSelectionButton>();
            if (button3 != null)
            {
                button3.UpdateButtonDisplay(selectedWeapon3);
            }
            else
            {
                Debug.LogWarning("升级选项3没有LevelUpSelectionButton组件");
            }
        }
    }

    /// <summary>
    /// 当玩家选择升级选项时触发
    /// </summary>
    /// <param name="buttonIndex">按钮索引 (1, 2, 3)</param>
    public void OnUpgradeSelected(int buttonIndex)
    {
        WeaponData selectedWeapon = null;
        int weaponListIndex = -1;
        
        switch (buttonIndex)
        {
            case 1:
                selectedWeapon = _currentSelectedWeapon1;
                weaponListIndex = _currentSelectedIndex1;
                break;
            case 2:
                selectedWeapon = _currentSelectedWeapon2;
                weaponListIndex = _currentSelectedIndex2;
                break;
            case 3:
                selectedWeapon = _currentSelectedWeapon3;
                weaponListIndex = _currentSelectedIndex3;
                break;
            default:
                Debug.LogError($"无效的按钮索引: {buttonIndex}");
                return;
        }

        if (selectedWeapon == null || weaponListIndex < 0)
        {
            Debug.LogError("选中的武器为空或索引无效！");
            return;
        }

        // 直接使用索引获取WeaponManager中的武器实例并升级
        WeaponData weaponInList = WeaponManager.instance.GetCurrentWeapon(weaponListIndex);
        if (weaponInList != null)
        {
            // 使用反射直接修改私有字段 _currentLevel
            var field = typeof(WeaponData).GetField("_currentLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                int oldLevel = (int)field.GetValue(weaponInList);
                field.SetValue(weaponInList, oldLevel + 1);
                
                // 只有当武器从等级0升级时才添加图标（第一次获得武器）
                if (oldLevel == 0)
                {
                    // 添加武器图标到UI
                    _weaponIconGenerator.AddWeaponIcon(weaponInList, $"UpgradedWeapon_{weaponInList.WeaponName}");
                }
            }
        }
        else
        {
            Debug.LogError($"无法获取索引为 {weaponListIndex} 的武器");
        }

        // 关闭升级面板
        UIController.instance.levelUpPanel.SetActive(false);
        UIController.instance.pauseButton.interactable = true;
        
        // 更新显示文本
        UIController.instance.UpdateWeaponLevelDisplay();
        UIController.instance.UpdatePlayerAttributeDisplay();

        // 恢复游戏时间
        Timer.instance.ResumeTimer();
    }
} 