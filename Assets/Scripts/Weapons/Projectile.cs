using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在弹药上的控制组件，负责设置弹药飞行速度和移动方向,飞行方向是向上，通过控制向上方向瞄准。
/// 20250624杜宜峰
/// </summary>
public class Projectile : MonoBehaviour
{
    public float MoveSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * MoveSpeed * Time.deltaTime;
    }
}