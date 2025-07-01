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
        UIController.instance.levelUpTitleText.text = "选择一项升级！";
        UIController.instance.pauseButton.interactable = false;
        UpgradeManager.instance.SelectRandomUpgradeItems();
        Timer.instance.PauseTimer();

    }
}
