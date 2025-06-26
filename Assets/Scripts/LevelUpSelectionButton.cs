using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpSelectionButton : MonoBehaviour
{
    public TMP_Text upgradeDescText, nameLevelText;
    public Image icon;

    public void UpdateButtonDisplay(WeaponData weapon)
    {
        upgradeDescText.text = weapon.WeaponName + " 升级";
        nameLevelText.text = "当前等级: " + weapon.CurrentLevel + "/" + weapon.MaxLevel;
        icon.sprite = weapon.WeaponIcon;
        Debug.Log("更新按钮显示: " + weapon.WeaponName);
    }
}
