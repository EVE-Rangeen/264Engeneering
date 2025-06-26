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
        coinText.text = "金币: " + PlayerStatsManager.instance.TotalCoins.ToString();
    }

    public void UpdateLevelTexts(int index)
    {
        if (index >= 0 && index < PlayerStatsManager.instance.UpgradeableAttributes.Count && index < levelTexts.Count)
        {
            var attribute = PlayerStatsManager.instance.UpgradeableAttributes[index];
            levelTexts[index].text = $"等级: {attribute.CurrentLevel}/{attribute.MaxLevel}";
        }
    }

    public void UpdateDescriptionTexts(int index)
    {
        if (index >= 0 && index < PlayerStatsManager.instance.UpgradeableAttributes.Count && index < descriptionTexts.Count)
        {
            var attribute = PlayerStatsManager.instance.UpgradeableAttributes[index];
            
            // 检查是否已达到最高等级
            if (attribute.IsMaxLevel)
            {
                // 已满级时只显示当前数值
                if (attribute.CurrentLevel > 0 && attribute.CurrentLevel <= attribute.UpgradeValues.Count)
                {
                    float currentValue = attribute.UpgradeValues[attribute.CurrentLevel];
                    descriptionTexts[index].text = $"{attribute.AttributeName}: {currentValue} (已满级)";
                }
                else
                {
                    descriptionTexts[index].text = $"{attribute.AttributeName}: 已满级";
                }
            }
            else
            {
                // 获取当前等级和下一等级的数值
                float currentValue = 0f;
                float nextValue = 0f;
        
                currentValue = attribute.UpgradeValues[attribute.CurrentLevel];
                nextValue = attribute.UpgradeValues[attribute.CurrentLevel + 1];
                
                // 显示格式：属性名: 从 当前值 升为 下一等级值
                descriptionTexts[index].text = $"{attribute.AttributeName}: 从 {currentValue} 升为 {nextValue}";
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
                costTexts[index].text = "已满级";
            }
            else
            {
                // 获取当前等级的升级费用
                if (attribute.CurrentLevel < attribute.UpgradeCosts.Count)
                {
                    int upgradeCost = attribute.UpgradeCosts[attribute.CurrentLevel];
                    costTexts[index].text = $"所需金币: {upgradeCost}";
                }
                else
                {
                    costTexts[index].text = "无法升级";
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
