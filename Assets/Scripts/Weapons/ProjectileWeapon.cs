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

    [Header("左轮参数")]
    [Tooltip("每弹夹子弹数量")]
    [SerializeField] private int clipSize = 6;
    [Tooltip("射击距离")]
    [SerializeField] private float range = 10f;
    [Tooltip("射击间隔")]
    [SerializeField] private float cooldown = 2f;
    [Tooltip("每颗子弹间隔时间")]
    [SerializeField] private float fireInterval = 0.07f; // 每颗子弹间隔时间，单位秒

    private float cooldownTimer = 0f;
    private bool isFiring = false;

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
        for (int i = 0; i < clipSize; i++)
        {
            FireBullet(targetPos);
            if (i < clipSize - 1)
                yield return new WaitForSeconds(fireInterval);
        }
        cooldownTimer = cooldown;
        isFiring = false;
    }

    void FireBullet(Vector3 targetPos)
    {
        // 用projectilePrefab作为预制体
        GameObject bulletObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity, null);
        Vector3 dir = (targetPos - transform.position).normalized;
        bulletObj.transform.up = dir; // 让子弹朝向目标
        bulletObj.SetActive(true);
        // 获取Projectile组件（如需进一步初始化）
        Projectile proj = bulletObj.GetComponent<Projectile>();
        // 你可以在这里做一些额外的初始化，比如设置伤害等
        SFXManager.instance.PlaySFXPitched(0);
    }

    GameObject FindNearestEnemy()
    {
        // 用物理方法在射程内查找敌人，减少遍历数量
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
