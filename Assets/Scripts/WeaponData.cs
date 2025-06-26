using UnityEngine;

/// <summary>
/// 武器类型枚举
/// </summary>
public enum WeaponType
{
    近战武器,    // Melee
    远程武器,    // Ranged
    魔法武器,    // Magic
    投掷武器     // Throwable
}

/// <summary>
/// 武器数据ScriptableObject
/// 用于存储武器的各种属性数据
/// </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/武器数据", order = 1)]
public class WeaponData : ScriptableObject
{
    [Header("基础信息")]
    [SerializeField] private string _weaponName = "新武器";
    [SerializeField] private Sprite _weaponIcon;
    [SerializeField] private WeaponType _weaponType = WeaponType.近战武器;
    [SerializeField, TextArea(2, 4)] private string _weaponDescription = "武器描述";
    [SerializeField] private int _currentLevel = 0;
    [SerializeField] private int _maxLevel = 10;

    [Header("伤害属性")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _attackRange = 1f;

    [Header("攻击效果")]
    [SerializeField] private int _attackCount = 1;
    [SerializeField] private int _piercing = 0;
    [SerializeField] private float _cooldownTime = 1f;
    [SerializeField] private float _knockback = 0f;

    [Header("弹道属性")]
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private float _bulletInterval = 0.1f;
    [SerializeField] private bool _destroyAfterShoot = false;

    /// <summary>
    /// 武器名称
    /// </summary>
    public string WeaponName => _weaponName;

    /// <summary>
    /// 武器图标
    /// </summary>
    public Sprite WeaponIcon => _weaponIcon;

    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType WeaponType => _weaponType;

    /// <summary>
    /// 武器描述
    /// </summary>
    public string WeaponDescription => _weaponDescription;

    /// <summary>
    /// 武器等级
    /// </summary>
    public int WeaponLevel => _currentLevel;

    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel => _currentLevel;

    /// <summary>
    /// 最大等级
    /// </summary>
    public int MaxLevel => _maxLevel;

    /// <summary>
    /// 是否已达到最大等级
    /// </summary>
    public bool IsMaxLevel => _currentLevel >= _maxLevel;

    /// <summary>
    /// 武器伤害
    /// </summary>
    public float Damage => _damage;

    /// <summary>
    /// 武器攻击速度（每秒攻击次数）
    /// </summary>
    public float AttackSpeed => _attackSpeed;

    /// <summary>
    /// 攻击范围
    /// </summary>
    public float AttackRange => _attackRange;

    /// <summary>
    /// 攻击数量（一次攻击产生的弹幕/攻击次数）
    /// </summary>
    public int AttackCount => _attackCount;

    /// <summary>
    /// 穿刺次数（0表示不穿刺）
    /// </summary>
    public int Piercing => _piercing;

    /// <summary>
    /// 冷却时间（秒）
    /// </summary>
    public float CooldownTime => _cooldownTime;

    /// <summary>
    /// 击退力度
    /// </summary>
    public float Knockback => _knockback;

    /// <summary>
    /// 弹射物飞行速度
    /// </summary>
    public float ProjectileSpeed => _projectileSpeed;

    /// <summary>
    /// 武器/弹射物持续时间（秒）
    /// </summary>
    public float Duration => _duration;

    /// <summary>
    /// 子弹发射间隔（秒）
    /// </summary>
    public float BulletInterval => _bulletInterval;

    /// <summary>
    /// 是否在射击后销毁武器
    /// </summary>
    public bool DestroyAfterShoot => _destroyAfterShoot;

    /// <summary>
    /// 获取武器的完整描述信息
    /// </summary>
    /// <returns>武器描述字符串</returns>
    public string GetWeaponDescription()
    {
        return $"武器名称: {_weaponName}\n" +
               $"武器类型: {_weaponType}\n" +
               $"等级: {_currentLevel}/{_maxLevel}\n" +
               $"伤害: {_damage}\n" +
               $"攻击速度: {_attackSpeed}/秒\n" +
               $"攻击范围: {_attackRange}\n" +
               $"攻击数量: {_attackCount}\n" +
               $"穿刺: {_piercing}\n" +
               $"冷却时间: {_cooldownTime}秒\n" +
               $"击退: {_knockback}\n" +
               $"弹射物速度: {_projectileSpeed}\n" +
               $"持续时间: {_duration}秒\n" +
               $"子弹间隔: {_bulletInterval}秒\n" +
               $"射击后销毁: {(_destroyAfterShoot ? "是" : "否")}";
    }

    /// <summary>
    /// 验证武器数据的有效性
    /// </summary>
    void OnValidate()
    {
        // 确保数值在合理范围内
        _damage = Mathf.Max(0f, _damage);
        _attackSpeed = Mathf.Max(0.1f, _attackSpeed);
        _attackRange = Mathf.Max(0f, _attackRange);
        _attackCount = Mathf.Max(1, _attackCount);
        _piercing = Mathf.Max(0, _piercing);
        _cooldownTime = Mathf.Max(0f, _cooldownTime);
        _knockback = Mathf.Max(0f, _knockback);

        // 验证弹道属性
        _projectileSpeed = Mathf.Max(0.1f, _projectileSpeed);
        _duration = Mathf.Max(0.1f, _duration);
        _bulletInterval = Mathf.Max(0.01f, _bulletInterval);

        // 确保武器等级至少为0
        _currentLevel = Mathf.Max(0, _currentLevel);
        _maxLevel = Mathf.Max(1, _maxLevel);
        
        // 确保当前等级不超过最大等级
        _currentLevel = Mathf.Min(_currentLevel, _maxLevel);

        // 确保武器名称不为空
        if (string.IsNullOrEmpty(_weaponName))
        {
            _weaponName = "未命名武器";
        }

        // 确保武器描述不为空
        if (string.IsNullOrEmpty(_weaponDescription))
        {
            _weaponDescription = "暂无描述";
        }
    }
} 