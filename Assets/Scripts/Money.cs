using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 金钱掉落物逻辑
/// 2025-06-26 肖沐奇 创建
/// </summary>
public class Money : MonoBehaviour
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("碰撞对象：" + collision.gameObject.name);
        // 检查碰撞对象或其父对象是否有Player标签，因为渲染Sprite的Collider是Player的子对象
        GameObject playerObject = null;

        // 先检查碰撞对象本身
        if (collision.gameObject.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
        }
        // 如果碰撞对象不是Player，检查其父对象
        else if (collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
            playerObject = collision.transform.parent.gameObject;
        }

        // 如果找到了玩家对象，则添加金钱
        if (playerObject != null)
        {
            if (_moneyConfig != null)
            {
                int amount = _moneyConfig.GetMoneyAmount(_moneyType);
                CoinController.instance.AddCoins(amount);
            }
            else
            {
                Debug.LogError("MoneyConfig 未加载！请确保 Resources/ScriptableObjects/Money Config 文件存在。");
            }
            Destroy(gameObject);
        }
    }
}
