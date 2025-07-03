using System.Collections;
using UnityEngine;

/// <summary>
/// FlameThrowerWeapon 持续伤害武器，自动攻击，扇形范围跟随玩家移动方向，45度角度范围。
/// 使用EnemyDamager实现伤害，保持与其他武器相同的架构。
/// 支持实时方向跟随，攻击期间方向会跟随玩家移动方向变化。
/// 杜宜峰2025-06-26
/// </summary>
public class FlameThrowerWeapon : MonoBehaviour
{
    [Header("武器索引")]
    [SerializeField] private int weaponIndex = 0;

    [Header("攻击区域（需挂载EnemyDamager）")]
    [SerializeField] private GameObject attackZone;

    [Header("火焰粒子特效")]
    [SerializeField] private GameObject flameVFXPrefab;
    [SerializeField] private bool _showFlameVFX = true; // 是否显示火焰特效

    [Header("可视化效果")]
    [SerializeField] private LineRenderer _attackAreaVisual;
    [SerializeField] private Color _lineColor = Color.red;
    [SerializeField] private float _lineWidth = 0.1f;
    [SerializeField] private bool _showVisualEffect = true; // 是否显示可视化效果

    [Header("音效")]
    [Tooltip("攻击音效的索引值（SFXManager中数组对应的音效的索引）")]
    [SerializeField] private int fireSFXIndex = 0;

    // 私有字段
    private bool _isAttacking = false;
    [SerializeField] private float _attackCooldown = 1.0f; // 攻击冷却时间（每次攻击之间的间隔）
    [SerializeField] private float _attackDuration = 2.0f; // 攻击持续时间（每次攻击持续的时间）
    [SerializeField] private float _attackRange = 3.0f;
    [SerializeField] private float _attackAngle = 45f; // 45度扇形
    [SerializeField] private float _damageInterval = 0.5f; // 持续伤害间隔
    private PlayerController _playerController;
    private EnemyDamager _enemyDamager;
    private PolygonCollider2D _polygonCollider;

    // 火焰特效相关
    private GameObject _flameVFXInstance;
    private ParticleSystem _flameParticleSystem;
    private bool _flameVFXInitialized = false; // 标记火焰特效是否已初始化完成

    // 方向跟随相关
    private Vector2 _lastAttackDirection = Vector2.right; // 记录上次的攻击方向
    private bool _directionChanged = false; // 标记方向是否发生变化

    void Start()
    {
        // 延迟初始化，确保WeaponManager已准备好
        StartCoroutine(InitializeWeaponData());

        // 获取玩家控制器引用
        _playerController = FindObjectOfType<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("FlameThrowerWeapon: 未找到PlayerController组件！");
        }

