using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [Header("金钱类型")]
    [SerializeField] private MoneyType _moneyType = MoneyType.Coin;
    [SerializeField] private int _coinAmount = 10;
    [SerializeField] private int _purseAmount = 100;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰撞对象是玩家，则添加金钱
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_moneyType == MoneyType.Coin)
            {
                CoinController.instance.AddCoins(_coinAmount);
            }
            else if (_moneyType == MoneyType.Purse)
            {
                CoinController.instance.AddCoins(_purseAmount);
            }
            Destroy(gameObject);
        }
    }
}
