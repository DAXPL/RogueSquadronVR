using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaterPumpController : Serviceable
{
    public delegate void OnWaterPumpStatusChangedDelegate();
    public OnWaterPumpStatusChangedDelegate OnStatusChanged;
    public NetworkVariable<int> health = new NetworkVariable<int>();
    private int fixTreshold = 100;

    protected override void onStatusChanged(bool previousValue, bool newValue)
    {
        base.onStatusChanged(previousValue, newValue);
        if(OnStatusChanged != null) OnStatusChanged();
    }
    [ServerRpc(RequireOwnership = false)]
    protected override void FixServerRpc()
    {
        if (!IsServer) return;
        base.FixServerRpc();
        health.Value = fixTreshold;
    }
    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        if (!IsServer) return;
        base.DamageServerRpc();
        health.Value = 0;
    }
    public void Fixing(int amount)
    {
        Debug.Log("Fixing");
        FixingServerRpc(amount);
    }
    [ServerRpc]
    public void FixingServerRpc(int amount)
    {
        health.Value += amount;
        if (health.Value >= fixTreshold) 
        {
            FixServerRpc();
        }
    }
}
