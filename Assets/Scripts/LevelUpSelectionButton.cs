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
        string localizedDesc = LocalizationManager.Instance.GetText(description);
        string localizedName = LocalizationManager.Instance.GetText("weapon_" + weapon.WeaponKey); // 武器名也建议本地化
        upgradeDescText.text = localizedName + ": " + localizedDesc;
        nameLevelText.text = LocalizationManager.Instance.GetText("current_level", weapon.CurrentLevel, weapon.MaxLevel);
        icon.sprite = weapon.WeaponIcon;
    }

    /// <summary>
    /// 更新按钮显示 - 饰品版本
    /// </summary>
    /// <param name="accessory">饰品数据</param>
    public void UpdateButtonDisplay(AccessoryData accessory)
    {
        int descIndex = Mathf.Max(0, accessory.CurrentLevel);
        string description = accessory.AccessoryDescriptions.Count > descIndex ? accessory.AccessoryDescriptions[descIndex] : "暂无描述";
        string localizedDesc = LocalizationManager.Instance.GetText(description);
        string localizedName = LocalizationManager.Instance.GetText("accessory_" + accessory.AccessoryKey); // 饰品名本地化
        upgradeDescText.text = localizedName + ": " + localizedDesc;
        nameLevelText.text = LocalizationManager.Instance.GetText("current_level", accessory.CurrentLevel, accessory.MaxLevel);
        icon.sprite = accessory.AccessoryIcon;
    }
}
