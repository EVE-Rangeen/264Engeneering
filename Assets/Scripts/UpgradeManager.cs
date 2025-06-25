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

        Debug.Log($"已随机选择3个武器: {selectedWeapon1.WeaponName}, {selectedWeapon2.WeaponName}, {selectedWeapon3.WeaponName}");
    }
} 