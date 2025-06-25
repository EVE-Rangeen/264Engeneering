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
    private float _maxHealth = 100f; // 最大生命值
    private float _health = 100f; // 当前生命值
    private float _damage = 1f; // 攻击力,接触玩家时造成的伤害
    private float _moveSpeed = 5f; // 移动速度，作用于EnemyController
    private float _mass = 1f; // 质量，影响敌人被击退的距离
    private ExpType _expType = ExpType.Normal; // 经验类型
    #endregion

    private void Awake()
    {
        //初始化血量为满血
        _health = _maxHealth;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 如果碰撞对象是玩家，则对它造成伤害
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<Player>().TakeEnemyDamage(_damage);
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