        // 获取EnemyDamager组件
        if (attackZone != null)
        {
            _enemyDamager = attackZone.GetComponent<EnemyDamager>();
            if (_enemyDamager == null)
            {
                Debug.LogError("FlameThrowerWeapon: AttackZone上未找到EnemyDamager组件！");
            }

            // 获取或创建PolygonCollider2D
            _polygonCollider = attackZone.GetComponent<PolygonCollider2D>();
            if (_polygonCollider == null)
            {
                _polygonCollider = attackZone.AddComponent<PolygonCollider2D>();
            }
            _polygonCollider.isTrigger = true;

            // 初始化可视化效果
            InitializeVisualEffect();

            // 初始化火焰特效
            InitializeFlameVFX();
        }
    }

    void Update()
    {
        // 在攻击期间实时检测方向变化
        if (_isAttacking)
        {
            CheckDirectionChange();
        }
    }

    /// <summary>
    /// 检测玩家移动方向是否发生变化
    /// </summary>
    private void CheckDirectionChange()
    {
        if (_playerController == null) return;

        Vector2 currentDirection = _playerController.GetMoveDirection();

        // 如果玩家没有移动，使用默认方向
        if (currentDirection == Vector2.zero)
        {
            currentDirection = Vector2.right;
        }

        // 检查方向是否发生变化（由于是8个固定方向，直接比较即可）
        if (currentDirection != _lastAttackDirection)
        {
            _directionChanged = true;
            _lastAttackDirection = currentDirection;

            // 立即更新攻击区域
            UpdateAttackZoneTransform();
        }
    }

    /// <summary>
    /// 初始化可视化效果
    /// </summary>
    private void InitializeVisualEffect()
    {
        if (_attackAreaVisual == null)
        {
            // 创建LineRenderer
            GameObject visualObj = new GameObject("AttackAreaVisual");
            visualObj.transform.SetParent(attackZone.transform);
            visualObj.transform.localPosition = Vector3.zero;
            visualObj.transform.localRotation = Quaternion.identity;

            _attackAreaVisual = visualObj.AddComponent<LineRenderer>();
        }

        // 配置LineRenderer
        _attackAreaVisual.material = new Material(Shader.Find("Sprites/Default"));
        _attackAreaVisual.startColor = _lineColor;
        _attackAreaVisual.endColor = _lineColor;
        _attackAreaVisual.startWidth = _lineWidth;
        _attackAreaVisual.endWidth = _lineWidth;
        _attackAreaVisual.useWorldSpace = false;
        _attackAreaVisual.loop = true;
        _attackAreaVisual.sortingOrder = 10; // 确保在最前面显示

        // 初始时隐藏
        _attackAreaVisual.enabled = false;
    }

    /// <summary>
    /// 初始化火焰特效
    /// </summary>
    private void InitializeFlameVFX()
    {
        if (flameVFXPrefab != null && _showFlameVFX)
        {
            // 实例化火焰特效
            _flameVFXInstance = Instantiate(flameVFXPrefab, attackZone.transform);
            _flameVFXInstance.transform.localPosition = Vector3.zero;
            _flameVFXInstance.transform.localRotation = Quaternion.Euler(0, 0, -_attackAngle / 2f);

            // 获取粒子系统组件
            _flameParticleSystem = _flameVFXInstance.GetComponent<ParticleSystem>();
            if (_flameParticleSystem == null)
            {
                Debug.LogWarning("FlameThrowerWeapon: 火焰特效Prefab上未找到ParticleSystem组件！");
            }
            else
            {
                // 标记火焰特效已初始化完成
                _flameVFXInitialized = true;
                Debug.Log("FlameThrowerWeapon: 火焰特效初始化完成");
            }

            // 初始时禁用
            _flameVFXInstance.SetActive(false);
        }
        else
        {
            // 如果没有火焰特效，也标记为已初始化（避免无限等待）
            _flameVFXInitialized = true;
        }
    }

    private IEnumerator InitializeWeaponData()
    {
        // 等待WeaponManager初始化完成
        while (WeaponManager.instance == null ||
               WeaponManager.instance.CurrentWeapons.Count == 0)
        {
            yield return null;
        }

        // 确保weaponIndex有效
        if (weaponIndex >= 0 && weaponIndex < WeaponManager.instance.CurrentWeapons.Count)
        {
            UpdateWeaponParameters();
        }
        else
        {
            Debug.LogError($"FlameThrowerWeapon: weaponIndex({weaponIndex})超出范围");
        }

        // 等待火焰特效初始化完成
        yield return StartCoroutine(WaitForFlameVFXInitialization());

        // 启动自动攻击循环
        StartCoroutine(AutoAttackLoop());
    }

    /// <summary>
    /// 等待火焰特效初始化完成
    /// </summary>
    private IEnumerator WaitForFlameVFXInitialization()
    {
        // 等待火焰特效初始化完成
        while (!_flameVFXInitialized)
        {
            yield return null;
        }

        // 额外等待一帧，确保所有组件都已准备就绪
        yield return null;

        Debug.Log("FlameThrowerWeapon: 所有初始化完成，开始攻击循环");
    }

    /// <summary>
    /// 更新武器参数
    /// </summary>
    private void UpdateWeaponParameters()
    {
        if (WeaponManager.instance == null || weaponIndex < 0 || weaponIndex >= WeaponManager.instance.CurrentWeapons.Count)
            return;

        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];

        // 调试信息：检查WeaponData
        Debug.Log($"FlameThrowerWeapon: 武器名称={weaponData.WeaponName}, 伤害={weaponData.Damage}, 范围={weaponData.AttackRange}, 冷却={weaponData.CooldownTime}, 持续时间={weaponData.Duration}");

        // 计算最终冷却时间（每次攻击之间的间隔）
        _attackCooldown = weaponData.CooldownTime * (1 - PlayerAttributeManager.instance.PlayerComponent.CooldownReductionFactor);

        // 获取攻击持续时间
        _attackDuration = weaponData.Duration;

        // 计算最终攻击范围（扇形半径）
        _attackRange = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor);

        // 获取持续伤害间隔时间
        _damageInterval = weaponData.BulletInterval;

        // 更新扇形碰撞器
        UpdateConeCollider();

        // 更新可视化效果
        UpdateVisualEffect();

        // 更新火焰特效
        UpdateFlameVFX();
    }

    /// <summary>
    /// 更新火焰特效
    /// </summary>
    private void UpdateFlameVFX()
    {
        if (!_showFlameVFX || _flameVFXInstance == null || _flameParticleSystem == null || !_flameVFXInitialized) return;

        // 更新粒子系统的Shape角度以匹配攻击角度
        var shape = _flameParticleSystem.shape;
        shape.angle = 45f; // 90°扇形

        // 更新粒子速度以匹配攻击范围
        var main = _flameParticleSystem.main;
        float particleSpeed = _attackRange / main.startLifetime.constant; // 速度 = 距离 / 时间
        main.startSpeed = particleSpeed;

        // 更新粒子数量以匹配攻击范围（更大的范围需要更多粒子）
        var emission = _flameParticleSystem.emission;
        float baseRate = 100f; // 基础发射率
        float rangeMultiplier = _attackRange / 3f; // 基于默认3米范围的比例
        emission.rateOverTime = baseRate * rangeMultiplier;
    }

    private IEnumerator AutoAttackLoop()
    {
        while (true)
        {
            // 攻击前初始化方向（获取当前玩家移动方向）
            InitializeAttackDirection();

            // 攻击前同步判定区朝向和范围，并设置参数
            UpdateAttackZoneTransform();
            SetAttackParameters();

            if (attackZone != null)
                attackZone.SetActive(true);
            _isAttacking = true;

            // 显示可视化效果
            if (_showVisualEffect && _attackAreaVisual != null)
            {
                _attackAreaVisual.enabled = true;
            }

            // 启用火焰特效
            if (_showFlameVFX && _flameVFXInstance != null && _flameVFXInitialized)
            {
                _flameVFXInstance.SetActive(true);
                if (_flameParticleSystem != null)
                {
                    _flameParticleSystem.Play();
                }
            }

            // 播放攻击音效（循环）
            if (SFXManager.instance != null)
            {
                var audio = SFXManager.instance.soundEffects[fireSFXIndex];
                audio.loop = true;
                audio.Play();
            }

            // 输出攻击开始日志
            Debug.Log($"FlameThrowerWeapon: 进入攻击，攻击持续时间={_attackDuration}秒");

            // 等待攻击持续时间
            yield return new WaitForSeconds(_attackDuration);

            if (attackZone != null)
                attackZone.SetActive(false);
            _isAttacking = false;

            // 隐藏可视化效果
            if (_showVisualEffect && _attackAreaVisual != null)
            {
                _attackAreaVisual.enabled = false;
            }

            // 禁用火焰特效
            if (_showFlameVFX && _flameVFXInstance != null && _flameVFXInitialized)
            {
                _flameVFXInstance.SetActive(false);
                if (_flameParticleSystem != null)
                {
                    _flameParticleSystem.Stop();
                }
            }

            // 停止攻击音效
            if (SFXManager.instance != null)
            {
                var audio = SFXManager.instance.soundEffects[fireSFXIndex];
                audio.Stop();
                audio.loop = false;
            }

            // 输出冷却开始日志
            Debug.Log($"FlameThrowerWeapon: 进入CD，CD持续时间={_attackCooldown}秒");

            // 等待冷却时间
            yield return new WaitForSeconds(_attackCooldown);
        }
    }

    /// <summary>
    /// 设置攻击参数（每次攻击时调用，确保参数正确）
    /// </summary>
    private void SetAttackParameters()
    {
        if (_enemyDamager == null) return;

        // 添加安全检查
        if (WeaponManager.instance == null ||
            weaponIndex < 0 ||
            weaponIndex >= WeaponManager.instance.CurrentWeapons.Count)
        {
            Debug.LogError($"FlameThrowerWeapon: 无法获取武器数据 - WeaponManager={WeaponManager.instance != null}, weaponIndex={weaponIndex}");
            return;
        }

        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];

        // 计算最终伤害
        float damage = weaponData.Damage;
        float finalDamage = damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor;

        // 计算最终击退力度
        float knockBackForce = weaponData.Knockback;
        float finalKnockBackForce = knockBackForce;

        // 设置EnemyDamager的参数
        _enemyDamager.damage = finalDamage;
        _enemyDamager.knockBackForce = finalKnockBackForce;
        _enemyDamager.timeBetweenDamage = weaponData.BulletInterval;
        _enemyDamager.damageOverTime = true; // 启用持续伤害
        _enemyDamager.lifeTime = 1000f; // 设置很大的生命周期，避免自动销毁
        _enemyDamager.shouldKnockBack = true; // 启用击退

        // 调试信息：检查EnemyDamager设置
        Debug.Log($"FlameThrowerWeapon: 设置参数 - 伤害={_enemyDamager.damage}, 击退={_enemyDamager.knockBackForce}, 间隔={_enemyDamager.timeBetweenDamage}, 生命周期={_enemyDamager.lifeTime}");
    }

    /// <summary>
    /// 同步attackZone的位置、朝向和范围（扇形跟随玩家移动方向）
    /// </summary>
    private void UpdateAttackZoneTransform()
    {
        if (attackZone == null) return;

        // 让attackZone的位置和武器一致
        attackZone.transform.position = transform.position;

        // 使用记录的攻击方向（支持实时更新）
        Vector2 playerDirection = _lastAttackDirection;

        // 计算朝向角度
        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        attackZone.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 更新扇形攻击区域
        UpdateConeCollider();

        // 更新可视化效果
        UpdateVisualEffect();
    }

    /// <summary>
    /// 更新可视化效果
    /// </summary>
    private void UpdateVisualEffect()
    {
        if (!_showVisualEffect || _attackAreaVisual == null) return;

        // 生成扇形线框点
        Vector3[] points = GenerateConeVisualPoints(_attackRange, _attackAngle);
        _attackAreaVisual.positionCount = points.Length;
        _attackAreaVisual.SetPositions(points);
    }

    /// <summary>
    /// 生成扇形可视化点
    /// </summary>
    /// <param name="radius">扇形半径</param>
    /// <param name="angle">扇形角度</param>
    /// <returns>可视化点数组</returns>
    private Vector3[] GenerateConeVisualPoints(float radius, float angle)
    {
        int segments = 16; // 扇形分段数
        Vector3[] points = new Vector3[segments + 2]; // +2 是因为要包含中心点和闭合

        float halfAngle = angle / 2f;
        float angleStep = angle / segments;

        // 中心点
        points[0] = Vector3.zero;

        // 生成扇形边缘点
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angleStep * i);
            float radians = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            points[i + 1] = new Vector3(x, y, 0);
        }

        return points;
    }

    /// <summary>
    /// 更新扇形碰撞器
    /// </summary>
    private void UpdateConeCollider()
    {
        if (_polygonCollider == null) return;

        Vector2[] points = GenerateConePoints(_attackRange, _attackAngle);
        _polygonCollider.points = points;
    }

    /// <summary>
    /// 生成扇形碰撞点
    /// </summary>
    /// <param name="radius">扇形半径</param>
    /// <param name="angle">扇形角度</param>
    /// <returns>碰撞点数组</returns>
    private Vector2[] GenerateConePoints(float radius, float angle)
    {
        int segments = 16; // 扇形分段数
        Vector2[] points = new Vector2[segments + 2]; // +2 是因为要包含中心点和闭合

        float halfAngle = angle / 2f;
        float angleStep = angle / segments;

        // 中心点
        points[0] = Vector2.zero;

        // 生成扇形边缘点
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angleStep * i);
            float radians = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            points[i + 1] = new Vector2(x, y);
        }

        return points;
    }

    /// <summary>
    /// 外部可调用，动态设置攻击范围
    /// </summary>
    public void SetAttackRange(float range, float angle)
    {
        _attackRange = range;
        _attackAngle = angle;
        UpdateWeaponParameters();
    }

    /// <summary>
    /// 外部可调用，设置火焰特效的显示状态
    /// </summary>
    public void SetFlameVFXEnabled(bool enabled)
    {
        _showFlameVFX = enabled;
        if (_flameVFXInstance != null)
        {
            _flameVFXInstance.SetActive(enabled && _isAttacking);
        }
    }

    /// <summary>
    /// 外部可调用，获取火焰特效的显示状态
    /// </summary>
    public bool IsFlameVFXEnabled()
    {
        return _showFlameVFX;
    }

    // 在编辑器中绘制攻击范围，方便调试
    void OnDrawGizmosSelected()
    {
        if (attackZone == null) return;

        Gizmos.color = Color.red;
        Vector3 center = attackZone.transform.position;
        Vector2 playerDirection = Vector2.right;

        if (_playerController != null)
        {
            playerDirection = _playerController.GetMoveDirection();
            if (playerDirection == Vector2.zero)
            {
                playerDirection = Vector2.right;
            }
        }

        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

        // 绘制扇形
        float halfAngle = _attackAngle / 2f;
        int segments = 16;
        float angleStep = _attackAngle / segments;

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = -halfAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Vector3 rayDirection = rotation * direction * _attackRange;

            Gizmos.DrawRay(center, rayDirection);
        }

        // 绘制扇形边缘
        Quaternion leftRotation = Quaternion.Euler(0, 0, -halfAngle);
        Quaternion rightRotation = Quaternion.Euler(0, 0, halfAngle);
        Vector3 leftEdge = leftRotation * direction * _attackRange;
        Vector3 rightEdge = rightRotation * direction * _attackRange;

        Gizmos.DrawLine(center, center + leftEdge);
        Gizmos.DrawLine(center, center + rightEdge);
    }

    /// <summary>
    /// 初始化攻击方向（在每次攻击开始时调用）
    /// </summary>
    private void InitializeAttackDirection()
    {
        if (_playerController == null) return;

        Vector2 currentDirection = _playerController.GetMoveDirection();

        // 如果玩家没有移动，使用默认方向
        if (currentDirection == Vector2.zero)
        {
            currentDirection = Vector2.right;
        }

        // 更新记录的攻击方向
        _lastAttackDirection = currentDirection;
        _directionChanged = false; // 重置方向变化标记
    }
}
