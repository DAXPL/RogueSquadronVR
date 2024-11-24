using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OperationWrathfall : Serviceable
{
    [SerializeField] private Fabricator[] fabricators;
    int points = 0;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var fab in fabricators)
        {
            if (fab == null) continue;
            points++;
            fab.OnFabricatorDestroyed += OnFabDestroyed;
            fab.gameObject.SetActive(false);
        }
        if(IsServer && StarshipManager.Instance) StarshipManager.Instance.LockTravel(true);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (var fab in fabricators)
        {
            if (fab == null) continue;
            fab.OnFabricatorDestroyed -= OnFabDestroyed;
        }
        HarvexAdversary[] adversary = FindObjectsOfType<HarvexAdversary>();
        if (IsServer)
        {
            foreach (var ad in adversary)
            {
                ad.DamageServerRpc(1000);
            }
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        if (!IsServer) return;
        base.DamageServerRpc();
        foreach (var fab in fabricators)
        {
            if(fab == null) continue;
            fab.gameObject.SetActive(true);
        }
        if (IsServer && StarshipManager.Instance) StarshipManager.Instance.LockTravel(true);
    }
   
    [ServerRpc(RequireOwnership = false)]
    protected override void FixServerRpc()
    {
        base.FixServerRpc();
        if (IsServer && StarshipManager.Instance) StarshipManager.Instance.LockTravel(false);
    }

    private void OnFabDestroyed()
    {
        points--;
        if(points <= 0)
        {
            FixServerRpc();
        }
    }
}
