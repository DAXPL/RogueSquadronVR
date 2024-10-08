using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaterPumpController : Serviceable
{
    public delegate void OnWaterPumpStatusChangedDelegate();
    public OnWaterPumpStatusChangedDelegate OnStatusChanged;

    protected override void onStatusChanged(bool previousValue, bool newValue)
    {
        base.onStatusChanged(previousValue, newValue);
        OnStatusChanged();
    }
}
