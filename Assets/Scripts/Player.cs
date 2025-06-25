using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家类，负责玩家的生命值、移动、攻击等
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class Player : MonoBehaviour
{
    #region 玩家属性
    private float _maxHealth = 100f; // 最大生命值
    private float _health = 100f; // 当前生命值
    private float _recovery = 0.1f; // 恢复速度，每秒恢复生命值的数量
    private float _armor = 0f;  //护甲，减免和护甲数值相等的伤害
    private float powerFactor = 1.0f; // 力量因子,按比例修改攻击力
    private float _moveSpeed = 5f; // 移动速度，作用于PlayerController
    private float _coolDownfactor = 1.0f; // 冷却因子，按比例修改技能冷却时间
    private float _attackAreaFactor = 1.0f; // 攻击范围因子，按比例修改攻击范围
    private int _projectileAmountIncrement = 0; //作用于所有武器，增加所有武器的射弹数
    private float _weaponDurationFactor = 1.0f; // 武器持续时间因子，按比例修改武器持续时间
    private float _magnetAreaFactor = 1.0f; // 吸取范围因子，按比例修改戏曲范围
    private float _luckIncrement = 0f; //幸运值加成，按比例修改掉落率
    #endregion

    private PlayerController _playerController;


    void Awake()
    {
        //初始化血量为满血
        _health = _maxHealth;
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("PlayerController组件未找到！");
        }
        _playerController.SetMoveSpeed(_moveSpeed);
    }

    /// <summary>
    /// 从怪物本体受到伤害
    /// </summary>
    public void TakeEnemyDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            Debug.Log("玩家死亡");
        }
    }

    /// <summary>
    /// 从怪物子弹受到伤害
    /// </summary>
    public void TakeEnemyProjectileDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            Debug.Log("玩家死亡");
        }
    }

    /// <summary>
    /// 治疗
    /// </summary>
    public void HealFromHealthBottle(float amount)
    {
        _health += amount;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }


}
