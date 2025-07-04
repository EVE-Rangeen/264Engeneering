/// <summary>
/// 谈恩萁创建
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceLevelController : MonoBehaviour
{
    public static ExperienceLevelController instance;
    public int currentExperience;
    public List<int> expLevels;
    public int currentLevel = 1, levelCount = 100;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 清空现有列表并重新生成
        expLevels.Clear();
        
        // 第1级不需要经验（玩家已经是1级）
        expLevels.Add(0);
        
        // 根据新的经验增长规则生成经验等级表
        for (int level = 2; level <= levelCount; level++)
        {
            int previousExp = expLevels[level - 2]; // 上一等级所需经验
            int expIncrease;
            
            if (level <= 20)
            {
                // 2-20级：当前等级所需经验 = 上一等级所需经验 + 10
                expIncrease = 10;
            }
            else if (level <= 40)
            {
                // 21-40级：当前等级所需经验 = 上一等级所需经验 + 12
                expIncrease = 12;
            }
            else
            {
                // 41-100级：当前等级所需经验 = 上一等级所需经验 + 20
                expIncrease = 20;
            }
            
            expLevels.Add(previousExp + expIncrease);
        }
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        if (currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
        }

        UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
    }

    public void LevelUp()
    {
        currentExperience -= expLevels[currentLevel];
        currentLevel++;

        if (currentLevel >= expLevels.Count)
        {
            currentLevel = expLevels.Count - 1;
        }

        // 更新UI显示
        UIController.instance.levelUpPanel.SetActive(true);
        UIController.instance.levelUpTitleText.text = "选择一项升级！";
        UIController.instance.pauseButton.interactable = false;
        UpgradeManager.instance.SelectRandomUpgradeItems();
        // 播放升级音效
        SFXManager.instance.PlayLevelUpSFX();
        // 暂停计时器
        Timer.instance.PauseTimer();
        // 增加最大血量与当前血量
        PlayerAttributeManager.instance.PlayerComponent.IncreaseMaxHealthAndCurrentHealth(3f);

    }
}
