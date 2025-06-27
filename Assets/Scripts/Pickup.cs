using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拾取物基类，实现拾取动画效果
/// </summary>
public class Pickup : MonoBehaviour
{
    [Header("拾取动画设置")]
    [SerializeField] private float _flyOutDistance = 0.5f; // 飞出距离
    [SerializeField] private float _initialAcceleration = 5f; // 初始加速度
    [SerializeField] private float _attractionAcceleration = 10f; // 向玩家的吸引加速度
    [SerializeField] private float _velocityDamping = 3f; // 速度阻尼加速度，防止绕圈

    protected bool _isPickedUp = false;
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

    /// <summary>
    /// 拾取方法，子类应该重写此方法执行具体逻辑，然后调用base.PickedUp()
    /// </summary>
    public virtual void PickedUp()
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
        bool isInAttractPhase = false; // 是否进入吸引阶段

        // 获取玩家移动方向（如果玩家有PlayerController组件）
        Vector2 playerDirection = GetPlayerMovementDirection();

        // 如果玩家没有移动方向，则使用随机方向
        if (playerDirection == Vector2.zero)
        {
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            playerDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        }

        while (_player != null)
        {
            Vector3 acceleration;

            if (!isInAttractPhase && distanceTraveled < _flyOutDistance)
            {
                // 第一阶段：向玩家移动方向加速
                acceleration = (Vector3)playerDirection * _initialAcceleration;
            }
            else
            {
                // 第二阶段：向玩家吸引 + 速度阻尼
                if (!isInAttractPhase)
                {
                    isInAttractPhase = true;
                    // 可以在这里添加阶段切换的视觉效果
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
            if (!isInAttractPhase)
            {
                distanceTraveled += Vector3.Distance(transform.position, newPosition);
            }

            transform.position = newPosition;

            // 检查是否足够接近玩家
            if (Vector3.Distance(transform.position, _player.transform.position) <= 0.3f)
            {
                break;
            }

            yield return null;
        }

        // 销毁物体
        Destroy(gameObject);
    }

    /// <summary>
    /// 获取玩家移动方向
    /// </summary>
    private Vector2 GetPlayerMovementDirection()
    {
        PlayerController playerController = _player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // 尝试获取玩家当前输入方向
            return playerController.GetMoveDirection();
        }

        // 如果没有输入，返回零向量
        return Vector2.zero;
    }

    /// <summary>
    /// 检测与玩家的碰撞
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isPickedUp && other.CompareTag("Player"))
        {
            // 如果正在执行拾取动画且碰到玩家，立即销毁
            Destroy(gameObject);
        }
    }
}
