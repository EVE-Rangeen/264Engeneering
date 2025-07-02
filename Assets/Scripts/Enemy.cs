using System.Collections;
using UnityEngine;

public enum ExpType
{
    Normal,
    High,
    VeryHigh,
}

public enum MoneyType
{
    Coin,
    Purse,
}

/// <summary>
/// 敌人逻辑
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class Enemy : MonoBehaviour
{
    #region 敌人属性
    [SerializeField] private float _maxHealth = 100f; // 最大生命值
    [SerializeField] private float _health = 100f; // 当前生命值
    [SerializeField] private float _damage = 1f; // 攻击力,接触玩家时造成的伤害
    [SerializeField] private float _moveSpeed = 1f; // 移动速度，作用于EnemyController
    [SerializeField] private float _mass = 1f; // 质量，影响敌人被击退的距离
    #endregion

    #region 掉落物相关
    [SerializeField] private ExpType _expType = ExpType.Normal; // 经验类型
    [SerializeField] private MoneyType _moneyType = MoneyType.Coin; // 金钱类型
    [SerializeField] private float _dropPossibility = 0.8f; // 总掉落概率
    [SerializeField][Range(0f, 1f)] private float _expWeight = 0.7f; // 经验权重，0表示只掉落金币，1表示只掉落经验
    [SerializeField] private float _deathAnimationDuration = 1f; // 死亡动画持续时间
    [SerializeField] private float _healthPotionDropPossibility = 0.01f; // 血瓶掉落概率，0表示不掉落，1表示必定掉落
    [SerializeField] private float _dashPotionDropPossibility = 0.01f; // 冲刺瓶掉落概率，0表示不掉落，1表示必定掉落
    #endregion

    private EnemyController _enemyController;
    private Rigidbody2D _rb;
    private Animator _animator;
    private bool _isDead = false; // 敌人是否已死亡的标志

    // 受击闪烁效果相关
    private SpriteRenderer[] _spriteRenderers; // 所有的 SpriteRenderer 组件
    private Material[] _originalMaterials; // 存储原始材质
    private Material _flashMaterial; // 白色闪烁材质
    private bool _isFlashing = false; // 是否正在闪烁

    private void Awake()
    {
        //初始化血量为满血
        _health = _maxHealth;
        _enemyController = GetComponent<EnemyController>();
        if (_enemyController == null)
        {
            Debug.LogError("EnemyController组件未找到！");
        }
        _enemyController.SetMoveSpeed(_moveSpeed);

        //初始化敌人的质量
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("Enemy需要Rigidbody2D组件！");
        }
        else
        {
            _rb.mass = _mass;
        }

        //初始化Animator组件
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Enemy需要Animator组件来播放死亡动画！");
        }

        //初始化受击闪烁效果
        InitializeFlashEffect();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 如果碰撞对象是玩家，则对它造成伤害
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerComponent = collision.gameObject.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.TakeEnemyDamage(_damage);
            }
        }
    }

    /// <summary>
    /// 初始化受击闪烁效果
    /// </summary>
    private void InitializeFlashEffect()
    {
        // 获取敌人及其子对象的所有SpriteRenderer组件
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (_spriteRenderers.Length > 0)
        {
            // 保存所有SpriteRenderer的原始材质
            _originalMaterials = new Material[_spriteRenderers.Length];
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _originalMaterials[i] = _spriteRenderers[i].material;
            }

            // 创建白色闪烁材质
            CreateFlashMaterial();
        }
        else
        {
            Debug.LogWarning($"敌人 {gameObject.name} 没有找到SpriteRenderer组件，无法显示受击闪烁效果！");
        }
    }

    /// <summary>
    /// 创建白色闪烁材质
    /// </summary>
    private void CreateFlashMaterial()
    {
        // 创建一个新的材质实例，使用Unity内置的白色着色器
        _flashMaterial = new Material(Shader.Find("GUI/Text Shader"));
        _flashMaterial.color = Color.white;

        // 设置材质名称便于调试
        _flashMaterial.name = "Enemy Flash Material";
    }

    /// <summary>
    /// 受到来自玩家武器的伤害
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (_isDead) return; // 如果已经死亡，避免重复执行

        _health -= damage;

        // 触发受击闪烁效果
        if (!_isFlashing && _spriteRenderers != null && _spriteRenderers.Length > 0)
        {
            StartCoroutine(_CoFlashWhite());
        }

        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
    }

    /// <summary>
    /// 敌人死亡处理
    /// </summary>
    private void Die()
    {
        if (_isDead) return; // 如果已经死亡，避免重复执行

        _isDead = true;
        _health = 0;

        // 设置死亡动画参数
        if (_animator != null)
        {
            _animator.SetBool("IsDead", true);
        }

        // 停止敌人移动
        if (_enemyController != null)
        {
            _enemyController.SetMoveSpeed(0f);
        }

        // 启动死亡协程，先播放动画再掉落物品
        StartCoroutine(_CoHandleDeath());
    }

    /// <summary>
    /// 处理死亡流程的协程：播放动画 -> 掉落物品 -> 销毁对象
    /// </summary>
    /// <returns></returns>
    private IEnumerator _CoHandleDeath()
    {
        // 等待死亡动画播放完成
        yield return new WaitForSeconds(_deathAnimationDuration);

        // 首先判断是否掉落物品
        if (Random.Range(0f, 1f) < _dropPossibility)
        {
            // 根据权重决定掉落经验还是金币
            if (Random.Range(0f, 1f) < _expWeight)
            {
                SpawnExp();
            }
            else
            {
                SpawnMoney();
            }
        }

        if (Random.Range(0f, 1f) < _healthPotionDropPossibility)
        {
            SpawnHealthPotion();
        }

        if (Random.Range(0f, 1f) < _dashPotionDropPossibility)
        {
            SpawnDashPotion();
        }

        // 更新UI击败敌人计数
        UIController.defeatedEnemyCount++;

        // 销毁敌人对象
        Destroy(gameObject);
    }

    /// <summary>
    /// 生成经验Prefab
    /// </summary>
    private void SpawnExp()
    {
        string expPrefabPath = "";
        switch (_expType)
        {
            case ExpType.Normal:
                expPrefabPath = "Prefabs/NormalExp";
                break;
            case ExpType.High:
                expPrefabPath = "Prefabs/HighExp";
                break;
            case ExpType.VeryHigh:
                expPrefabPath = "Prefabs/VeryHighExp";
                break;
        }

        GameObject expPrefab = Resources.Load<GameObject>(expPrefabPath);
        if (expPrefab != null)
        {
            Instantiate(expPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"无法加载经验Prefab：{expPrefabPath}");
        }
    }

    /// <summary>
    /// 生成金币Prefab
    /// </summary>
    private void SpawnMoney()
    {
        string moneyPrefabPath = "";
        switch (_moneyType)
        {
            case MoneyType.Coin:
                moneyPrefabPath = "Prefabs/Coin";
                break;
            case MoneyType.Purse:
                moneyPrefabPath = "Prefabs/Purse";
                break;
        }

        GameObject moneyPrefab = Resources.Load<GameObject>(moneyPrefabPath);
        if (moneyPrefab != null)
        {
            Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"无法加载金币Prefab：{moneyPrefabPath}");
        }
    }

    ///<summary>
    /// 生成血瓶Prefab
    /// </summary>
    private void SpawnHealthPotion()
    {
        string healthPotionPrefabPath = "Prefabs/HealthPotion";
        GameObject healthPotionPrefab = Resources.Load<GameObject>(healthPotionPrefabPath);
        if (healthPotionPrefab != null)
        {
            Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"无法加载血瓶Prefab：{healthPotionPrefabPath}");
        }
    }

    /// <summary>
    /// 生成冲刺瓶Prefab
    /// </summary>
    private void SpawnDashPotion()
    {
        string dashPotionPrefabPath = "Prefabs/DashPotion";
        GameObject dashPotionPrefab = Resources.Load<GameObject>(dashPotionPrefabPath);
    }

    /// <summary>
    /// 受击时的白色闪烁效果协程（使用材质替换）
    /// </summary>
    /// <returns></returns>
    private IEnumerator _CoFlashWhite()
    {
        _isFlashing = true;

        // 将所有SpriteRenderer切换到白色材质
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null && _flashMaterial != null)
            {
                _spriteRenderers[i].material = _flashMaterial;
            }
        }

        // 等待0.1秒
        yield return new WaitForSeconds(0.05f);

        // 恢复原始材质
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null && _originalMaterials != null && i < _originalMaterials.Length)
            {
                _spriteRenderers[i].material = _originalMaterials[i];
            }
        }

        _isFlashing = false;
    }

    /// <summary>
    /// 清理材质资源
    /// </summary>
    private void OnDestroy()
    {
        // 销毁创建的材质实例，避免内存泄漏
        if (_flashMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(_flashMaterial);
            }
            else
            {
                DestroyImmediate(_flashMaterial);
            }
        }
    }
}
