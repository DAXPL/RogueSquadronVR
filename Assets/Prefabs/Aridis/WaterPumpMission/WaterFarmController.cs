using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class WaterFarmController : Serviceable
{
    [SerializeField] private WaterPumpController[] waterPumps;
    [SerializeField] private TextMeshProUGUI stateOutput;

    public delegate void OnWaterFarmControllerStatusChangedDelegate();
    public OnWaterFarmControllerStatusChangedDelegate OnStatusChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var pump in waterPumps)
        {
            pump.OnStatusChanged+=OnWaterPumpStatusChange;
        }
        OnWaterPumpStatusChange();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (var pump in waterPumps)
        {
            pump.OnStatusChanged-=OnWaterPumpStatusChange;
        }
    }
  
    
    private void OnWaterPumpStatusChange()
    {
        int workingPumps = 0;
        foreach (var pump in waterPumps)
        {
            if(pump.IsOperative()) 
                workingPumps++;
        }
        stateOutput.SetText($"{workingPumps}/{waterPumps.Length}");
        if(workingPumps >= waterPumps.Length && IsServer)
        {
            FixServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        if(!IsServer) return;

        int damagedPumps = 0;
        foreach (var pump in waterPumps)
        {
            pump.Damage();
            damagedPumps++;
        }
        isOperative.Value = false;
        OnWaterPumpStatusChange();
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void FixServerRpc()
    {
        if (!IsServer) return;

        foreach (var pump in waterPumps)
        {
            pump.Fix();
        }
        isOperative.Value = true;
    }

   }
