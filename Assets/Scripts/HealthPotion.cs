using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Pickup
{
    protected override void DataUpdate()
    {
        PlayerResourceManager.instance.AddHealthPotions();
    }
}
