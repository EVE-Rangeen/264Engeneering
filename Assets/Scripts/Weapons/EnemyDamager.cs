using UnityEngine;

/// <summary>
/// 挂载在弹药上的伤害组件，负责对敌人造成伤害，并根据设置决定是否销毁自身。
/// 2025-06-24杜宜峰
/// </summary>
public class EnemyDamager : MonoBehaviour
{
    [Header("伤害设置")]
    [Tooltip("对敌人造成的伤害数值")]
    public float damageAmount = 5f;
    [Tooltip("是否在击中敌人后销毁自身")]
    public bool destroyOnImpact = false;
    [Tooltip("弹药的生命周期，单位秒")]
    public float lifeTime = 2f;

    [Header("击退设置")]
    [Tooltip("是否对敌人造成击退效果")]
    public bool shouldKnockBack = false;
    [Tooltip("击退的力度")]
    public float knockBackForce = 5f;

    [Header("持续伤害设置")]
    [Tooltip("是否造成持续伤害")]
    public bool damageOverTime = false;
    [Tooltip("持续伤害间隔时间")]
    public float timeBetweenDamage = 0.5f;

    [Header("其他设置")]
    [Tooltip("是否销毁父物体")]
    public bool destroyParent = false;
    [Tooltip("生长速度")]
    public float growSpeed = 5f;

    [Header("敌人检测设置")]
    [Tooltip("敌人标签")]
    public string enemyTag = "Enemy";
    [Tooltip("敌人排序层级")]
    public string enemySortingLayer = "Enemy";
    [Tooltip("检测方式：0=Tag, 1=SortingLayer, 2=Both")]
    public int detectionMethod = 0;

    // 私有字段
    private float _timer;
    private Vector3 _targetSize;
    private float _damageCounter;
    private System.Collections.Generic.List<GameObject> _enemiesInRange = new System.Collections.Generic.List<GameObject>();
    private bool _shouldDestroy = false; // 是否应该销毁

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        _targetSize = transform.localScale;
        transform.localScale = Vector3.zero;
        
        // 如果生命周期设置为很大的值，则不自动销毁
        if (lifeTime > 100f)
        {
            _shouldDestroy = false;
        }
        else
        {
            _shouldDestroy = true;
        }
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    private void Update()
    {
        // 处理生长动画
        transform.localScale = Vector3.MoveTowards(transform.localScale, _targetSize, growSpeed * Time.deltaTime);

        // 只有在应该销毁时才处理生命周期
        if (_shouldDestroy)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                _targetSize = Vector3.zero;
                if (transform.localScale.x == 0f)
                {
                    Destroy(gameObject);
                    if (destroyParent)
                    {
                        Destroy(transform.parent.gameObject);
                    }
                }
            }
        }

        // 处理持续伤害
        if (damageOverTime)
        {
            _damageCounter -= Time.deltaTime;
            if (_damageCounter <= 0)
            {
                _damageCounter = timeBetweenDamage;
                for (int i = 0; i < _enemiesInRange.Count; i++)
                {
                    if (_enemiesInRange[i] != null)
                    {
                        DealDamage(_enemiesInRange[i]);
                    }
                    else
                    {
                        _enemiesInRange.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 进入触发器
    /// </summary>
    /// <param name="collision">碰撞的碰撞器</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!damageOverTime)
        {
            if (IsEnemy(collision.gameObject))
            {
                DealDamage(collision.gameObject);

                if (destroyOnImpact)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (IsEnemy(collision.gameObject))
            {
                if (!_enemiesInRange.Contains(collision.gameObject))
                {
                    _enemiesInRange.Add(collision.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 离开触发器
    /// </summary>
    /// <param name="collision">碰撞的碰撞器</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (damageOverTime)
        {
            if (IsEnemy(collision.gameObject))
            {
                _enemiesInRange.Remove(collision.gameObject);
            }
        }
    }

    /// <summary>
    /// 检查是否为敌人
    /// </summary>
    /// <param name="obj">要检查的对象</param>
    /// <returns>是否为敌人</returns>
    private bool IsEnemy(GameObject obj)
    {
        switch (detectionMethod)
        {
            case 0: // 仅使用Tag
                return obj.CompareTag(enemyTag);
            
            case 1: // 仅使用SortingLayer
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                return spriteRenderer != null && spriteRenderer.sortingLayerName == enemySortingLayer;
            
            case 2: // 同时使用Tag和SortingLayer
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                return obj.CompareTag(enemyTag) && sr != null && sr.sortingLayerName == enemySortingLayer;
            
            default:
                return obj.CompareTag(enemyTag);
        }
    }

    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    /// <param name="enemy">敌人对象</param>
    private void DealDamage(GameObject enemy)
    {
        //TakeDamage(damageAmount);
        Debug.Log($"对敌人 {enemy.name} 造成 {damageAmount} 点伤害");
        
        // 应用击退效果
        if (shouldKnockBack)
        {
            ApplyKnockBack(enemy);
        }
    }

    /// <summary>
    /// 对敌人应用击退效果
    /// </summary>
    /// <param name="enemy">敌人对象</param>
    private void ApplyKnockBack(GameObject enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            // 计算击退方向（从弹药到敌人的方向）
            Vector2 knockBackDirection = (enemy.transform.position - transform.position).normalized;
            
            // 应用击退力
            enemyRb.AddForce(knockBackDirection * knockBackForce, ForceMode2D.Impulse);
            
            Debug.Log($"对敌人施加了击退效果，力度：{knockBackForce}");
        }
    }

    /// <summary>
    /// 获取武器对怪物造成的伤害 返回伤害数值
    /// </summary>
    
    public float GetAppliedDamageAmount()
    {
        return damageAmount;
    }
}