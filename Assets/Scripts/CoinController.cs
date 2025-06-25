using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ES3;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;
    public int _currentCoins = 0;  // 当前场景的临时金币
    private int _totalCoins = 0;  // 永久金币总量

    /// <summary>
    /// 当前场景的临时金币数量
    /// </summary>
    public int CurrentCoins => _currentCoins;

    /// <summary>
    /// 永久金币总量
    /// </summary>
    public int TotalCoins => _totalCoins;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 添加临时金币（游戏内获得）
    /// </summary>
    /// <param name="amount">要添加的金币数量</param>
    public void AddCoins(int amount)
    {
        _currentCoins += amount;
        _totalCoins += _currentCoins;
        Save("TotalCoins", _totalCoins);

        UIController.instance.UpdateCoin();
    }
}
