/// <summary>
/// 谈恩萁创建
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpSelectionButton : MonoBehaviour
{
    public TMP_Text upgradeDescText, nameLevelText;
    public Image icon;

    /// <summary>
    /// 更新按钮显示 - 武器版本
    /// </summary>
    /// <param name="weapon">武器数据</param>
    public void UpdateButtonDisplay(WeaponData weapon)
    {
        // 获取当前等级对应的描述索引
        int descIndex = Mathf.Max(0, weapon.CurrentLevel);
        string description = weapon.WeaponDescriptions.Count > descIndex ? weapon.WeaponDescriptions[descIndex] : "暂无描述";
        
        upgradeDescText.text = weapon.WeaponName + " " + description;
        nameLevelText.text = "当前等级: " + weapon.CurrentLevel + "/" + weapon.MaxLevel;
        icon.sprite = weapon.WeaponIcon;
    }
    
    /// <summary>
    /// 更新按钮显示 - 饰品版本
    /// </summary>
    /// <param name="accessory">饰品数据</param>
    public void UpdateButtonDisplay(AccessoryData accessory)
    {
        // 获取当前等级对应的描述索引
        int descIndex = Mathf.Max(0, accessory.CurrentLevel);
        string description = accessory.AccessoryDescriptions.Count > descIndex ? accessory.AccessoryDescriptions[descIndex] : "暂无描述";
        
        upgradeDescText.text = accessory.AccessoryName + " " + description;
        nameLevelText.text = "当前等级: " + accessory.CurrentLevel + "/" + accessory.MaxLevel;
        icon.sprite = accessory.AccessoryIcon;
    }
}
