using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 旋转武器脚本，控制环绕实体的旋转攻击行为。环绕实体需要挂载EnemyDamager组件并且默认inactive，作为SpinningWeapon的子物体。
/// 2025-06-30杜宜峰
/// </summary>
public class SpinningWeapon : MonoBehaviour
{
    [Header("武器索引")]
    [SerializeField] private int weaponIndex = 0;

    [Header("旋转武器参数")]
    [Tooltip("环绕实体的预制体")]
    [SerializeField] private GameObject spinObjectPrefab;
    [Tooltip("环绕半径")]
    [SerializeField] private float _orbitRadius = 2f;
    [Tooltip("环绕速度")]
    [SerializeField] private float _orbitSpeed = 100f;
    [Tooltip("同时环绕的实体数量")]
    [SerializeField] private int _spinObjectCount = 3;
    [Tooltip("单个伤害实体对同一敌人的伤害间隔")]
    [SerializeField] private float _damageInterval = 0.5f;
    [Tooltip("环绕实体存在时间")]
    [SerializeField] private float _duration = 5f;
    [Tooltip("每波攻击之间的冷却时间")]
    [SerializeField] private float _cooldownTime = 3f;

    [Header("生成间隔")]
    [Tooltip("多个环绕实体生成时的间隔时间")]
    [SerializeField] private float _spawnInterval = 0.2f;

    [Header("音效")]
    [Tooltip("生成音效的索引值（SFXManager中数组对应的音效的索引）")]
    [SerializeField] private int _spawnSFXIndex = 0;

    [Header("调试用最终参数")]
    [SerializeField] private float finalDamage;
    [SerializeField] private float finalKnockBackForce;
    [SerializeField] private float finalTimeBetweenDamage;
    [SerializeField] private float finalOrbitSpeed;
    [SerializeField] private int finalSpinObjectCount;
    [SerializeField] private float finalOrbitRadius;
    [SerializeField] private float finalCooldownTime;
    [SerializeField] private float finalDuration;

    private List<GameObject> activeSpinObjects = new List<GameObject>();
    private float _cooldownTimer = 0f;
    private bool _isSpawning = false;

