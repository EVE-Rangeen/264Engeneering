using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三段连击武器
/// 包含：向左攻击、向右攻击、地面砸击（圆形范围攻击）
/// 只负责连击逻辑控制，伤害由各攻击区域的EnemyDamager处理
/// 每次攻击都克隆新的攻击物体，确保动画正确播放
/// 仅支持自动攻击模式
/// 2025-6-25杜宜峰
/// </summary>
public class ComboAttackWeapon : MonoBehaviour
{
    [Header("攻击预制体")]
    [SerializeField] private GameObject _leftSwordPrefab; // 左剑攻击预制体
    [SerializeField] private GameObject _rightSwordPrefab; // 右剑攻击预制体
    [SerializeField] private GameObject _aoeAttackPrefab; // AOE攻击预制体

    [Header("连击设置")]
    // [Tooltip("连击时间窗口")]
    // [SerializeField] private float _comboTimeWindow = 2f; // 连击时间窗口
    [Tooltip("攻击冷却时间")]
    [SerializeField] private float _attackCooldown = 0.5f; // 攻击冷却时间

    [Header("攻击持续时间设置")]
    [Tooltip("剑攻击持续时间")]
    [SerializeField] private float _swordAttackDuration = 0.3f; // 剑攻击持续时间
    [Tooltip("AOE攻击持续时间")]
    [SerializeField] private float _aoeAttackDuration = 0.5f; // AOE攻击持续时间

    [Header("自动攻击设置")]
    [SerializeField] private bool _autoAttack = true; // 是否自动攻击
    [SerializeField] private float _autoAttackInterval = 1f; // 自动攻击间隔

