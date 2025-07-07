/// <summary>
/// 谈恩萁创建
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsUIController : MonoBehaviour
{
    public static StatsUIController instance;
    public TMP_Text coinText;
    public List<TMP_Text> levelTexts;
    public List<TMP_Text> descriptionTexts;
    public List<TMP_Text> costTexts;

    void Awake()
    {
        instance = this;
        var font = Resources.Load<TMPro.TMP_FontAsset>("Fonts/Uranus_Pixel_11Px SDF");
        if (font != null)
        {
            if (coinText != null) coinText.font = font;
            for (int i = 0; i < costTexts.Count; i++)
            {
                if (costTexts[i] != null) costTexts[i].font = font;
            }
        }
        else
        {
            Debug.LogError("未找到中文TMP字体: Assets/Fonts/Uranus_Pixel_11Px SDF.asset");
        }
    }

    void Start()
    {
        // 在Start中调用，确保PlayerStatsManager已经初始化
        UpdateCoin();

        // 遍历所有可升级属性，更新对应的UI文本
        for (int i = 0; i < PlayerStatsManager.instance.UpgradeableAttributes.Count; i++)
        {
            UpdateLevelTexts(i);
            UpdateDescriptionTexts(i);
            UpdateCostTexts(i);
        }
    }

    public void UpdateCoin()
    {
        coinText.text = LocalizationManager.Instance.GetText("coins", PlayerStatsManager.instance.TotalCoins.ToString());
    }

    public void UpdateLevelTexts(int index)
    {
        if (index >= 0 && index < PlayerStatsManager.instance.UpgradeableAttributes.Count && index < levelTexts.Count)
        {
            var attribute = PlayerStatsManager.instance.UpgradeableAttributes[index];
            levelTexts[index].text = LocalizationManager.Instance.GetText("attribute_level_format", attribute.CurrentLevel + 1, attribute.MaxLevel);
        }
    }

    public void UpdateDescriptionTexts(int index)
    {
        if (index >= 0 && index < PlayerStatsManager.instance.UpgradeableAttributes.Count && index < descriptionTexts.Count)
        {
            var attribute = PlayerStatsManager.instance.UpgradeableAttributes[index];

            if (attribute.IsMaxLevel)
            {
                if (attribute.CurrentLevel > 0 && attribute.CurrentLevel == attribute.UpgradeValues.Count)
                {
                    descriptionTexts[index].text = LocalizationManager.Instance.GetText("attribute_max_value_format", attribute.AttributeName, attribute.CurrentLevel);
                }
                else
                {
                    descriptionTexts[index].text = LocalizationManager.Instance.GetText("attribute_max_level", attribute.AttributeName);
                }
            }
            else
            {
                int currentLevel = attribute.CurrentLevel + 1; // 当前等级（显示为1起始）
                int nextLevel = attribute.CurrentLevel + 2;    // 下一级等级
                descriptionTexts[index].text = LocalizationManager.Instance.GetText("attribute_upgrade_format", attribute.AttributeName, currentLevel, nextLevel);
            }
        }
    }

    public void UpdateCostTexts(int index)
    {
        if (index >= 0 && index < PlayerStatsManager.instance.UpgradeableAttributes.Count && index < costTexts.Count)
        {
            var attribute = PlayerStatsManager.instance.UpgradeableAttributes[index];

            // 检查是否已达到最高等级
            if (attribute.IsMaxLevel)
            {
                costTexts[index].text = LocalizationManager.Instance.GetText("max_level_reached");
            }
            else
            {
                // 获取当前等级的升级费用
                if (attribute.CurrentLevel < attribute.UpgradeCosts.Count)
                {
                    int upgradeCost = attribute.UpgradeCosts[attribute.CurrentLevel];
                    costTexts[index].text = LocalizationManager.Instance.GetText("attribute_cost_format", upgradeCost);
                }
                else
                {
                    costTexts[index].text = LocalizationManager.Instance.GetText("cannot_upgrade");
                }
            }
        }
    }

    public void OnUpgradeButtonClicked(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < PlayerStatsManager.instance.UpgradeableAttributes.Count)
        {
            // 尝试升级属性
            if (PlayerStatsManager.instance.UpgradeAttribute(buttonIndex))
            {
                // 升级成功，更新UI
                UpdateLevelTexts(buttonIndex);
                UpdateDescriptionTexts(buttonIndex);
                UpdateCostTexts(buttonIndex);

                // 同时更新金币显示
                UpdateCoin();
            }
        }
    }
}
