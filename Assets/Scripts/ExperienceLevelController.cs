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
        while (expLevels.Count <= levelCount)
        {
            expLevels.Add(Mathf.RoundToInt(expLevels[expLevels.Count - 1] * 1.1f));
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

    void LevelUp()
    {
        currentExperience -= expLevels[currentLevel];
        currentLevel++;

        if (currentLevel >= expLevels.Count)
        {
            currentLevel = expLevels.Count - 1;
        }

        UIController.instance.levelUpPanel.SetActive(true);
        UpgradeManager.instance.SelectRandomWeapons();
        Timer.instance.PauseTimer();
    }

    /// <summary>
    /// 当玩家选择升级选项时触发
    /// </summary>
    /// <param name="selectedWeapon">选中的武器数据</param>
    public void OnUpgradeSelected()
    {
        
        // 关闭升级面板
        UIController.instance.levelUpPanel.SetActive(false);
        
        // 恢复游戏时间
        Timer.instance.ResumeTimer();
    }
}
