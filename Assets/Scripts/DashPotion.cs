using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冲刺瓶
/// 2025-07-02 肖沐奇 创建
/// </summary>
public class DashPotion : Pickup
{
    protected override void DataUpdate()
    {
        PlayerResourceManager.instance.AddDashCharges();
    }
}
