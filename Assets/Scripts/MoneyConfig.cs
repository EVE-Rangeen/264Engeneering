using UnityEngine;

/// <summary>
/// 金钱配置数据
/// 2025-06-26 肖沐奇 创建
/// </summary>
[CreateAssetMenu(fileName = "Money Config", menuName = "Game Data/Money Config")]
public class MoneyConfig : ScriptableObject
{
    [Header("金币配置")]
    [SerializeField] private int _coinAmount = 10;

    [Header("钱袋配置")]
    [SerializeField] private int _purseAmount = 100;

    /// <summary>
    /// 获取指定类型金钱的数值
    /// </summary>
    /// <param name="moneyType">金钱类型</param>
    /// <returns>金钱数值</returns>
    public int GetMoneyAmount(MoneyType moneyType)
    {
        switch (moneyType)
        {
            case MoneyType.Coin:
                return _coinAmount;
            case MoneyType.Purse:
                return _purseAmount;
            default:
                Debug.LogWarning($"未知的金钱类型：{moneyType}");
                return 0;
        }
    }

    /// <summary>
    /// 金币数量
    /// </summary>
    public int CoinAmount => _coinAmount;

    /// <summary>
    /// 钱袋数量
    /// </summary>
    public int PurseAmount => _purseAmount;
}