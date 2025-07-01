using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色默认朝向枚举
/// </summary>
public enum DefaultFacingDirection
{
    Left,   // 默认朝左
    Right   // 默认朝右
}

/// <summary>
/// 2D角色控制器，支持WASD移动和冲刺功能
/// 2025-06-24 肖沐奇 创建
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float _dashSpeedMultiplier = 1.5f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    [Header("朝向设置")]
    [SerializeField] private DefaultFacingDirection _defaultFacingDirection = DefaultFacingDirection.Right;
    [SerializeField] private Transform _rendererTransform;

    private float _moveSpeed = 5f;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection;
    private bool _canDash = true;
    private bool _isDashing = false;
    private bool _isLocked = false; // 锁定状态，死亡后锁定所有操作

    /// <summary>
    /// 锁定玩家操作（死亡时调用）
    /// </summary>
    public void LockPlayer()
    {
        _isLocked = true;
        _moveInput = Vector2.zero;
        _rb.velocity = Vector2.zero; // 立即停止移动
        _rb.simulated = false; // 停止物理模拟
        Debug.Log("玩家操作已锁定");
    }

    /// <summary>
    /// 解锁玩家操作
    /// </summary>
    public void UnlockPlayer()
    {
        _isLocked = false;
        Debug.Log("玩家操作已解锁");
    }

    /// <summary>
    /// 检查玩家是否被锁定
    /// </summary>
    public bool IsLocked => _isLocked;

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("PlayerController需要Rigidbody2D组件！");
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

        // 获取Animator组件
        _animator = _rendererTransform.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("在_rendererTransform上未找到Animator组件！");
        }

        // 根据默认朝向初始化_lastMoveDirection
        InitializeLastMoveDirection();
    }

    /// <summary>
    /// 根据默认朝向初始化最后移动方向
    /// </summary>
    private void InitializeLastMoveDirection()
    {
        switch (_defaultFacingDirection)
        {
            case DefaultFacingDirection.Left:
                _lastMoveDirection = Vector2.left;
                break;
            case DefaultFacingDirection.Right:
                _lastMoveDirection = Vector2.right;
                break;
        }
    }

    /// <summary>
    /// 每帧更新输入和移动
    /// </summary>
    void Update()
    {
        if (_isLocked) return; // 如果被锁定，不处理任何输入

        if (!_isDashing)
        {
            HandleMovementInput();
        }
        HandleDashInput();
        HandleHealInput();
        UpdateAnimationState();
    }

    /// <summary>
    /// 固定时间步长的物理更新
    /// </summary>
    void FixedUpdate()
    {
        if (_isLocked) return; // 如果被锁定，不处理移动

        if (!_isDashing)
        {
            Move();
        }
    }

    /// <summary>
    /// 处理治疗输入
    /// </summary>
    private void HandleHealInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Player player = GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("PlayerController需要Player组件！");
                return;
            }
            player.HealFromHealthBottle();
        }
    }

    /// <summary>
    /// 处理移动输入
    /// </summary>
    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D 或 左/右箭头
        float vertical = Input.GetAxisRaw("Vertical");     // W/S 或 上/下箭头

        _moveInput = new Vector2(horizontal, vertical).normalized;

        // 记录最后的移动方向（用于冲刺）
        if (_moveInput != Vector2.zero)
        {
            _lastMoveDirection = _moveInput;
            // 更新角色朝向
            UpdateFacingDirection();
        }
    }

    /// <summary>
    /// 处理冲刺输入
    /// </summary>
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash /*&& IsDashAvailable() 接口等待实现*/)
        {
            StartCoroutine(_CoDash());
        }
    }

    /// <summary>
    /// 执行移动
    /// </summary>
    private void Move()
    {
        Vector2 velocity = _moveInput * _moveSpeed;
        _rb.velocity = velocity;
    }

    /// <summary>
    /// 更新动画状态
    /// </summary>
    private void UpdateAnimationState()
    {
        if (_animator == null) return;

        // 检测是否有移动输入（WASD）
        bool hasMovementInput = _moveInput != Vector2.zero;

        // 如果有任何输入（移动或冲刺）则设置IsRunning为true
        bool isRunning = hasMovementInput || _isDashing;

        _animator.SetBool("IsRunning", isRunning);
    }

    /// <summary>
    /// 更新角色朝向
    /// </summary>
    private void UpdateFacingDirection()
    {
        if (_rendererTransform == null) return;

        // 只有在有水平移动时才更新朝向
        if (Mathf.Abs(_lastMoveDirection.x) > 0.1f)
        {
            bool isMovingRight = _lastMoveDirection.x > 0;
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
    /// 冲刺协程
    /// </summary>
    private IEnumerator _CoDash()
    {
        _canDash = false;
        _isDashing = true;

        // 应用冲刺速度（当前移速的倍数）
        _rb.velocity = _lastMoveDirection * (_moveSpeed * _dashSpeedMultiplier);

        // 等待冲刺持续时间
        yield return new WaitForSeconds(_dashDuration);

        _isDashing = false;

        // 等待冷却时间
        yield return new WaitForSeconds(_dashCooldown);

        _canDash = true;
    }

    /// <summary>
    /// 获取当前移动方向（用于外部脚本参考）
    /// </summary>
    public Vector2 GetMoveDirection()
    {
        return _lastMoveDirection;
    }

    /// <summary>
    /// 检查是否正在冲刺
    /// </summary>
    public bool IsDashing()
    {
        return _isDashing;
    }

    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }
}
