using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 闪电打击武器
/// 在武器射程范围内对最近的X个敌人劈下闪电，每个敌人造成单体伤害
/// 闪电攻击有动画效果，伤害由EnemyDamager脚本实现
/// TODO 闪电动画有问题 现在闪电顶部对齐敌人位置 然后向下劈 现在sprite的pivot是顶部中心 已解决
/// 2025-06-29 杜宜峰创建
/// </summary>
public class LightingStrikerWeapon : MonoBehaviour
{
    [Header("闪电预制体")]
    [SerializeField] private GameObject _lightningPrefab; // 闪电攻击预制体
    [Tooltip("闪电动画子物体（可选，用于分离动画和伤害）")]
    [SerializeField] private GameObject _lightningAnimatorPrefab; // 闪电动画子物体预制体

    [Header("武器索引")]
    [SerializeField] private int weaponIndex = 2; // 当前武器在WeaponManager中的索引

    [Header("闪电武器参数")]
    [Tooltip("同时攻击的敌人数量")]
    [SerializeField] private int _targetCount = 2; // 同时攻击的敌人数量
    [Tooltip("闪电攻击范围")]
    [SerializeField] private float _attackRange = 8f; // 攻击范围
    [Tooltip("攻击冷却时间")]
    [SerializeField] private float _cooldown = 3f; // 攻击冷却时间
    [Tooltip("闪电持续时间")]
    [SerializeField] private float _lightningDuration = 0.5f; // 闪电持续时间
    [Tooltip("闪电间隔时间")]
    [SerializeField] private float _lightningInterval = 0.1f; // 闪电间隔时间
    [Tooltip("闪电碰撞体半径")]
    [SerializeField] private float _lightningColliderRadius = 0.1f; // 闪电碰撞体半径
    [Tooltip("闪电宽度缩放")]
    [SerializeField] private float _lightningWidthScale = 1f; // 闪电宽度缩放
    [Tooltip("闪电高度缩放")]
    [SerializeField] private float _lightningHeightScale = 5f; // 闪电高度缩放

    [Header("闪电位置偏移")]
    [SerializeField] private float _lightningOffsetY = 4f; // 闪电位置偏移


    [Header("音效")]
    [Tooltip("闪电音效的索引值（SFXManager中数组对应的音效的索引）")]
    [SerializeField] private int _lightningSFXIndex = 0;

