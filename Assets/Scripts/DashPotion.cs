using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPotion : Pickup
{
    protected override void DataUpdate()
    {
        PlayerResourceManager.instance.AddDashCharges();
    }
}
