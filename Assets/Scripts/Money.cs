using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 金钱掉落物逻辑
/// 2025-06-26 肖沐奇 创建
/// </summary>
public class Money : Pickup
{
    [Header("金钱类型")]
    [SerializeField] private MoneyType _moneyType = MoneyType.Coin;

    private MoneyConfig _moneyConfig;

    void Awake()
    {
        // 从Resources目录加载金钱配置
        _moneyConfig = Resources.Load<MoneyConfig>("ScriptableObjects/Money Config");
        if (_moneyConfig == null)
        {
            Debug.LogError("无法从Resources/ScriptableObjects/Money Config加载金钱配置！");
        }
    }

    public override void PickedUp()
    {
        // 防止重复拾取
        if (_isPickedUp) return;

        if (_moneyConfig != null)
        {
            int amount = _moneyConfig.GetMoneyAmount(_moneyType);
            CoinController.instance.AddCoins(amount);
        }
        else
        {
            Debug.LogError("MoneyConfig 未加载！请确保 Resources/ScriptableObjects/Money Config 文件存在。");
        }

        // 调用基类的拾取动画
        base.PickedUp();
    }
}
