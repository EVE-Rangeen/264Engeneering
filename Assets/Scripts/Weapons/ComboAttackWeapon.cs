using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三段连击武器
/// 包含：向左攻击、向右攻击、地面砸击（圆形范围攻击）
/// 只负责连击逻辑控制，伤害由各攻击区域的EnemyDamager处理
/// TODO 修改动画 左右挥砍的动画一直修不好 左剑攻击时候动画是左边挥砍 右边挥砍的时候还是左边挥砍 
/// 并且动画随着攻击范围变化而变化没有实现
/// 2025-6-25杜宜峰
/// </summary>
public class ComboAttackWeapon : MonoBehaviour
{
    [Header("攻击预制体")]
    [SerializeField] private GameObject _leftSwordPrefab; // 左剑攻击预制体
    [SerializeField] private GameObject _rightSwordPrefab; // 右剑攻击预制体
    [SerializeField] private GameObject _aoeAttackPrefab; // AOE攻击预制体

    [Header("攻击设置")]

    [Tooltip("每组攻击之间的冷却时间")]
    [SerializeField] private float _attackCooldown = 0.5f; // 攻击冷却时间
    [SerializeField] private float _attackRange = 0.5f; // 攻击范围

    [Header("攻击持续时间设置")]
    [Tooltip("剑攻击持续时间")]
    [SerializeField] private float _swordAttackDuration = 0.3f; // 剑攻击持续时间
    [Tooltip("AOE攻击持续时间")]
    [SerializeField] private float _aoeAttackDuration = 0.5f; // AOE攻击持续时间

    [Header("自动攻击设置")]
    [SerializeField] private bool _autoAttack = true; // 是否自动攻击
    [Tooltip("一个组攻击每次攻击之间的冷却时间")]
    [SerializeField] private float _autoAttackInterval = 1f; // 自动攻击间隔

    [Header("插地大剑表现")]
    [Tooltip("插地大剑预制体（仅表现，无伤害）")]
    [SerializeField] private GameObject _downSwordPrefab;
    [Tooltip("插地大剑比AOE提前出现的时间（秒）")]
    [SerializeField] private float _downSwordLeadTime = 0.2f;

    [Header("动画控制")]
    [Tooltip("角色或武器的Animator组件（从Inspector拖入）")]
    [SerializeField] private Animator _animator;

    [Header("音效")]
    [Tooltip("挥剑音效的索引值（SFXManager中数组对应的音效的索引）")]
    [SerializeField] private int swordSFXIndex = 0;
    [Tooltip("大剑插地进行AOE攻击音效的索引值（SFXManager中数组对应的音效的索引）")]
    [SerializeField] private int aoeSFXIndex = 0;   

    [Header("武器索引")]
    [SerializeField] private int weaponIndex = 1; // 当前武器在WeaponManager中的索引

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

        // 初始化时禁用Animator，防止自动播放动画
        if (_animator != null)
        {
            _animator.enabled = false;
        }

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
    /// 自动攻击协程（每组三连击，组间冷却）
    /// </summary>
    private IEnumerator _CoAutoAttack()
    {
        while (true)
        {
            // 第一段：左剑
            StartCoroutine(_CoLeftAttack());
            yield return new WaitUntil(() => !_isAttacking);
            yield return new WaitForSeconds(_attackCooldown);

            // 第二段：右剑
            StartCoroutine(_CoRightAttack());
            yield return new WaitUntil(() => !_isAttacking);
            yield return new WaitForSeconds(_attackCooldown);

            // 第三段：AOE
            StartCoroutine(_CoAoeAttack());
            yield return new WaitUntil(() => !_isAttacking);

            // 组冷却
            yield return new WaitForSeconds(_autoAttackInterval);
        }
    }



    /// <summary>
    /// 执行攻击
    /// </summary>
    private void PerformAttack()
    {
        _lastAttackTime = Time.time;
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
        
        // 播放挥剑音效
        SFXManager.instance.PlaySFXPitched(swordSFXIndex);
        Debug.Log("播放左剑音效");

        if (_leftSwordPrefab == null)
        {
            Debug.LogError("左剑攻击预制体未设置");
            _isAttacking = false;
            _canAttack = true;
            yield break;
        }

        // 实例化左剑攻击物体
        GameObject leftSwordInstance = Instantiate(_leftSwordPrefab, transform.position, transform.rotation, transform);
        leftSwordInstance.SetActive(true);
        ApplyLeftSwordParams(leftSwordInstance);

        // 启用Animator并设置参数
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.ResetTrigger("IsLeft");
            _animator.ResetTrigger("IsRight");
            _animator.Play("Idle", 0, 0f); // 先回Idle
            _animator.SetTrigger("IsLeft"); // 触发左攻击
        }

