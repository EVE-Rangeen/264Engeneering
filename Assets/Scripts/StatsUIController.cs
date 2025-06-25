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
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 在Start中调用，确保PlayerStatsManager已经初始化
        UpdateCoin();
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
            descriptionTexts[index].text = attribute.AttributeDescription;
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
                
                // 同时更新金币显示
                UpdateCoin();
            }
        }
    }
}
