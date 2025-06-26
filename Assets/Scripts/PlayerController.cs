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
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    [Header("朝向设置")]
    [SerializeField] private DefaultFacingDirection _defaultFacingDirection = DefaultFacingDirection.Right;
    [SerializeField] private Transform _facingTransform;

    private float _moveSpeed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection;
    private bool _canDash = true;
    private bool _isDashing = false;

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
        if (_facingTransform == null)
        {
            _facingTransform = transform;
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
        if (!_isDashing)
        {
            HandleMovementInput();
        }
        HandleDashInput();
    }

    /// <summary>
    /// 固定时间步长的物理更新
    /// </summary>
    void FixedUpdate()
    {
        if (!_isDashing)
        {
            Move();
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
    /// 更新角色朝向
    /// </summary>
    private void UpdateFacingDirection()
    {
        if (_facingTransform == null) return;

        // 只有在有水平移动时才更新朝向
        if (Mathf.Abs(_lastMoveDirection.x) > 0.1f)
        {
            bool isMovingRight = _lastMoveDirection.x > 0;
            Vector3 currentScale = _facingTransform.localScale;

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

            _facingTransform.localScale = currentScale;
        }
    }

    /// <summary>
    /// 冲刺协程
    /// </summary>
    private IEnumerator _CoDash()
    {
        _canDash = false;
        _isDashing = true;

        // 应用冲刺速度
        _rb.velocity = _lastMoveDirection * _dashSpeed;

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
