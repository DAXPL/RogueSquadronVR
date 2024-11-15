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
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (var fab in fabricators)
        {
            if (fab == null) continue;
            fab.OnFabricatorDestroyed -= OnFabDestroyed;
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