    // 私有字段
    private float _cooldownTimer = 0f;
    private bool _isAttacking = false;
    private int _currentTargetCount; // 当前实际攻击数量

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        // 延迟初始化，确保WeaponManager已准备好
        StartCoroutine(InitializeWeaponData());
    }

    /// <summary>
    /// 延迟初始化武器数据
    /// </summary>
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
            WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];
            _targetCount = weaponData.AttackCount;
            // 修正：从WeaponData读取攻击范围，并叠加玩家攻击范围加成
            _attackRange = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor);
        }
        else
        {
            _targetCount = 3; // 默认值
            Debug.LogError($"LightingStrikerWeapon: weaponIndex({weaponIndex})超出范围");
        }
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0f || _isAttacking) return;

        // 查找最近的敌人
        List<GameObject> nearestEnemies = FindNearestEnemies(_targetCount);
        if (nearestEnemies.Count == 0) return;

        // 检查是否有敌人在射程内
        bool hasEnemyInRange = false;
        foreach (var enemy in nearestEnemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= _attackRange)
            {
                hasEnemyInRange = true;
                break;
            }
        }

        if (!hasEnemyInRange) return;

        // 启动闪电攻击协程
        StartCoroutine(StrikeLightningAttack(nearestEnemies));
    }

    /// <summary>
    /// 闪电攻击协程
    /// </summary>
    /// <param name="targets">目标敌人列表</param>
    private IEnumerator StrikeLightningAttack(List<GameObject> targets)
    {
        _isAttacking = true;

        // 对每个目标敌人劈下闪电
        foreach (var target in targets)
        {
            if (target == null) continue;

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist > _attackRange) continue;

            // 启动单个闪电攻击
            StartCoroutine(StrikeSingleLightning(target));

            // 闪电之间的间隔时间
            yield return new WaitForSeconds(_lightningInterval);
        }

        // 等待所有闪电攻击完成
        yield return new WaitForSeconds(_lightningDuration + 0.2f);

        _cooldownTimer = _cooldown;
        _isAttacking = false;
    }

    /// <summary>
    /// 对单个目标劈下闪电
    /// </summary>
    /// <param name="target">目标敌人</param>
    private IEnumerator StrikeSingleLightning(GameObject target)
    {
        if (_lightningPrefab == null)
        {
            Debug.LogError("LightingStrikerWeapon: 闪电预制体未设置！");
            yield break;
        }

        // 计算闪电动画位置
        Vector3 lightningPosition = target.transform.position + Vector3.up * _lightningOffsetY;
        GameObject lightning = Instantiate(_lightningPrefab, lightningPosition, Quaternion.identity, null);
        lightning.SetActive(true);

        // 如果设置了动画子物体，也实例化它并附加到闪电上
        if (_lightningAnimatorPrefab != null)
        {
            GameObject animatorObj = Instantiate(_lightningAnimatorPrefab, lightningPosition, lightning.transform.rotation, lightning.transform);
            animatorObj.SetActive(true);
            animatorObj.transform.localScale = new Vector3(_lightningWidthScale, _lightningHeightScale, 1f);
            Animator animator = animatorObj.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(0, 0, 0f);
            }
        }

        // 播放闪电音效
        SFXManager.instance.PlaySFXPitched(_lightningSFXIndex);

        // 生成伤害判定体（InvisibleDamageTrigger）
        GameObject damageTrigger = new GameObject("LightningDamageTrigger");
        damageTrigger.transform.position = target.transform.position;
        var collider = damageTrigger.AddComponent<CircleCollider2D>();
        collider.radius = _lightningColliderRadius; // 建议0.1f
        collider.isTrigger = true;
        var damager = damageTrigger.AddComponent<EnemyDamager>();
        // 传递伤害参数
        if (WeaponManager.instance != null && PlayerAttributeManager.instance != null && weaponIndex >= 0 && weaponIndex < WeaponManager.instance.CurrentWeapons.Count)
        {
            WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];
            float damage = weaponData.Damage;
            float finalDamage = damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor;
            float knockBackForce = weaponData.Knockback;
            damager.damage = finalDamage;
            damager.knockBackForce = knockBackForce;
            damager.timeBetweenDamage = weaponData.BulletInterval;
            damager.destroyOnImpact = true; // 命中即销毁
            damager.lifeTime = _lightningDuration; // 持续时间
        }
        else
        {
            damager.damage = 5f;
            damager.knockBackForce = 5f;
            damager.timeBetweenDamage = 0.1f;
            damager.destroyOnImpact = true;
            damager.lifeTime = _lightningDuration;
        }

        // 等待闪电持续时间
        yield return new WaitForSeconds(_lightningDuration);

        // 销毁闪电动画
        if (lightning != null)
        {
            Destroy(lightning);
        }
        // 伤害判定体会自动销毁（EnemyDamager生命周期）
    }

    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    /// <param name="count">要查找的敌人数量</param>
    /// <returns>最近的敌人列表</returns>
    private List<GameObject> FindNearestEnemies(int count)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange);
        List<GameObject> enemies = new List<GameObject>();

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                enemies.Add(hit.gameObject);
            }
        }

        // 按距离排序并返回最近的count个
        enemies = enemies.OrderBy(enemy =>
            Vector3.Distance(transform.position, enemy.transform.position)).ToList();

        return enemies.Take(count).ToList();
    }

    /// <summary>
    /// 绘制Gizmos（用于调试）
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 绘制攻击范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        // 绘制闪电尺寸预览
        Gizmos.color = Color.red;
        Vector3 lightningSize = new Vector3(_lightningWidthScale, _lightningHeightScale, 1f);
        // 闪电从天空劈下，底部在敌人位置
        Gizmos.DrawWireCube(transform.position + Vector3.up * (_lightningHeightScale * 0.5f), lightningSize);
    }

    /// <summary>
    /// 获取当前攻击数量
    /// </summary>
    /// <returns>当前攻击数量</returns>
    public int GetCurrentTargetCount()
    {
        return _currentTargetCount;
    }

    /// <summary>
    /// 获取攻击范围
    /// </summary>
    /// <returns>攻击范围</returns>
    public float GetAttackRange()
    {
        return _attackRange;
    }

    /// <summary>
    /// 获取冷却时间
    /// </summary>
    /// <returns>冷却时间</returns>
    public float GetCooldown()
    {
        return _cooldown;
    }
}
