using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Slider expSlider;
    public TMP_Text expText;
    public TMP_Text coinText;
    public TMP_Text timeText;
    public GameObject levelUpPanel;
    public Button pauseButton;
    public TMP_Text weaponInfoText;
    public TMP_Text accessoryInfoText;
    public TMP_Text playerInfoText;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateTime();
    }

    public void UpdateExperience(int currentExp, int levelExp, int currentLvl)
    {
        expSlider.maxValue = levelExp;
        expSlider.value = currentExp;
        expText.text = "等级: " + currentLvl;
    }

    public void UpdateCoin()
    {
        coinText.text = "金币: " + CoinController.instance._currentCoins;
    }

    public void UpdateTime()
    {
        timeText.text = "时间: " + Timer.instance.GetTime();
    }
}