        // 只需等待持续时间
        yield return new WaitForSeconds(_swordAttackDuration);

        // 等待一帧确保动画播放完成
        yield return null;
        
        // 禁用Animator防止最后一帧显示
        if (_animator != null)
        {
            _animator.enabled = false;
        }

        // 销毁攻击实例
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
        
        // 播放挥剑音效
        SFXManager.instance.PlaySFXPitched(swordSFXIndex);
        Debug.Log("播放右剑音效");

        if (_rightSwordPrefab == null)
        {
            Debug.LogError("右剑攻击预制体未设置");
            _isAttacking = false;
            _canAttack = true;
            yield break;
        }

        // 实例化右剑攻击物体
        GameObject rightSwordInstance = Instantiate(_rightSwordPrefab, transform.position, transform.rotation, transform);
        rightSwordInstance.SetActive(true);
        ApplyRightSwordParams(rightSwordInstance);

        // 启用Animator并设置参数
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.ResetTrigger("IsLeft");
            _animator.ResetTrigger("IsRight");
            _animator.Play("Idle", 0, 0f); // 先回Idle
            _animator.SetTrigger("IsRight"); // 触发右攻击
            _animator.SetTrigger("IsIdle");
        }

        // 只需等待持续时间
        yield return new WaitForSeconds(_swordAttackDuration);

        // 等待一帧确保动画播放完成
        yield return null;
        
        // 禁用Animator防止最后一帧显示
        if (_animator != null)
        {
            _animator.enabled = false;
        }

        // 销毁攻击实例
        if (rightSwordInstance != null)
        {
            Destroy(rightSwordInstance);
        }

        _isAttacking = false;
        _canAttack = true;
    }

    /// <summary>
    /// AOE攻击协程（含插地大剑表现）
    /// </summary>
    private IEnumerator _CoAoeAttack()
    {
        _isAttacking = true;
        _canAttack = false;

        // 1. 先实例化插地大剑（仅表现，无伤害）
        GameObject downSwordInstance = null;
        if (_downSwordPrefab != null)
        {
            downSwordInstance = Instantiate(_downSwordPrefab, transform.position, transform.rotation, transform);
            downSwordInstance.SetActive(true);
        }

        // 2. 等待提前量
        yield return new WaitForSeconds(_downSwordLeadTime);

        // 播放插地大剑音效
        SFXManager.instance.PlaySFXPitched(aoeSFXIndex);
        Debug.Log("播放插地大剑音效");

        // 3. 实例化AOE伤害
        if (_aoeAttackPrefab == null)
        {
            Debug.LogError("AOE攻击预制体未设置");
            _isAttacking = false;
            _canAttack = true;
            if (downSwordInstance != null) Destroy(downSwordInstance);
            yield break;
        }
        GameObject aoeAttackInstance = Instantiate(_aoeAttackPrefab, transform.position, transform.rotation, transform);
        aoeAttackInstance.SetActive(true);
        ApplyAoeParams(aoeAttackInstance);

        // 4. 等待AOE持续时间
        yield return new WaitForSeconds(_aoeAttackDuration);

        // 5. 销毁AOE实例
        if (aoeAttackInstance != null) Destroy(aoeAttackInstance);

        // 6. 销毁插地大剑（可选：如需更长表现可单独控制）
        if (downSwordInstance != null) Destroy(downSwordInstance);

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
            Gizmos.DrawWireCube(transform.position + new Vector3(-_attackRange / 2f, 0, 0), new Vector3(_attackRange / 1f, 1f, 1f));
        }

        // 绘制右剑攻击区域
        if (_rightSwordPrefab != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + new Vector3(_attackRange / 2f, 0, 0), new Vector3(_attackRange / 1f, 1f, 1f));
        }

        // 绘制AOE攻击区域
        if (_aoeAttackPrefab != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _attackRange * 0.5f);
        }
    }



    /// <summary>
    /// 左剑参数传递
    /// </summary>
    private void ApplyLeftSwordParams(GameObject instance)
    {
        if (instance == null) return;
        if (WeaponManager.instance == null || PlayerAttributeManager.instance == null) return;
        if (weaponIndex < 0 || weaponIndex >= WeaponManager.instance.CurrentWeapons.Count) return;
        //获取武器数据并且计算最终数据同时准备传参
        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];

        float damage = weaponData.Damage;
        float finalDamage = damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor;

        float knockBackForce = weaponData.Knockback;
        float finalKnockBackForce = knockBackForce;
        
        //传参给EnemyDamager
        _attackCooldown = weaponData.CooldownTime * (1 - PlayerAttributeManager.instance.PlayerComponent.CooldownReductionFactor);
        _autoAttackInterval = weaponData.BulletInterval;

        _attackRange = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor);
        // _attackRange = weaponData.AttackRange;//测试用

        // 设置碰撞箱参数
        BoxCollider2D box = instance.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = new Vector2(_attackRange, 1f);
        }
        // 设置左剑位置偏移（与Gizmos一致）
        instance.transform.localPosition = new Vector3(-_attackRange / 2f + 1f, 0, 0);

        EnemyDamager damager = instance.GetComponent<EnemyDamager>();
        if (damager != null)
        {
            damager.damage = finalDamage;
            damager.knockBackForce = finalKnockBackForce;
        }
    }

    /// <summary>
    /// 右剑参数传递
    /// </summary>
    private void ApplyRightSwordParams(GameObject instance)
    {
        if (instance == null) return;
        if (WeaponManager.instance == null || PlayerAttributeManager.instance == null) return;
        if (weaponIndex < 0 || weaponIndex >= WeaponManager.instance.CurrentWeapons.Count) return;
        //获取武器数据并且计算最终数据同时准备传参
        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];

        float damage = weaponData.Damage;
        float finalDamage = damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor;
        
        float knockBackForce = weaponData.Knockback;
        float finalKnockBackForce = knockBackForce;
        //传参给EnemyDamager
        _attackCooldown = weaponData.CooldownTime * (1 - PlayerAttributeManager.instance.PlayerComponent.CooldownReductionFactor);
        _autoAttackInterval = weaponData.BulletInterval;

        _attackRange = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor);
        // _attackRange = weaponData.AttackRange;//测试用

        // 设置碰撞箱参数
        BoxCollider2D box = instance.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = new Vector2(_attackRange, 1f);
        }
        // 设置右剑位置偏移（与Gizmos一致）
        instance.transform.localPosition = new Vector3(_attackRange / 2f - 1f, 0, 0);

        EnemyDamager damager = instance.GetComponent<EnemyDamager>();
        if (damager != null)
        {
            damager.damage = finalDamage;
            damager.knockBackForce = finalKnockBackForce;
        }
    }

    /// <summary>
    /// AOE参数传递
    /// </summary>
    private void ApplyAoeParams(GameObject instance)
    {
        if (instance == null) return;
        if (WeaponManager.instance == null || PlayerAttributeManager.instance == null) return;
        if (weaponIndex < 0 || weaponIndex >= WeaponManager.instance.CurrentWeapons.Count) return;
        //获取武器数据并且计算最终数据同时准备传参
        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];
        float damage = weaponData.Damage;
        float finalDamage = damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor
                            * weaponData.SpecialAttackMultiplier;
        
        float knockBackForce = weaponData.Knockback;
        float finalKnockBackForce = knockBackForce;

        //传参给EnemyDamager
        _attackCooldown = weaponData.CooldownTime * (1 - PlayerAttributeManager.instance.PlayerComponent.CooldownReductionFactor);
        _autoAttackInterval = weaponData.BulletInterval;
        _attackRange = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor);

        // 设置AOE碰撞体半径
        CircleCollider2D circle = instance.GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            circle.radius = _attackRange * 0.5f;
        }

        EnemyDamager damager = instance.GetComponent<EnemyDamager>();
        if (damager != null)
        {
            damager.damage = finalDamage;
            damager.knockBackForce = finalKnockBackForce;
        }
    }

}
