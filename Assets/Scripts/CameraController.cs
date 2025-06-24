using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D相机控制器，实现平滑跟随玩家移动
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("跟随设置")]
    [SerializeField] private Transform _target; // 跟随目标（玩家）
    [SerializeField] private float _followSpeed = 5f; // 跟随速度
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -10); // 相机偏移量

    [Header("平滑设置")]
    [SerializeField] private bool _useSmoothing = true; // 是否使用平滑跟随
    [SerializeField] private float _smoothTime = 0.3f; // 平滑时间

    private Vector3 _velocity = Vector3.zero; // 用于SmoothDamp的速度引用

    /// <summary>
    /// 初始化，自动查找玩家对象
    /// </summary>
    void Start()
    {
        // 如果没有手动设置目标，尝试自动查找玩家
        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraController：未找到目标对象！请手动设置Target或给玩家添加'Player'标签。");
            }
        }
    }

    /// <summary>
    /// 在LateUpdate中更新相机位置，确保在玩家移动后执行
    /// </summary>
    void LateUpdate()
    {
        if (_target == null) return;

        FollowTarget();
    }

    /// <summary>
    /// 跟随目标对象
    /// </summary>
    private void FollowTarget()
    {
        Vector3 targetPosition = _target.position + _offset;

        // 选择跟随方式
        if (_useSmoothing)
        {
            // 平滑跟随
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
        }
        else
        {
            // 线性插值跟随
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 立即移动到目标位置（无平滑效果）
    /// </summary>
    public void SnapToTarget()
    {
        if (_target != null)
        {
            Vector3 targetPosition = _target.position + _offset;
            transform.position = targetPosition;
            _velocity = Vector3.zero; // 重置速度
        }
    }

    /// <summary>
    /// 设置新的跟随目标
    /// </summary>
    /// <param name="newTarget">新的目标对象</param>
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    /// <summary>
    /// 设置相机偏移量
    /// </summary>
    /// <param name="newOffset">新的偏移量</param>
    public void SetOffset(Vector3 newOffset)
    {
        _offset = newOffset;
    }

    /// <summary>
    /// 设置跟随速度
    /// </summary>
    /// <param name="speed">新的跟随速度</param>
    public void SetFollowSpeed(float speed)
    {
        _followSpeed = Mathf.Max(0f, speed);
    }
}
