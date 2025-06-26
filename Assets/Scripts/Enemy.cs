using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpType
{
    Normal,
    High,
    VeryHigh,
}

/// <summary>
/// 敌人基类，所有敌人继承自此类
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
            Debug.Log("敌人与玩家碰撞");
            Debug.Log(collision.gameObject.name);
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
        }
    }
}
