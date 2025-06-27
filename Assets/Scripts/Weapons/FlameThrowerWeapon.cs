using System.Collections;
using UnityEngine;

/// <summary>
/// FlameThrowerWeapon 始终自动攻击，定时激活/关闭 attackZone，支持扇形范围，面向玩家朝向（+x方向）。
/// 杜宜峰2025-06-26
/// </summary>
public class FlameThrowerWeapon : MonoBehaviour
{
    [Header("攻击判定区域（需挂载EnemyDamager）")]
    [SerializeField] private GameObject attackZone;

    [Header("攻击参数")]
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float attackRange = 3.0f; // 扇形半径
    [SerializeField] private float attackAngle = 90f;  // 扇形角度

    private bool _isAttacking = false;

    void Start()
    {
        StartCoroutine(AutoAttackLoop());
    }

    private IEnumerator AutoAttackLoop()
    {
        while (true)
        {
            // 攻击前同步判定区朝向和范围
            UpdateAttackZoneTransform();
            if (attackZone != null)
                attackZone.SetActive(true);
            _isAttacking = true;
            yield return new WaitForSeconds(attackDuration);

            if (attackZone != null)
                attackZone.SetActive(false);
            _isAttacking = false;
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    /// <summary>
    /// 同步attackZone的位置、朝向和范围（扇形始终面向+X方向）
    /// </summary>
    private void UpdateAttackZoneTransform()
    {
        if (attackZone == null) return;
        // 让attackZone的位置和父物体一致，朝向+X
        attackZone.transform.position = transform.position;
        attackZone.transform.rotation = transform.rotation; // 保持与武器一致

        // 如果attackZone有自定义的扇形判定脚本或组件，更新其参数
        var cone = attackZone.GetComponent<FireAttackZone>();
        if (cone != null)
        {
            cone.attackRange = attackRange;
            cone.attackAngle = attackAngle;
        }
        // 如果用PolygonCollider2D自定义扇形，也可在此处动态设置其points
    }

    /// <summary>
    /// 外部可调用，动态设置攻击范围
    /// </summary>
    public void SetAttackRange(float range, float angle)
    {
        attackRange = range;
        attackAngle = angle;
    }
} 
