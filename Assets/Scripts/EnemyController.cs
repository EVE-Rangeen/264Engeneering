using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人控制器，负责敌人的移动AI
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class EnemyController : MonoBehaviour
{
    private float _moveSpeed = 1f;

    [Header("朝向设置")]
    [SerializeField] private DefaultFacingDirection _defaultFacingDirection = DefaultFacingDirection.Right;
    [SerializeField] private Transform _rendererTransform;

    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("EnemyController需要Rigidbody2D组件！");
        }
        else
        {
            // 设置插值模式使得显示更加平滑
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        // 如果没有指定朝向Transform，则使用自身Transform
        if (_rendererTransform == null)
        {
            _rendererTransform = transform;
        }

        // 根据默认朝向初始化移动方向
        InitializeMoveDirection();
    }

    /// <summary>
    /// 根据默认朝向初始化移动方向
    /// </summary>
    private void InitializeMoveDirection()
    {
        switch (_defaultFacingDirection)
        {
            case DefaultFacingDirection.Left:
                _moveDirection = Vector2.left;
                break;
            case DefaultFacingDirection.Right:
                _moveDirection = Vector2.right;
                break;
        }
    }

    /// <summary>
    /// 查找玩家引用
    /// </summary>
    void Start()
    {
        // 通过标签查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("未找到标签为'Player'的游戏对象！请确保玩家对象设置了Player标签。");
        }
    }

    /// <summary>
    /// 每帧计算移动方向
    /// </summary>
    void Update()
    {
        if (_playerTransform != null)
        {
            CalculateMoveDirection();
        }
    }

    /// <summary>
    /// 固定时间步长的物理更新
    /// </summary>
    void FixedUpdate()
    {
        if (_playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// 计算朝向玩家的移动方向
    /// </summary>
    private void CalculateMoveDirection()
    {
        Vector3 direction = _playerTransform.position - transform.position;
        _moveDirection = direction.normalized;

        // 更新敌人朝向
        UpdateFacingDirection();
    }

    /// <summary>
    /// 移动向玩家
    /// </summary>
    private void MoveTowardsPlayer()
    {
        Vector2 velocity = _moveDirection * _moveSpeed;
        _rb.velocity = velocity;
    }

    /// <summary>
    /// 更新敌人朝向
    /// </summary>
    private void UpdateFacingDirection()
    {
        if (_rendererTransform == null) return;

        // 只有在有水平移动时才更新朝向
        if (Mathf.Abs(_moveDirection.x) > 0.1f)
        {
            bool isMovingRight = _moveDirection.x > 0;
            Vector3 currentScale = _rendererTransform.localScale;

            switch (_defaultFacingDirection)
            {
                case DefaultFacingDirection.Left:
                    // 默认朝左：向左移动时保持原始x缩放，向右移动时翻转x缩放
                    if (isMovingRight)
                    {
                        currentScale.x = -Mathf.Abs(currentScale.x);
                    }
                    else
                    {
                        currentScale.x = Mathf.Abs(currentScale.x);
                    }
                    break;

                case DefaultFacingDirection.Right:
                    // 默认朝右：向右移动时保持原始x缩放，向左移动时翻转x缩放
                    if (isMovingRight)
                    {
                        currentScale.x = Mathf.Abs(currentScale.x);
                    }
                    else
                    {
                        currentScale.x = -Mathf.Abs(currentScale.x);
                    }
                    break;
            }

            _rendererTransform.localScale = currentScale;
        }
    }

    /// <summary>
    /// 设置移动速度（供外部调用）
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    /// <summary>
    /// 获取当前移动方向（供外部调用）
    /// </summary>
    public Vector2 GetMoveDirection()
    {
        return _moveDirection;
    }
}
