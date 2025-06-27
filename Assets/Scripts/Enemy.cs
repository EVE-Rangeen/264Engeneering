using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpType
{
    Normal,
    High,
    VeryHigh,
}

public enum MoneyType
{
    Coin,
    Purse,
}

/// <summary>
/// 敌人逻辑
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class Enemy : MonoBehaviour
{
    #region 敌人属性
    [SerializeField] private float _maxHealth = 100f; // 最大生命值
    [SerializeField] private float _health = 100f; // 当前生命值
    [SerializeField] private float _damage = 1f; // 攻击力,接触玩家时造成的伤害
    [SerializeField] private float _moveSpeed = 1f; // 移动速度，作用于EnemyController
    [SerializeField] private float _mass = 1f; // 质量，影响敌人被击退的距离
    [SerializeField] private ExpType _expType = ExpType.Normal; // 经验类型
    [SerializeField] private MoneyType _moneyType = MoneyType.Coin; // 金钱类型
    [SerializeField] private float _dropPossibility = 0.8f; // 总掉落概率
    [SerializeField][Range(0f, 1f)] private float _expWeight = 0.7f; // 经验权重，0表示只掉落金币，1表示只掉落经验
    #endregion

    private EnemyController _enemyController;

    private Rigidbody2D _rb;

    private void Awake()
    {
        //初始化血量为满血
        _health = _maxHealth;
        _enemyController = GetComponent<EnemyController>();
        if (_enemyController == null)
        {
            Debug.LogError("EnemyController组件未找到！");
        }
        _enemyController.SetMoveSpeed(_moveSpeed);

        //初始化敌人的质量
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("Enemy需要Rigidbody2D组件！");
        }
        else
        {
            _rb.mass = _mass;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 如果碰撞对象是玩家，则对它造成伤害
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerComponent = collision.gameObject.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.TakeEnemyDamage(_damage);
            }
        }
    }

    /// <summary>
    /// 受到来自玩家武器的伤害
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
    }

    /// <summary>
    /// 敌人死亡处理
    /// </summary>
    private void Die()
    {
        // 首先判断是否掉落物品
        if (Random.Range(0f, 1f) < _dropPossibility)
        {
            // 根据权重决定掉落经验还是金币
            if (Random.Range(0f, 1f) < _expWeight)
            {
                SpawnExp();
            }
            else
            {
                SpawnMoney();
            }
        }

        // 销毁敌人对象
        Destroy(gameObject);
        UIController.defeatedEnemyCount++;
    }

    /// <summary>
    /// 生成经验Prefab
    /// </summary>
    private void SpawnExp()
    {
        string expPrefabPath = "";
        switch (_expType)
        {
            case ExpType.Normal:
                expPrefabPath = "Prefabs/NormalExp";
                break;
            case ExpType.High:
                expPrefabPath = "Prefabs/HighExp";
                break;
            case ExpType.VeryHigh:
                expPrefabPath = "Prefabs/VeryHighExp";
                break;
        }

        GameObject expPrefab = Resources.Load<GameObject>(expPrefabPath);
        if (expPrefab != null)
        {
            Instantiate(expPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"无法加载经验Prefab：{expPrefabPath}");
        }
    }

    /// <summary>
    /// 生成金币Prefab
    /// </summary>
    private void SpawnMoney()
    {
        string moneyPrefabPath = "";
        switch (_moneyType)
        {
            case MoneyType.Coin:
                moneyPrefabPath = "Prefabs/Coin";
                break;
            case MoneyType.Purse:
                moneyPrefabPath = "Prefabs/Purse";
                break;
        }

        GameObject moneyPrefab = Resources.Load<GameObject>(moneyPrefabPath);
        if (moneyPrefab != null)
        {
            Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"无法加载金币Prefab：{moneyPrefabPath}");
        }
    }
}