    void Start()
    {
        // 延迟初始化，确保WeaponManager已准备好
        StartCoroutine(InitializeWeaponData());
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
            WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];
            _spinObjectCount = weaponData.AttackCount;
        }
        else
        {
            _spinObjectCount = 3; // 默认值
            Debug.LogError($"SpinningWeapon: weaponIndex({weaponIndex})超出范围");
        }
    }

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0f || _isSpawning) return;

        // 检查是否有敌人在范围内
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        float dist = Vector3.Distance(transform.position, nearestEnemy.transform.position);
        if (dist > _orbitRadius * 2f) return; // 检查敌人是否在攻击范围内

        // 启动生成协程
        StartCoroutine(SpawnSpinObjects());
    }

    /// <summary>
    /// 生成环绕实体的协程
    /// </summary>
    IEnumerator SpawnSpinObjects()
    {
        _isSpawning = true;

        // 清理之前的环绕实体
        ClearActiveSpinObjects();

        // 计算当前武器数据
        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];
        
        // 计算最终参数
        finalDamage = weaponData.Damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor;
        finalKnockBackForce = weaponData.Knockback;
        finalTimeBetweenDamage = weaponData.BulletInterval;
        finalOrbitSpeed = weaponData.ProjectileSpeed * Mathf.Rad2Deg; // 将弧度转换为度数
        finalSpinObjectCount = PlayerAttributeManager.instance.PlayerComponent.ProjectileAmountIncrement + weaponData.AttackCount;
        finalOrbitRadius = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor);
        finalCooldownTime = weaponData.CooldownTime * (1 - PlayerAttributeManager.instance.PlayerComponent.CooldownReductionFactor);
        finalDuration = weaponData.Duration;

        // 生成环绕实体
        for (int i = 0; i < finalSpinObjectCount; i++)
        {
            SpawnSpinObject(i, finalSpinObjectCount, finalOrbitRadius, finalOrbitSpeed, 
                           finalDamage, finalKnockBackForce, finalTimeBetweenDamage, finalDuration);
            
            // 播放音效
            SFXManager.instance.PlaySFX(_spawnSFXIndex);
            
            // 间隔生成
            if (i < finalSpinObjectCount - 1)
                yield return new WaitForSeconds(_spawnInterval);
        }

        _cooldownTimer = finalCooldownTime;
        _isSpawning = false;
    }

    /// <summary>
    /// 生成单个环绕实体
    /// </summary>
    void SpawnSpinObject(int index, int totalCount, float radius, float speed, 
                        float damage, float knockback, float damageInterval, float duration)
    {
        // 计算初始角度，让环绕实体均匀分布
        float angle = (360f / totalCount) * index;
        Vector3 spawnPosition = transform.position + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;

        // 实例化环绕实体
        GameObject spinObject = Instantiate(spinObjectPrefab, spawnPosition, Quaternion.identity, transform);
        spinObject.SetActive(true);

        // 设置EnemyDamager参数
        EnemyDamager damager = spinObject.GetComponent<EnemyDamager>();
        if (damager != null)
        {
            damager.damage = damage;
            damager.knockBackForce = knockback;
            damager.timeBetweenDamage = _damageInterval;
            damager.lifeTime = _duration;
        }

        // 设置碰撞体半径
        CircleCollider2D collider = spinObject.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = radius * 0.2f; // 碰撞体半径为总半径的20%
        }

        // 初始化Animator组件（如果有的话）
        Animator animator = spinObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
            // 可以在这里设置动画的初始状态
            // animator.Play("Idle"); // 播放默认动画
        }

        // 添加旋转组件
        SpinObjectController spinController = spinObject.GetComponent<SpinObjectController>();
        if (spinController == null)
        {
            spinController = spinObject.AddComponent<SpinObjectController>();
        }
        spinController.Initialize(transform, radius, speed, angle, _duration);

        activeSpinObjects.Add(spinObject);
    }

    /// <summary>
    /// 清理活跃的环绕实体
    /// </summary>
    void ClearActiveSpinObjects()
    {
        foreach (GameObject obj in activeSpinObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        activeSpinObjects.Clear();
    }

    /// <summary>
    /// 用物理方法在射程内查找敌人，减少遍历数量
    /// </summary>
    GameObject FindNearestEnemy()
    {
        float searchRadius = _orbitRadius * 2f; // 搜索半径设为环绕半径的2倍
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float d = Vector3.Distance(transform.position, hit.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = hit.gameObject;
                }
            }
        }
        return nearest;
    }

    void OnDestroy()
    {
        ClearActiveSpinObjects();
    }
}

/// <summary>
/// 环绕实体控制器，控制单个环绕实体的旋转行为
/// </summary>
public class SpinObjectController : MonoBehaviour
{
    private Transform center;
    private float radius;
    private float speed;
    private float startAngle;
    private float duration;
    private float timer;
    private Animator animator;

    public void Initialize(Transform centerTransform, float orbitRadius, float orbitSpeed, float initialAngle, float lifeTime)
    {
        center = centerTransform;
        radius = orbitRadius;
        speed = orbitSpeed;
        startAngle = initialAngle;
        duration = lifeTime;
        timer = 0f;
        
        // 获取Animator组件
        animator = GetComponent<Animator>();
        
        // 如果有Animator组件，确保动画开始播放
        if (animator != null)
        {
            animator.enabled = true;
            // 可以在这里设置动画参数，比如播放速度等
            // animator.SetFloat("Speed", orbitSpeed / 100f); // 根据环绕速度调整动画速度
        }
    }

    void Update()
    {
        if (center == null) return;

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            // 在销毁前停止动画
            if (animator != null)
            {
                animator.enabled = false;
            }
            Destroy(gameObject);
            return;
        }

        // 计算当前角度
        float currentAngle = startAngle + (speed * timer);
        
        // 更新位置
        Vector3 newPosition = center.position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;
        transform.position = newPosition;
        
        // 可选：根据移动方向调整动画朝向
        // 如果你想让动画朝向移动方向，可以在这里调整transform.rotation
        // Vector3 direction = (newPosition - transform.position).normalized;
        // if (direction != Vector3.zero)
        // {
        //     transform.right = direction; // 或者使用transform.up = direction，取决于你的精灵朝向
        // }
    }
}
