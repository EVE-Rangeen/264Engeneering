using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///射弹武器的脚本，控制射弹武器的射击行为。子弹需要挂载EnemyDamager组件并且默认inactive，作为Weapon的子物体。
///2025-06-24杜宜峰
/// </summary>
public class ProjectileWeapon : MonoBehaviour
{
    //public EnemyDamager damager; // 仅用于Inspector拖拽预制体引用
    [SerializeField] private GameObject projectilePrefab; // 仅用于Inspector拖拽预制体引用

    [Header("武器索引")]
    [SerializeField] private int weaponIndex = 0;

    [Header("射弹武器参数")]
    [Tooltip("基础弹夹子弹数量")]
    [SerializeField] private int clipSize = 6; // 基础弹夹大小
    [Tooltip("射击距离")]
    [SerializeField] private float range = 10f;
    [Tooltip("射击间隔")]
    [SerializeField] private float cooldown = 2f;
    [Tooltip("每颗子弹间隔时间")]
    [SerializeField] private float fireInterval = 0.07f; // 每颗子弹间隔时间，单位秒

    [SerializeField] private int currentClipSize; //测试用显示当前弹夹大小

    [Header("音效")]
    [Tooltip("射击音效的索引值（SFXManager中数组对应的音效的索引）")]
    [SerializeField] private int fireSFXIndex = 0;
    private float cooldownTimer = 0f;
    private bool isFiring = false;
    //private int currentClipSize; // 当前实际弹夹大小（基础值+增量）




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
            currentClipSize = weaponData.AttackCount;
        }
        else
        {
            currentClipSize = 1; // 默认值
            Debug.LogError($"ProjectileWeapon: weaponIndex({weaponIndex})超出范围");
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f || isFiring) return;

        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        float dist = Vector3.Distance(transform.position, nearestEnemy.transform.position);
        if (dist > range) return;

        // 启动速射协程
        StartCoroutine(FireBurst(nearestEnemy.transform.position));
    }

    IEnumerator FireBurst(Vector3 targetPos)
    {
        isFiring = true;
        for (int i = 0; i < currentClipSize; i++)
        {
            FireBullet(targetPos);
            if (i < currentClipSize - 1)
                yield return new WaitForSeconds(fireInterval);
        }
        cooldownTimer = cooldown;
        isFiring = false;
    }


    /// <summary>
    /// 开火函数 并且计算各种数值传参给EnemyDamager和Projectile
    /// </summary>
    void FireBullet(Vector3 targetPos)
    {


        //每次开火的时候计算各种数值传参给EnemyDamager
        WeaponData weaponData = WeaponManager.instance.CurrentWeapons[weaponIndex];

        //计算最终伤害
        float damage = weaponData.Damage;
        float finalDamage = damage * PlayerAttributeManager.instance.PlayerComponent.PowerFactor;

        //计算最终击退力度
        float knockBackForce = weaponData.Knockback;
        float finalKnockBackForce = knockBackForce;

        //获取持续伤害间隔时间
        float timeBetweenDamage = weaponData.BulletInterval;
        float finalTimeBetweenDamage = timeBetweenDamage;

        //获取子弹飞行速度
        float projectileSpeed = weaponData.ProjectileSpeed;
        float finalProjectileSpeed = projectileSpeed;

        // 从Playerattributemanager获取武器参数并更新到当前脚本

        //计算当前弹夹大小
        currentClipSize = PlayerAttributeManager.instance.PlayerComponent.ProjectileAmountIncrement + weaponData.AttackCount; // 攻击数量对应弹夹大小

        //计算最终射击距离
        range = weaponData.AttackRange * (1 + PlayerAttributeManager.instance.PlayerComponent.AttackAreaFactor); // 攻击范围对应射击距离

        //计算最终冷却时间
        cooldown = weaponData.CooldownTime * (1 - PlayerAttributeManager.instance.PlayerComponent.CooldownReductionFactor); // 冷却时间

        // //测试用 计算冷却时间
        // cooldown = weaponData.CooldownTime;

        fireInterval = weaponData.BulletInterval; // 子弹间隔时间



        // 用projectilePrefab作为预制体
        GameObject bulletObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity, null);
        Vector3 dir = (targetPos - transform.position).normalized;
        bulletObj.transform.up = dir; // 让子弹朝向目标
        bulletObj.SetActive(true);

        // 获取Projectile组件
        Projectile proj = bulletObj.GetComponent<Projectile>();

        //获取EnemyDamager组件
        EnemyDamager enemyDamager = bulletObj.GetComponent<EnemyDamager>();

        //传参给EnemyDamager
        enemyDamager.damage = finalDamage;
        enemyDamager.knockBackForce = finalKnockBackForce;
        enemyDamager.timeBetweenDamage = finalTimeBetweenDamage;
        enemyDamager.lifeTime = weaponData.Duration;

        //传参给Projectile
        proj.MoveSpeed = finalProjectileSpeed;

        //播放音效
        SFXManager.instance.PlaySFXPitched(fireSFXIndex);
    }



    /// <summary>
    /// 用物理方法在射程内查找敌人，减少遍历数量
    /// </summary>
    GameObject FindNearestEnemy()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
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




}
