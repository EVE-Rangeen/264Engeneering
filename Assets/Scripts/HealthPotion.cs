using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 血瓶
/// 2025-07-02 肖沐奇 创建
/// </summary>
public class HealthPotion : Pickup
{
    protected override void DataUpdate()
    {
        PlayerResourceManager.instance.AddHealthPotions();
    }
}