    // 私有字段
    private int _currentCombo = 0; // 当前连击数
    private float _lastAttackTime = 0f; // 上次攻击时间
    // private float _comboTimer = 0f; // 连击计时器
    private bool _canAttack = true; // 是否可以攻击
    private bool _isAttacking = false; // 是否正在攻击
    private PlayerController _playerController; // 玩家控制器引用

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogWarning("ComboAttackWeapon: 未找到PlayerController组件");
        }

        // 验证预制体设置
        ValidatePrefabs();

        // 如果启用自动攻击，开始自动攻击协程
        if (_autoAttack)
        {
            StartCoroutine(_CoAutoAttack());
        }
    }

    /// <summary>
    /// 验证预制体设置
    /// </summary>
    private void ValidatePrefabs()
    {
        if (_leftSwordPrefab == null)
            Debug.LogError("ComboAttackWeapon: 左剑攻击预制体未设置！");
        if (_rightSwordPrefab == null)
            Debug.LogError("ComboAttackWeapon: 右剑攻击预制体未设置！");
        if (_aoeAttackPrefab == null)
            Debug.LogError("ComboAttackWeapon: AOE攻击预制体未设置！");
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    void Update()
    {
        // UpdateComboTimer();
        
        // 如果不是自动攻击，处理手动输入
        // if (!_autoAttack)
        // {
        //     HandleAttackInput();
        // }
    }

    /// <summary>
    /// 自动攻击协程
    /// </summary>
    private IEnumerator _CoAutoAttack()
    {
        while (true)
        {
            // 等待攻击间隔
            yield return new WaitForSeconds(_autoAttackInterval);
            
            // 如果可以攻击且不在攻击中，执行攻击
            if (_canAttack && !_isAttacking && Time.time >= _lastAttackTime + _attackCooldown)
            {
                PerformAttack();
            }
        }
    }

    /// <summary>
    /// 更新连击计时器
    /// </summary>
    // private void UpdateComboTimer()
    // {
    //     if (_currentCombo > 0)
    //     {
    //         _comboTimer += Time.deltaTime;
    //         
    //         // 如果超过连击时间窗口，重置连击
    //         if (_comboTimer >= _comboTimeWindow)
    //         {
    //             ResetCombo();
    //         }
    //     }
    // }

    /// <summary>
    /// 处理攻击输入
    /// </summary>
    // private void HandleAttackInput()
    // {
    //     // 检测攻击输入（空格键或鼠标左键）
    //     if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
    //     {
    //         if (_canAttack && !_isAttacking && Time.time >= _lastAttackTime + _attackCooldown)
    //         {
    //                 PerformAttack();
    //             }
    //         }
    //     }
    // }

    /// <summary>
    /// 执行攻击
    /// </summary>
    private void PerformAttack()
    {
        _lastAttackTime = Time.time;
        // _comboTimer = 0f;
        _currentCombo++;

        switch (_currentCombo)
        {
            case 1:
                StartCoroutine(_CoLeftAttack()); // 向左攻击
                break;
            case 2:
                StartCoroutine(_CoRightAttack()); // 向右攻击
                break;
            case 3:
                StartCoroutine(_CoAoeAttack()); // AOE攻击
                ResetCombo(); // 完成连击后重置
                break;
        }

        Debug.Log($"执行第 {_currentCombo} 段攻击");
    }

    /// <summary>
    /// 左攻击协程
    /// </summary>
    private IEnumerator _CoLeftAttack()
    {
        _isAttacking = true;
        _canAttack = false;

        // 检查预制体是否存在
        if (_leftSwordPrefab == null)
        {
            Debug.LogError("左剑攻击预制体未设置");
            _isAttacking = false;
            _canAttack = true;
            yield break;
        }

        // 克隆左剑攻击物体
        GameObject leftSwordInstance = Instantiate(_leftSwordPrefab, transform.position, transform.rotation, transform);
        // 确保克隆的物体是激活状态
        leftSwordInstance.SetActive(true);
        
        // 等待剑攻击持续时间
        yield return new WaitForSeconds(_swordAttackDuration);
        
        // 销毁攻击物体
        if (leftSwordInstance != null)
        {
            Destroy(leftSwordInstance);
        }

        _isAttacking = false;
        _canAttack = true;
    }

    /// <summary>
    /// 右攻击协程
    /// </summary>
    private IEnumerator _CoRightAttack()
    {
        _isAttacking = true;
        _canAttack = false;

        // 检查预制体是否存在
        if (_rightSwordPrefab == null)
        {
            Debug.LogError("右剑攻击预制体未设置");
            _isAttacking = false;
            _canAttack = true;
            yield break;
        }

        // 克隆右剑攻击物体
        GameObject rightSwordInstance = Instantiate(_rightSwordPrefab, transform.position, transform.rotation, transform);
        // 确保克隆的物体是激活状态
        rightSwordInstance.SetActive(true);
        
        // 等待剑攻击持续时间
        yield return new WaitForSeconds(_swordAttackDuration);
        
        // 销毁攻击物体
        if (rightSwordInstance != null)
        {
            Destroy(rightSwordInstance);
        }

        _isAttacking = false;
        _canAttack = true;
    }

    /// <summary>
    /// AOE攻击协程
    /// </summary>
    private IEnumerator _CoAoeAttack()
    {
        _isAttacking = true;
        _canAttack = false;

        // 检查预制体是否存在
        if (_aoeAttackPrefab == null)
        {
            Debug.LogError("AOE攻击预制体未设置");
            _isAttacking = false;
            _canAttack = true;
            yield break;
        }

        // 克隆AOE攻击物体
        GameObject aoeAttackInstance = Instantiate(_aoeAttackPrefab, transform.position, transform.rotation, transform);
        // 确保克隆的物体是激活状态
        aoeAttackInstance.SetActive(true);
        
        // 等待AOE攻击持续时间
        yield return new WaitForSeconds(_aoeAttackDuration);
        
        // 销毁攻击物体
        if (aoeAttackInstance != null)
        {
            Destroy(aoeAttackInstance);
        }

        _isAttacking = false;
        _canAttack = true;
    }

    /// <summary>
    /// 重置连击
    /// </summary>
    private void ResetCombo()
    {
        _currentCombo = 0;
        // _comboTimer = 0f;
        Debug.Log("连击重置");
    }

    /// <summary>
    /// 获取当前连击数
    /// </summary>
    /// <returns>当前连击数</returns>
    public int GetCurrentCombo()
    {
        return _currentCombo;
    }

    /// <summary>
    /// 检查是否可以攻击
    /// </summary>
    /// <returns>是否可以攻击</returns>
    public bool CanAttack()
    {
        return _canAttack && !_isAttacking && Time.time >= _lastAttackTime + _attackCooldown;
    }

    /// <summary>
    /// 设置自动攻击
    /// </summary>
    /// <param name="enabled">是否启用自动攻击</param>
    public void SetAutoAttack(bool enabled)
    {
        _autoAttack = enabled;
        if (enabled)
        {
            StartCoroutine(_CoAutoAttack());
        }
    }

    /// <summary>
    /// 在Scene视图中绘制攻击范围（仅用于调试）
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 绘制左剑攻击区域
        if (_leftSwordPrefab != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _leftSwordPrefab.transform.localScale);
        }

        // 绘制右剑攻击区域
        if (_rightSwordPrefab != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, _rightSwordPrefab.transform.localScale);
        }

        // 绘制AOE攻击区域
        if (_aoeAttackPrefab != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _aoeAttackPrefab.transform.localScale);
        }
    }
}
