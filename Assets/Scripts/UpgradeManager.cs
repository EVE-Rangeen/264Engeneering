/// <summary>
/// 谈恩萁创建
/// </summary>
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 升级项目类型
/// </summary>
public enum UpgradeItemType
{
    Weapon,
    Accessory
}

/// <summary>
/// 升级项目数据
/// 统一包装武器和饰品数据
/// </summary>
public class UpgradeItem
{
    public UpgradeItemType ItemType { get; private set; }
    public WeaponData WeaponData { get; private set; }
    public AccessoryData AccessoryData { get; private set; }
    public int Index { get; private set; }

    public UpgradeItem(WeaponData weapon, int index)
    {
        ItemType = UpgradeItemType.Weapon;
        WeaponData = weapon;
        Index = index;
    }
    
    public UpgradeItem(AccessoryData accessory, int index)
    {
        ItemType = UpgradeItemType.Accessory;
        AccessoryData = accessory;
        Index = index;
    }
}

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

    [Header("图标生成器")]
    [SerializeField] private IconGenerator _weaponIconGenerator;
    [SerializeField] private IconGenerator _accessoryIconGenerator;

    // 当前选中的升级项目
    private UpgradeItem _currentSelectedItem1;
    private UpgradeItem _currentSelectedItem2;
    private UpgradeItem _currentSelectedItem3;

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
    /// 从当前武器和饰品中随机选择3个项目并更新升级选项显示
    /// </summary>
    public void SelectRandomUpgradeItems()
    {
        if (WeaponManager.instance == null || AccessoryManager.instance == null)
        {
            Debug.LogError("WeaponManager 或 AccessoryManager 实例不存在！");
            return;
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
        
        if (allUpgradeItems.Count < 3)
        {
            Debug.LogWarning($"可升级项目数量不足3个，只有 {allUpgradeItems.Count} 个项目（已排除满级项目）");
            
            // 如果可升级项目不足3个，可以考虑以下处理方式：
            // 1. 显示所有可升级项目
            // 2. 用空选项填充
            // 3. 显示特殊提示
            
            if (allUpgradeItems.Count == 0)
            {
                Debug.Log("所有武器和饰品都已满级！");
                return;
            }
        }

        // 随机打乱升级项目列表
        for (int i = 0; i < allUpgradeItems.Count; i++)
        {
            int randomIndex = Random.Range(i, allUpgradeItems.Count);
            UpgradeItem temp = allUpgradeItems[i];
            allUpgradeItems[i] = allUpgradeItems[randomIndex];
            allUpgradeItems[randomIndex] = temp;
        }

        // 选择前3个项目并分配给升级选项（如果不足3个，就选择所有可用的）
        _currentSelectedItem1 = allUpgradeItems.Count > 0 ? allUpgradeItems[0] : null;
        _currentSelectedItem2 = allUpgradeItems.Count > 1 ? allUpgradeItems[1] : null;
        _currentSelectedItem3 = allUpgradeItems.Count > 2 ? allUpgradeItems[2] : null;

        // 更新按钮显示
        UpdateButtonDisplay(_upgradeOption1, _currentSelectedItem1);
        UpdateButtonDisplay(_upgradeOption2, _currentSelectedItem2);
        UpdateButtonDisplay(_upgradeOption3, _currentSelectedItem3);
    }

    /// <summary>
    /// 只从当前武器中随机选择3个武器并更新升级选项显示
    /// </summary>
    public void SelectRandomWeapons()
    {
        if (WeaponManager.instance == null)
        {
            Debug.LogError("WeaponManager 实例不存在！");
            return;
        }

        // 收集所有可升级的武器（排除满级武器）
        List<UpgradeItem> weaponUpgradeItems = new List<UpgradeItem>();
        
        // 添加未满级的武器
        var currentWeapons = WeaponManager.instance.CurrentWeapons;
        for (int i = 0; i < currentWeapons.Count; i++)
        {
            if (!currentWeapons[i].IsMaxLevel)
            {
                weaponUpgradeItems.Add(new UpgradeItem(currentWeapons[i], i));
            }
        }
        
        if (weaponUpgradeItems.Count < 3)
        {
            Debug.LogWarning($"可升级武器数量不足3个，只有 {weaponUpgradeItems.Count} 个武器（已排除满级武器）");
            
            if (weaponUpgradeItems.Count == 0)
            {
                Debug.Log("所有武器都已满级！");
                return;
            }
        }

        // 随机打乱武器列表
        for (int i = 0; i < weaponUpgradeItems.Count; i++)
        {
            int randomIndex = Random.Range(i, weaponUpgradeItems.Count);
            UpgradeItem temp = weaponUpgradeItems[i];
            weaponUpgradeItems[i] = weaponUpgradeItems[randomIndex];
            weaponUpgradeItems[randomIndex] = temp;
        }

        // 选择前3个武器并分配给升级选项（如果不足3个，就选择所有可用的）
        _currentSelectedItem1 = weaponUpgradeItems.Count > 0 ? weaponUpgradeItems[0] : null;
        _currentSelectedItem2 = weaponUpgradeItems.Count > 1 ? weaponUpgradeItems[1] : null;
        _currentSelectedItem3 = weaponUpgradeItems.Count > 2 ? weaponUpgradeItems[2] : null;

        // 更新按钮显示
        UpdateButtonDisplay(_upgradeOption1, _currentSelectedItem1);
        UpdateButtonDisplay(_upgradeOption2, _currentSelectedItem2);
        UpdateButtonDisplay(_upgradeOption3, _currentSelectedItem3);
    }
    
    /// <summary>
    /// 更新按钮显示
    /// </summary>
    /// <param name="buttonObj">按钮对象</param>
    /// <param name="upgradeItem">升级项目</param>
    private void UpdateButtonDisplay(GameObject buttonObj, UpgradeItem upgradeItem)
    {
        if (buttonObj == null)
        {
            Debug.LogWarning("按钮对象为空");
            return;
        }

        // 如果升级项目为空（没有可升级的项目），隐藏按钮
        if (upgradeItem == null)
        {
            buttonObj.SetActive(false);
            Debug.Log($"{buttonObj.name} 已隐藏，因为没有可升级的项目");
            return;
        }

        // 显示按钮（可能之前被隐藏了）
        buttonObj.SetActive(true);

        LevelUpSelectionButton button = buttonObj.GetComponent<LevelUpSelectionButton>();
        if (button == null)
        {
            Debug.LogWarning($"{buttonObj.name} 没有 LevelUpSelectionButton 组件");
            return;
        }

        // 根据项目类型调用对应的显示方法
        if (upgradeItem.ItemType == UpgradeItemType.Weapon)
        {
            button.UpdateButtonDisplay(upgradeItem.WeaponData);
        }
        else if (upgradeItem.ItemType == UpgradeItemType.Accessory)
        {
            button.UpdateButtonDisplay(upgradeItem.AccessoryData);
        }
    }

    /// <summary>
    /// 当玩家选择升级选项时触发
    /// </summary>
    /// <param name="buttonIndex">按钮索引 (1, 2, 3)</param>
    public void OnUpgradeSelected(int buttonIndex)
    {
        UpgradeItem selectedItem = null;
        
        switch (buttonIndex)
        {
            case 1:
                selectedItem = _currentSelectedItem1;
                break;
            case 2:
                selectedItem = _currentSelectedItem2;
                break;
            case 3:
                selectedItem = _currentSelectedItem3;
                break;
            default:
                Debug.LogError($"无效的按钮索引: {buttonIndex}");
                return;
        }

        if (selectedItem == null)
        {
            Debug.LogError("选中的升级项目为空！");
            return;
        }

        // 根据项目类型执行升级
        if (selectedItem.ItemType == UpgradeItemType.Weapon)
        {
            UpgradeWeapon(selectedItem);
        }
        else if (selectedItem.ItemType == UpgradeItemType.Accessory)
        {
            UpgradeAccessory(selectedItem);
        }

        // 关闭升级面板
        UIController.instance.levelUpPanel.SetActive(false);
        UIController.instance.pauseButton.interactable = true;
        
        // 更新显示文本
        UIController.instance.UpdateWeaponLevelDisplay();
        UIController.instance.UpdateAccessoryLevelDisplay();
        UIController.instance.UpdatePlayerAttributeDisplay();

        // 恢复游戏时间
        Timer.instance.ResumeTimer();
    }
    
    /// <summary>
    /// 升级武器
    /// </summary>
    /// <param name="upgradeItem">武器升级项目</param>
    private void UpgradeWeapon(UpgradeItem upgradeItem)
    {
        var currentWeapons = WeaponManager.instance.CurrentWeapons;
        if (upgradeItem.Index >= 0 && upgradeItem.Index < currentWeapons.Count)
        {
            WeaponData weaponInList = currentWeapons[upgradeItem.Index];
            if (weaponInList != null)
            {
                // 直接使用属性的set方法修改等级
                int oldLevel = weaponInList.CurrentLevel;
                int newLevel = oldLevel + 1;
                weaponInList.CurrentLevel = newLevel;
                
                Debug.Log($"武器 {weaponInList.WeaponName} 从等级 {oldLevel} 升级到 {newLevel}");
                
                // 执行武器特殊效果
                WeaponManager.instance.ExecuteWeaponSpecialEffect(weaponInList, newLevel);
                
                // 只有当武器从等级0升级时才添加图标（第一次获得武器）与生成预制体
                if (oldLevel == 0 && _weaponIconGenerator != null)
                {
                    _weaponIconGenerator.AddWeaponIcon(weaponInList, $"UpgradedWeapon_{weaponInList.WeaponName}");
                    WeaponPrefabGenerator.instance.GenerateWeaponPrefab(weaponInList.WeaponName);
                }
            }
            else
            {
                Debug.LogError($"索引为 {upgradeItem.Index} 的武器为空");
            }
        }
        else
        {
            Debug.LogError($"无效的武器索引: {upgradeItem.Index}");
        }
    }
    
    /// <summary>
    /// 升级饰品
    /// </summary>
    /// <param name="upgradeItem">饰品升级项目</param>
    private void UpgradeAccessory(UpgradeItem upgradeItem)
    {
        var currentAccessories = AccessoryManager.instance.CurrentAccessories;
        if (upgradeItem.Index >= 0 && upgradeItem.Index < currentAccessories.Count)
        {
            AccessoryData accessory = currentAccessories[upgradeItem.Index];
            if (accessory != null && !accessory.IsMaxLevel)
            {
                int oldLevel = accessory.CurrentLevel;
                int newLevel = oldLevel + 1;
                accessory.CurrentLevel = newLevel;
                
                Debug.Log($"饰品 {accessory.AccessoryName} 从等级 {oldLevel} 升级到 {newLevel}");
                
                // 执行饰品特殊效果
                AccessoryManager.instance.ExecuteAccessorySpecialEffect(accessory, newLevel);
                
                // 只有当饰品从等级0升级时才添加图标（第一次获得饰品）
                if (oldLevel == 0 && _accessoryIconGenerator != null)
                {
                    _accessoryIconGenerator.AddAccessoryIcon(accessory, $"UpgradedAccessory_{accessory.AccessoryName}");
                }
            }
            else
            {
                Debug.LogWarning($"饰品 {accessory?.AccessoryName} 已达到最大等级或为空");
            }
        }
        else
        {
            Debug.LogError($"无效的饰品索引: {upgradeItem.Index}");
        }
    }
} 