using UnityEngine;

public class FireAttackZone : MonoBehaviour
{
    [Header("攻击属性")]
    public float damage = 15f;
    public float attackRange = 5f;
    public float attackAngle = 90f;

    // 这个函数将由动画事件调用
    public void DealDamageInCone()
    {
        // 攻击方向就是这个对象自己的正方向 (通常是x轴正方向，取决于你的美术素材)
        Vector2 attackDirection = transform.right;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector2 directionToEnemy = enemy.transform.position - transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;

            if (distanceToEnemy <= attackRange)
            {
                if (Vector2.Angle(attackDirection, directionToEnemy) <= attackAngle / 2)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage);
                    }
                }
            }
        }
    }

    // 动画播放结束后，通过动画事件调用此函数来隐藏自己
    public void HideZone()
    {
        gameObject.SetActive(false);
    }

    // 在编辑器中绘制攻击范围，方便调试
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 direction = transform.right; // 使用对象的朝向

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * attackRange);

        float halfAngle = attackAngle / 2;
        Quaternion upRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.forward);
        Quaternion downRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.forward);
        Vector3 upRayDirection = upRayRotation * direction * attackRange;
        Vector3 downRayDirection = downRayRotation * direction * attackRange;

        Gizmos.DrawRay(transform.position, upRayDirection);
        Gizmos.DrawRay(transform.position, downRayDirection);
    }
}