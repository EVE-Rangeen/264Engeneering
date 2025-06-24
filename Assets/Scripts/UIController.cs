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

    void Awake()
    {
        instance = this;
    }

    public void UpdateExperience(int currentExp, int levelExp, int currentLvl)
    {
        expSlider.maxValue = levelExp;
        expSlider.value = currentExp;
        expText.text = "等级: " + currentLvl;
    }

    public void UpdateCoin()
    {
        coinText.text = "金币: " + CoinController.instance.currentCoins;
    }
}
