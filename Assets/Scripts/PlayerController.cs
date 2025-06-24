using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D角色控制器，支持WASD移动和冲刺功能
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection = Vector2.right; // 默认朝向右
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
        }
    }

    /// <summary>
    /// 处理冲刺输入
    /// </summary>
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash && IsDashAvailable())
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
}

//TODO: 假接口，等待外部实现
bool IsDashAvailable()
{
    return true;
}
