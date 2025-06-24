using UnityEngine;

/// <summary>
/// 挂载在弹药上的伤害组件，负责对敌人造成伤害，并根据设置决定是否销毁自身。
/// 2025-06-24杜宜峰
/// </summary>
public class EnemyDamager : MonoBehaviour
{
    [Header("伤害设置")]

    [Tooltip("对敌人造成的伤害数值")]
    public float DamageAmount = 5f;
    [Tooltip("是否在击中敌人后销毁自身")]
    public bool DestroyOnHit = true;
    [Tooltip("弹药的生命周期，单位秒")]
    public float Lifetime = 2f;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= Lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // var enemy = other.GetComponent<EnemyController>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(DamageAmount);
            // }
            Debug.Log($"对敌人造成了{DamageAmount}点伤害");

            if (DestroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }


    /// 获取射弹对怪物造成的伤害
    public float GetProjectileDamage()
    {
        return DamageAmount;
        Debug.Log($"返回数值：造成了{DamageAmount}点伤害");
    }

}