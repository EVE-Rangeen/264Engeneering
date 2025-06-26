using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家类，负责玩家的生命值、移动、攻击等
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class Player : MonoBehaviour
{
    #region 玩家属性（局外成长的）
    private float _maxHealth = 100f; // 最大生命值
    private float _health = 100f; // 当前生命值
    private float _recovery = 0.1f; // 恢复速度，每秒恢复生命值的数量
    private float _armor = 0f;  //护甲，减免和护甲数值相等的伤害
    private float _powerFactor = 1.0f; // 力量因子,按比例修改攻击力
    private float _moveSpeed = 5f; // 移动速度，作用于PlayerController
    private float _coolDownfactor = 1.0f; // 冷却因子，按比例修改技能冷却时间
    private float _attackAreaFactor = 1.0f; // 攻击范围因子，按比例修改攻击范围
    private int _projectileAmountIncrement = 0; //作用于所有武器，增加所有武器的射弹数
    private float _weaponDurationFactor = 1.0f; // 武器持续时间因子，按比例修改武器持续时间
    private float _magnetAreaFactor = 1.0f; // 吸取范围因子，按比例修改吸取范围
    private float _luckIncrement = 0f; //幸运值加成，按比例修改掉落率
    #endregion

    #region 玩家属性访问器
    /// <summary>
    /// 最大生命值
    /// </summary>
    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            // 更新血量UI的最大值
            if (_healthSlider != null)
            {
                _healthSlider.maxValue = _maxHealth;
            }
        }
    }

    /// <summary>
    /// 当前生命值
    /// </summary>
    public float Health => _health;

    /// <summary>
    /// 恢复速度
    /// </summary>
    public float Recovery
    {
        get => _recovery;
        set => _recovery = value;
    }

    /// <summary>
    /// 护甲值
    /// </summary>
    public float Armor
    {
        get => _armor;
        set => _armor = value;
    }

    /// <summary>
    /// 力量因子
    /// </summary>
    public float PowerFactor
    {
        get => _powerFactor;
        set => _powerFactor = value;
    }

    /// <summary>
    /// 移动速度
    /// </summary>
    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
            // 更新PlayerController的移动速度
            if (_playerController != null)
            {
                _playerController.SetMoveSpeed(_moveSpeed);
            }
        }
    }

    /// <summary>
    /// 冷却因子
    /// </summary>
    public float CoolDownFactor => _coolDownfactor;

    /// <summary>
    /// 攻击范围因子
    /// </summary>
    public float AttackAreaFactor => _attackAreaFactor;

    /// <summary>
    /// 射弹数量增量
    /// </summary>
    public int ProjectileAmountIncrement => _projectileAmountIncrement;

    /// <summary>
    /// 武器持续时间因子
    /// </summary>
    public float WeaponDurationFactor => _weaponDurationFactor;

    /// <summary>
    /// 吸取范围因子
    /// </summary>
    public float MagnetAreaFactor
    {
        get => _magnetAreaFactor;
        set => _magnetAreaFactor = value;
    }

    /// <summary>
    /// 幸运值增量
    /// </summary>
    public float LuckIncrement
    {
        get => _luckIncrement;
        set => _luckIncrement = value;
    }
    #endregion

    [SerializeField] private Slider _healthSlider; // 血量显示滑条

    private PlayerController _playerController;
    private Coroutine _healthRecoveryCoroutine;

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

        // 初始化血量UI
        InitializeHealthUI();

        // 启动回血协程
        _healthRecoveryCoroutine = StartCoroutine(_CoHealthRecovery());
    }

    private void OnDestroy()
    {
        // 停止回血协程
        if (_healthRecoveryCoroutine != null)
        {
            StopCoroutine(_healthRecoveryCoroutine);
        }
    }

    /// <summary>
    /// 初始化血量UI
    /// </summary>
    private void InitializeHealthUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = _maxHealth;
            _healthSlider.value = _health;
        }
        else
        {
            Debug.LogWarning("血量Slider未设置！请在Inspector中拖拽Slider组件到_healthSlider字段。");
        }
    }

    /// <summary>
    /// 更新血量UI显示
    /// </summary>
    private void UpdateHealthUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = _health;
        }
    }

    /// <summary>
    /// 每秒回血协程
    /// </summary>
    private IEnumerator _CoHealthRecovery()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // 只有在未满血且回血值大于0时才进行回血
            if (_health < _maxHealth && _recovery > 0)
            {
                _health += _recovery;
                if (_health > _maxHealth)
                {
                    _health = _maxHealth;
                }
                // 更新血量UI
                UpdateHealthUI();
            }
        }
    }

    /// <summary>
    /// 计算护甲减伤后的实际伤害
    /// </summary>
    /// <param name="originalDamage">原始伤害</param>
    /// <returns>减伤后的实际伤害</returns>
    private float CalculateActualDamage(float originalDamage)
    {
        float actualDamage = originalDamage - _armor;
        // 确保伤害不会小于0
        return Mathf.Max(0f, actualDamage);
    }

    /// <summary>
    /// 从怪物本体受到伤害
    /// </summary>
    public void TakeEnemyDamage(float damage)
    {
        float actualDamage = CalculateActualDamage(damage);
        _health -= actualDamage;

        if (_health <= 0)
        {
            _health = 0;
            Debug.Log("玩家死亡");
        }
        // 更新血量UI
        UpdateHealthUI();
    }

    /// <summary>
    /// 从怪物子弹受到伤害
    /// </summary>
    public void TakeEnemyProjectileDamage(float damage)
    {
        float actualDamage = CalculateActualDamage(damage);
        _health -= actualDamage;

        // 输出伤害信息用于调试
        if (actualDamage < damage)
        {
            Debug.Log($"护甲减伤：原始伤害{damage}，护甲{_armor}，实际伤害{actualDamage}");
        }

        if (_health <= 0)
        {
            _health = 0;
            //TODO: 完成游戏结算逻辑
            Debug.Log("玩家死亡");
        }
        // 更新血量UI
        UpdateHealthUI();
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
        // 更新血量UI
        UpdateHealthUI();
    }

    /// <summary>
    /// 增加最大生命值和当前生命值
    /// </summary>
    /// <param name="amount">增加的数值</param>
    public void IncreaseMaxHealthAndCurrentHealth(float amount)
    {
        _maxHealth += amount;
        _health += amount;

        // 更新血量UI的最大值和当前值
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = _maxHealth;
            _healthSlider.value = _health;
        }

        Debug.Log($"生命值提升：最大生命值增加{amount}，当前最大生命值{_maxHealth}，当前生命值{_health}");
    }
}
