using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;
    public int currentCoins = 0;

    void Awake()
    {
        instance = this;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UIController.instance.UpdateCoin();
    }
}
