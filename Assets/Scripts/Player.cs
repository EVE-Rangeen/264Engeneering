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

    [SerializeField] private float _magnetRange = 1.0f; // 吸取范围半径
    [SerializeField] private float _deathAnimationDuration = 2.0f; // 死亡动画播放时间
    [SerializeField] private Slider _healthSlider; // 血量显示滑条


    #region 玩家属性（局外成长的）
    private float _maxHealth = 100f; // 最大生命值
    private float _health = 100f; // 当前生命值
    private float _recovery = 0.1f; // 恢复速度，每秒恢复生命值的数量
    private float _armor = 0f;  //护甲，减免和护甲数值相等的伤害
    private float _powerFactor = 1.0f; // 力量因子,按比例修改攻击力
    private float _moveSpeed = 1f; // 移动速度，作用于PlayerController
    private float _magnetAreaFactor = 1.0f; // 吸取范围因子，按比例修改吸取范围
    private float _luckIncrement = 0f; //幸运值加成，按比例修改掉落率


    #endregion 玩家属性（基础数值）

    [Header("技能冷却因子")]
    [SerializeField] private float _cooldownReductionFactor = 0.0f; // 冷却因子，按比例减少技能冷却时间
    [Header("攻击范围因子")]
    [SerializeField] private float _attackAreaFactor = 0.0f; // 攻击范围因子，按比例修改攻击范围
    [Header("射弹数量增量")]
    [SerializeField] private int _projectileAmountIncrement = 0; //作用于所有武器，增加所有武器的射弹数
    [Header("武器持续时间因子")]
    [SerializeField] private float _weaponDurationFactor = 1.0f; // 武器持续时间因子，按比例修改武器持续时间

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
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            // 更新血量UI的值
            if (_healthSlider != null)
            {
                _healthSlider.value = _health;
            }
            // 如果血量小于0，则死亡
            if (_health <= 0)
            {
                Die();
            }
        }
    }

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
    public float CooldownReductionFactor
    {
        get => _cooldownReductionFactor;
        set => _cooldownReductionFactor = value;
    }

    /// <summary>
    /// 攻击范围因子
    /// </summary>
    public float AttackAreaFactor => _attackAreaFactor;

    /// <summary>
    /// 射弹数量增量
    /// </summary>
    public int ProjectileAmountIncrement
    {
        get => _projectileAmountIncrement;
        set => _projectileAmountIncrement = value;
    }

    /// <summary>
    /// 武器持续时间因子
    /// </summary>
    public float WeaponDurationFactor
    {
        get => _weaponDurationFactor;
        set => _weaponDurationFactor = value;
    }

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


    private PlayerController _playerController;
    private Animator _animator;
    private Coroutine _healthRecoveryCoroutine;
    private bool _isDead = false; // 玩家是否已死亡的标志

    // 受击闪烁效果相关
    private SpriteRenderer[] _spriteRenderers; // 所有的 SpriteRenderer 组件
    private Material[] _originalMaterials; // 存储原始材质
    private Material _redFlashMaterial; // 红色闪烁材质
    private bool _isFlashing = false; // 是否正在闪烁
    private bool _isShowingRedConstantly = false; // 是否持续显示红色
    private Coroutine _redDisplayCoroutine; // 持续红色显示协程

    private void Start()
    {
        //初始化血量为满血
        _health = _maxHealth;

        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("PlayerController组件未找到！");
        }
        _playerController.SetMoveSpeed(_moveSpeed);

        // 获取Animator组件
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator组件未找到！请确保玩家GameObject上有Animator组件。");
        }

        // 初始化血量UI
        InitializeHealthUI();

        // 启动回血协程
        _healthRecoveryCoroutine = StartCoroutine(_CoHealthRecovery());

        // 初始化受击闪烁效果
        InitializeFlashEffect();
    }

    private void Update()
    {
        // 检测并拾取周围的Pickup物体
        DetectAndPickupItems();
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

            // 如果玩家已死亡，停止回血
            if (_isDead) break;

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
    /// 处理玩家死亡逻辑
    /// </summary>
    private void Die()
    {
        if (_isDead) return; // 如果已经死亡，避免重复执行

        _isDead = true;
        _health = 0;

        // 锁定玩家操作
        if (_playerController != null)
        {
            _playerController.LockPlayer();
        }

        // 停止所有武器的攻击
        // Weapons节点在Player的子节点中，索引为1
        transform.GetChild(1).gameObject.SetActive(false);

        // 停止回血协程
        if (_healthRecoveryCoroutine != null)
        {
            StopCoroutine(_healthRecoveryCoroutine);
            _healthRecoveryCoroutine = null;
        }

        // 播放死亡动画
        if (_animator != null)
        {
            _animator.SetBool("IsDead", true);
            Debug.Log("开始播放死亡动画");
        }

        // 启动死亡处理协程
        StartCoroutine(_CoHandleDeath());
    }

    /// <summary>
    /// 死亡处理协程：播放死亡动画，等待动画播放完毕后显示游戏结束界面
    /// </summary>
    private IEnumerator _CoHandleDeath()
    {
        // 更新血量UI
        UpdateHealthUI();

        // 等待死亡动画播放完毕
        yield return new WaitForSeconds(_deathAnimationDuration + 1.0f);

        // 显示游戏结果界面
        if (UIController.instance != null)
        {
            UIController.instance.UpdateGameResultDisplay();
            Debug.Log("死亡动画播放完毕，显示游戏结果界面");
        }
        else
        {
            Debug.LogError("UIController实例未找到！");
        }

        Debug.Log("玩家死亡处理完成");
    }

    /// <summary>
    /// 初始化受击闪烁效果
    /// </summary>
    private void InitializeFlashEffect()
    {
        // 获取玩家及其子对象的所有SpriteRenderer组件
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (_spriteRenderers.Length > 0)
        {
            // 保存所有SpriteRenderer的原始材质
            _originalMaterials = new Material[_spriteRenderers.Length];
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _originalMaterials[i] = _spriteRenderers[i].material;
            }

            // 创建红色闪烁材质
            _redFlashMaterial = FlashEffectUtils.CreateFlashMaterial(Color.red);
        }
        else
        {
            Debug.LogWarning($"玩家 {gameObject.name} 没有找到SpriteRenderer组件，无法显示受击闪烁效果！");
        }
    }

    /// <summary>
    /// 从怪物本体受到伤害
    /// </summary>
    public void TakeEnemyDamage(float damage)
    {
        if (_isDead) return; // 如果已经死亡，不再受伤

        float actualDamage = CalculateActualDamage(damage);
        _health -= actualDamage;

        // 触发持续红色显示效果
        if (!_isShowingRedConstantly && _spriteRenderers != null && _spriteRenderers.Length > 0)
        {
            StartShowingRedConstantly();
        }

        if (_health <= 0)
        {
            Die();
        }
        else
        {
            // 更新血量UI
            UpdateHealthUI();
        }
    }

    /// <summary>
    /// 从怪物子弹受到伤害
    /// </summary>
    public void TakeEnemyProjectileDamage(float damage)
    {
        if (_isDead) return; // 如果已经死亡，不再受伤

        float actualDamage = CalculateActualDamage(damage);
        _health -= actualDamage;

        // 触发红色闪烁效果（0.1秒）
        if (!_isFlashing && _spriteRenderers != null && _spriteRenderers.Length > 0)
        {
            StartCoroutine(_CoFlashRed());
        }

        // 输出伤害信息用于调试
        if (actualDamage < damage)
        {
            Debug.Log($"护甲减伤：原始伤害{damage}，护甲{_armor}，实际伤害{actualDamage}");
        }

        if (_health <= 0)
        {
            Die();
        }
        else
        {
            // 更新血量UI
            UpdateHealthUI();
        }
    }

    /// <summary>
    /// 治疗
    /// </summary>
    public void HealFromHealthPotion()
    {
        if (_isDead) return; // 如果已经死亡，不能治疗
        if (PlayerResourceManager.instance.CurrentHealthPotions <= 0) return; // 如果血瓶数量为0，不能治疗

        // 这么写是因为GetHealthPotionHealPercent()返回的是百分比数值，所以需要乘以0.01f
        _health += PlayerResourceManager.instance.GetHealthPotionHealPercent() * 0.01f * MaxHealth;
        PlayerResourceManager.instance.UseHealthPotion();

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
        if (_isDead) return; // 如果已经死亡，不能增加生命值

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

    /// <summary>
    /// 检测并拾取周围的Pickup物体
    /// </summary>
    private void DetectAndPickupItems()
    {
        if (_isDead) return; // 如果已经死亡，不再拾取物品
        // 计算实际的吸取范围（基础范围 * 范围因子）
        float actualMagnetRange = _magnetRange * _magnetAreaFactor;

        // 使用OverlapCircleAll检测范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, actualMagnetRange);

        foreach (Collider2D collider in colliders)
        {
            // 检查是否有Pickup组件
            Pickup pickup = collider.GetComponent<Pickup>();
            if (pickup != null)
            {
                // 调用拾取接口
                pickup.PickedUp();
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 在Scene视图中绘制拾取范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 计算实际的吸取范围（基础范围 * 范围因子）
        float actualMagnetRange = _magnetRange * _magnetAreaFactor;

        // 设置Gizmo颜色为半透明的绿色
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

        // 绘制实心圆表示拾取范围
        Gizmos.DrawSphere(transform.position, actualMagnetRange);

        // 设置Gizmo颜色为绿色线框
        Gizmos.color = Color.green;

        // 绘制线框圆表示拾取范围边界
        Gizmos.DrawWireSphere(transform.position, actualMagnetRange);

        // 在范围旁边显示数值信息
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * (actualMagnetRange + 0.5f),
            $"拾取范围: {actualMagnetRange:F1}\n基础: {_magnetRange:F1} × 因子: {_magnetAreaFactor:F1}"
        );
    }
#endif

    /// <summary>
    /// 开始持续显示红色（怪物本体伤害）
    /// </summary>
    private void StartShowingRedConstantly()
    {
        // 如果已经在持续显示红色，重新开始计时
        if (_redDisplayCoroutine != null)
        {
            StopCoroutine(_redDisplayCoroutine);
        }

        _redDisplayCoroutine = StartCoroutine(_CoShowRedConstantly());
    }

    /// <summary>
    /// 持续显示红色协程（怪物本体接触伤害）
    /// </summary>
    /// <returns></returns>
    private IEnumerator _CoShowRedConstantly()
    {
        _isShowingRedConstantly = true;

        // 切换到红色材质
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null && _redFlashMaterial != null)
            {
                _spriteRenderers[i].material = _redFlashMaterial;
            }
        }

        // 等待一段时间后自动恢复（防止一直红色）
        // 这里设置为2秒，如果2秒内没有新的本体伤害就恢复原色
        yield return new WaitForSeconds(0.1f);

        // 恢复原始材质
        RestoreOriginalMaterials();
        _isShowingRedConstantly = false;
        _redDisplayCoroutine = null;
    }

    /// <summary>
    /// 红色闪烁效果协程（怪物子弹伤害）
    /// </summary>
    /// <returns></returns>
    private IEnumerator _CoFlashRed()
    {
        _isFlashing = true;

        // 如果正在持续显示红色，暂时停止
        bool wasShowingRedConstantly = _isShowingRedConstantly;
        if (_isShowingRedConstantly)
        {
            _isShowingRedConstantly = false;
            if (_redDisplayCoroutine != null)
            {
                StopCoroutine(_redDisplayCoroutine);
                _redDisplayCoroutine = null;
            }
        }

        // 切换到红色材质
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null && _redFlashMaterial != null)
            {
                _spriteRenderers[i].material = _redFlashMaterial;
            }
        }

        // 等待0.1秒
        yield return new WaitForSeconds(0.1f);

        // 恢复原始材质
        RestoreOriginalMaterials();

        // 如果之前在持续显示红色，恢复该状态
        if (wasShowingRedConstantly)
        {
            StartShowingRedConstantly();
        }

        _isFlashing = false;
    }

    /// <summary>
    /// 恢复原始材质
    /// </summary>
    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null && _originalMaterials != null && i < _originalMaterials.Length)
            {
                _spriteRenderers[i].material = _originalMaterials[i];
            }
        }
    }

    /// <summary>
    /// 停止所有受击效果
    /// </summary>
    private void StopAllHitEffects()
    {
        // 停止所有相关协程
        if (_redDisplayCoroutine != null)
        {
            StopCoroutine(_redDisplayCoroutine);
            _redDisplayCoroutine = null;
        }

        // 恢复原始材质
        RestoreOriginalMaterials();

        // 重置状态
        _isFlashing = false;
        _isShowingRedConstantly = false;
    }

    /// <summary>
    /// 清理材质资源
    /// </summary>
    private void OnDestroy()
    {
        // 停止回血协程
        if (_healthRecoveryCoroutine != null)
        {
            StopCoroutine(_healthRecoveryCoroutine);
        }

        // 停止所有受击效果
        StopAllHitEffects();

        // 销毁创建的材质实例，避免内存泄漏
        if (_redFlashMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(_redFlashMaterial);
            }
            else
            {
                DestroyImmediate(_redFlashMaterial);
            }
        }
    }
}
