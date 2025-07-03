using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拾取物基类，实现拾取动画效果
/// 2025-07-02 肖沐奇 创建
/// </summary>
public class Pickup : MonoBehaviour
{
    [Header("拾取动画设置")]
    [SerializeField] private float _flyOutDistance = 0.3f; // 飞出距离
    [SerializeField] private float _initialAcceleration = 10f; // 初始加速度
    [SerializeField] private float _attractionAcceleration = 20f; // 向玩家的吸引加速度
    [SerializeField] private float _velocityDamping = 10f; // 速度阻尼加速度，防止绕圈

    protected bool _isPickedUp = false;
    private bool _isInAttractPhase = false; // 是否处于吸引阶段
    private Player _player;

    void Start()
    {
        // 查找玩家引用
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogError("未找到Player对象！");
        }
    }

    protected virtual void DataUpdate()
    {
        // 子类重写此方法，实现数据更新逻辑
        Debug.LogError("子类未重写DataUpdate方法！");
    }

    /// <summary>
    /// 拾取方法，子类应该重写此方法执行具体逻辑，然后调用base.PickedUp()
    /// </summary>
    public void PickedUp()
    {
        if (_isPickedUp) return;
        _isPickedUp = true;

        // 启动拾取动画协程
        StartCoroutine(_CoPickupAnimation());
    }

    /// <summary>
    /// 拾取动画协程：基于物理的加速度系统
    /// </summary>
    private IEnumerator _CoPickupAnimation()
    {
        if (_player == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Vector3 startPosition = transform.position;
        Vector3 velocity = Vector3.zero; // 初始速度为0
        float distanceTraveled = 0f; // 已飞行距离

        // 计算从玩家到拾取物的方向（远离玩家的方向）
        Vector3 flyOutDirection = (transform.position - _player.transform.position).normalized;

        while (_player != null)
        {
            Vector3 acceleration;

            if (!_isInAttractPhase && distanceTraveled < _flyOutDistance)
            {
                // 第一阶段：向远离玩家的方向加速
                acceleration = flyOutDirection * _initialAcceleration;
            }
            else
            {
                // 第二阶段：向玩家吸引 + 速度阻尼
                if (!_isInAttractPhase)
                {
                    _isInAttractPhase = true;
                    // 可以在这里添加阶段切换的视觉效果
                    Debug.Log($"拾取物 {gameObject.name} 进入吸引阶段");
                }

                // 计算从拾取物指向玩家的方向
                Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
                Vector3 attractionAccel = directionToPlayer * _attractionAcceleration;

                // 添加速度阻尼（向速度反方向施加加速度）
                Vector3 dampingAccel = Vector3.zero;
                if (velocity.magnitude > 0.1f) // 避免在速度很小时产生抖动
                {
                    Vector3 velocityDirection = velocity.normalized;
                    dampingAccel = -velocityDirection * _velocityDamping;
                }

                acceleration = attractionAccel + dampingAccel;
            }

            // 物理模拟：更新速度和位置
            velocity += acceleration * Time.deltaTime;
            Vector3 newPosition = transform.position + velocity * Time.deltaTime;

            // 更新已飞行距离（仅在第一阶段计算）
            if (!_isInAttractPhase)
            {
                distanceTraveled += Vector3.Distance(transform.position, newPosition);
            }

            transform.position = newPosition;

            yield return null;
        }
    }

    /// <summary>
    /// 检测与玩家的碰撞
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 更严格的条件：必须是已拾取状态、处于吸引阶段、且碰撞对象是玩家
        if (_isPickedUp && _isInAttractPhase && other.CompareTag("Player"))
        {
            DataUpdate();
            // 如果正在执行拾取动画且碰到玩家，立即销毁
            Destroy(gameObject);
        }
    }
}
