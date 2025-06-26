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

    void Start()
    {
        // 更新显示文本
        UpdateWeaponLevelDisplay();
        UpdatePlayerAttributeDisplay();
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

    public void UpdateWeaponLevelDisplay()
    {
        string weaponLevelText = "武器等级\n";
        
        // 遍历所有当前武器，只显示等级大于0的武器
        var currentWeapons = WeaponManager.instance.CurrentWeapons;
        bool hasWeapons = false;
        
        foreach (var weapon in currentWeapons)
        {
            if (weapon.WeaponLevel > 0)
            {
                weaponLevelText += $"{weapon.WeaponName}: 等级 {weapon.CurrentLevel}/{weapon.MaxLevel}\n";
                hasWeapons = true;
            }
        }
        
        // 如果没有武器，显示提示信息
        if (!hasWeapons)
        {
            weaponLevelText += "暂无武器";
        }
        
        // 更新UI文本
        weaponInfoText.text = weaponLevelText;
    }

    /// <summary>
    /// 更新人物属性显示
    /// </summary>
    public void UpdatePlayerAttributeDisplay()
    {
        string playerAttributeText = "人物属性\n";

        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        // 显示所有可访问的玩家属性
        playerAttributeText += $"最大生命值: {player.MaxHealth:F1}\n";
        playerAttributeText += $"当前生命值: {player.Health:F1}\n";
        playerAttributeText += $"恢复速度: {player.Recovery:F1}/秒\n";
        playerAttributeText += $"护甲值: {player.Armor:F1}\n";
        playerAttributeText += $"力量因子: {player.PowerFactor:F2}\n";
        playerAttributeText += $"移动速度: {player.MoveSpeed:F1}\n";
        playerAttributeText += $"冷却因子: {player.CoolDownFactor:F2}\n";
        playerAttributeText += $"攻击范围因子: {player.AttackAreaFactor:F2}\n";
        playerAttributeText += $"射弹数量增量: {player.ProjectileAmountIncrement}\n";
        playerAttributeText += $"武器持续时间因子: {player.WeaponDurationFactor:F2}\n";
        playerAttributeText += $"吸取范围因子: {player.MagnetAreaFactor:F2}\n";
        playerAttributeText += $"幸运值增量: {player.LuckIncrement:F1}";

        // 更新UI文本
        playerInfoText.text = playerAttributeText;
    }
}
