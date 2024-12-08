using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AquarisMission : Serviceable
{
    [SerializeField] private WaterFarmController[] waterFarms;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var farm in waterFarms)
        {
            farm.OnStatusChanged += OnWaterFarmStatusChange;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (var farm in waterFarms)
        {
            farm.OnStatusChanged -= OnWaterFarmStatusChange;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        if (!IsServer) return;
        base.DamageServerRpc();
        foreach (var farm in waterFarms)
        {
            farm.Damage();
        }
    }

    private void OnWaterFarmStatusChange()
    {
        if(!IsServer) return;
        if(IsOperative())return;
        foreach (var farm in waterFarms)
        {
            if(farm && farm.IsOperative() == false) return;
        }
        FixServerRpc();
    }
}
