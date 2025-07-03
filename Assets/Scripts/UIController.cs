/// <summary>
/// 谈恩萁创建
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public static int defeatedEnemyCount = 0;

    [Header("UI组件")]
    //经验条
    public Slider expSlider;
    //经验文本
    public TMP_Text expText;
    //金币文本
    public TMP_Text coinText;
    //时间文本
    public TMP_Text timeText;
    //暂停按钮
    public Button pauseButton;
    //血瓶数量文本
    public TMP_Text healthPotionText;
    //血瓶数量图标
    public Image healthPotionIcon;
    //疾走次数文本
    public TMP_Text dashText;

    [Header("游戏设置")]
    //自动结束游戏时间 - 分钟
    public int autoEndGameMinutes = 10;
    //自动结束游戏时间 - 秒钟
    public int autoEndGameSeconds = 0;

    [Header("升级界面")]
    //升级界面
    public GameObject levelUpPanel;
    //升级界面标题
    public TMP_Text levelUpTitleText;
    [Header("暂停界面")]
    //武器信息文本
    public TMP_Text weaponInfoText;
    //饰品信息文本
    public TMP_Text accessoryInfoText;
    //人物信息文本
    public TMP_Text playerInfoText;
    [Header("设置界面")]
    //是否开启神力模式
    public Toggle powerModeToggle;
    [Header("结算界面")]
    //结算界面
    public GameObject gameResultPanel;
    //游戏模式文本
    public TMP_Text gameModeText;
    //生存时间文本
    public TMP_Text survivalTimeText;
    //获得金币文本
    public TMP_Text gainCoinText;
    //最高等级文本
    public TMP_Text MaxLevelText;
    //击败敌人文本
    public TMP_Text beatenEnemyText;
    //获得武器文本
    public TMP_Text totalWeaponText;
    //获得饰品文本
    public TMP_Text totalAccessoryText;
    //总武器图标
    public GameObject totalWeaponIcon;
    //总饰品图标
    public GameObject totalAccessoryIcon;
    
    

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
        UpdateAccessoryLevelDisplay();
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

    public void UpdatePowerMode()
    {
        var player = PlayerAttributeManager.instance.PlayerComponent;
        
        if (powerModeToggle.isOn)
        {
            // Toggle刚被打开：开启神力模式
            player.MaxHealth *= 2f;
            player.Health *= 2f; // 当前血量也翻倍
            player.Armor += 3f;
            Debug.Log($"神力模式已开启：最大血量翻倍至{player.MaxHealth}，当前血量翻倍至{player.Health}，护甲+3至{player.Armor}");
        }
        else
        {
            // Toggle刚被关闭：关闭神力模式
            player.MaxHealth /= 2f;
            player.Health /= 2f; // 当前血量也减半
            player.Armor -= 3f;
            Debug.Log($"神力模式已关闭：最大血量恢复至{player.MaxHealth}，当前血量恢复至{player.Health}，护甲-3至{player.Armor}");
        }
    }

    /// <summary>
    /// 更新获得金币显示（结算界面用）
    /// </summary>
    public void UpdateGainCoinDisplay()
    {
        gainCoinText.text = "获得金币: " + CoinController.instance._currentCoins;
    }

    /// <summary>
    /// 更新生存时间显示（结算界面用）
    /// </summary>
    public void UpdateSurvivalTimeDisplay()
    {
        survivalTimeText.text = "生存时间: " + Timer.instance.GetTime();
    }

    /// <summary>
    /// 更新最高等级显示（结算界面用）
    /// </summary>
    public void UpdateMaxLevelDisplay()
    {
        MaxLevelText.text = "最高等级: " + ExperienceLevelController.instance.currentLevel;
    }

    /// <summary>
    /// 更新游戏模式显示
    /// </summary>
    public void UpdateGameModeDisplay()
    {
        gameModeText.text = "游戏模式: " + (powerModeToggle.isOn ? "神力模式" : "普通模式");
    }

    /// <summary>
    /// 更新击败敌人数量显示
    /// </summary>
    public void UpdateDefeatedEnemyCountDisplay()
    {
        beatenEnemyText.text = "击败敌人: " + defeatedEnemyCount;
    }

    /// <summary>
    /// 更新全部结算界面显示
    /// 统一调用所有结算界面相关的更新方法
    /// </summary>
    public void UpdateGameResultDisplay()
    {
        // 显示结算界面
        gameResultPanel.SetActive(true);
        // 更新生存时间
        UpdateSurvivalTimeDisplay();
        
        // 更新最高等级
        UpdateMaxLevelDisplay();
        
        // 更新获得金币
        UpdateGainCoinDisplay();
        
        // 更新获得武器
        UpdateTotalWeaponDisplay();
        
        // 更新获得饰品
        UpdateTotalAccessoryDisplay();

        // 更新游戏模式
        UpdateGameModeDisplay();

        // 更新击败敌人数量
        UpdateDefeatedEnemyCountDisplay();

        // 重置并暂停计时器
        Timer.instance.ResetTimer();
        Timer.instance.PauseTimer();
    }

    public void UpdateTime()
    {
        string currentTime = Timer.instance.GetTime();
        timeText.text = currentTime;
        
        // 检查是否到达自动结束时间
        // 将时间字符串转换为总秒数进行比较
        string[] currentTimeParts = currentTime.Split(':');
        int currentMinutes = int.Parse(currentTimeParts[0]);
        int currentSeconds = int.Parse(currentTimeParts[1]);
        int currentTotalSeconds = currentMinutes * 60 + currentSeconds;
        
        int autoEndTotalSeconds = autoEndGameMinutes * 60 + autoEndGameSeconds;
        
        if (currentTotalSeconds >= autoEndTotalSeconds)
        {
            Debug.Log($"游戏时间到达 {autoEndGameMinutes}:{autoEndGameSeconds}，准备自动结束游戏");
            UpdateGameResultDisplay();
        }
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
    /// 更新饰品等级显示
    /// </summary>
    public void UpdateAccessoryLevelDisplay()
    {
        string accessoryLevelText = "饰品等级\n";
        
        // 遍历所有当前饰品，只显示等级大于0的饰品
        var currentAccessories = AccessoryManager.instance.CurrentAccessories;
        bool hasAccessories = false;
        
        foreach (var accessory in currentAccessories)
        {
            if (accessory.CurrentLevel > 0)
            {
                accessoryLevelText += $"{accessory.AccessoryName}: 等级 {accessory.CurrentLevel}/{accessory.MaxLevel}\n";
                hasAccessories = true;
            }
        }
        
        // 如果没有饰品，显示提示信息
        if (!hasAccessories)
        {
            accessoryLevelText += "暂无饰品";
        }
        
        // 更新UI文本
        accessoryInfoText.text = accessoryLevelText;
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
        playerAttributeText += $"冷却因子: {player.CooldownReductionFactor:F2}\n";
        playerAttributeText += $"攻击范围因子: {player.AttackAreaFactor:F2}\n";
        playerAttributeText += $"射弹数量增量: {player.ProjectileAmountIncrement}\n";
        playerAttributeText += $"武器持续时间因子: {player.WeaponDurationFactor:F2}\n";
        playerAttributeText += $"吸取范围因子: {player.MagnetAreaFactor:F2}\n";
        playerAttributeText += $"幸运值增量: {player.LuckIncrement:F1}";

        // 更新UI文本
        playerInfoText.text = playerAttributeText;
    }

    /// <summary>
    /// 更新总武器显示（结算界面用）
    /// </summary>
    public void UpdateTotalWeaponDisplay()
    {
        
        // 遍历所有当前武器，只为等级大于0的武器生成图标
        var currentWeapons = WeaponManager.instance.CurrentWeapons;
        
        foreach (var weapon in currentWeapons)
        {
            if (weapon.WeaponLevel > 0)
            {
                // 创建新的GameObject作为子物体
                GameObject iconObject = new GameObject($"WeaponIcon_{weapon.WeaponName}");
                
                // 设置父物体
                iconObject.transform.SetParent(totalWeaponIcon.transform);
                
                // 添加Image组件
                Image iconImage = iconObject.AddComponent<Image>();
                
                // 设置图标精灵
                if (weapon.WeaponIcon != null)
                {
                    iconImage.sprite = weapon.WeaponIcon;
                }
                else
                {
                    Debug.LogWarning($"武器 {weapon.WeaponName} 没有设置图标");
                }

                // 设置RectTransform属性
                RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one;

                Debug.Log($"成功添加武器图标: {weapon.WeaponName}");
            }
        }
    }

    /// <summary>
    /// 更新总饰品显示（结算界面用）
    /// </summary>
    public void UpdateTotalAccessoryDisplay()
    {
        
        // 遍历所有当前饰品，只为等级大于0的饰品生成图标
        var currentAccessories = AccessoryManager.instance.CurrentAccessories;
        
        foreach (var accessory in currentAccessories)
        {
            if (accessory.CurrentLevel > 0)
            {
                // 创建新的GameObject作为子物体
                GameObject iconObject = new GameObject($"AccessoryIcon_{accessory.AccessoryName}");
                
                // 设置父物体
                iconObject.transform.SetParent(totalAccessoryIcon.transform);
                
                // 添加Image组件
                Image iconImage = iconObject.AddComponent<Image>();
                
                // 设置图标精灵
                iconImage.sprite = accessory.AccessoryIcon;

                // 设置RectTransform属性
                RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one;

                Debug.Log($"成功添加饰品图标: {accessory.AccessoryName}");
            }
        }
    }
}
