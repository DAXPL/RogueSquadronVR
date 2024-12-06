using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SignalRestorationController : Serviceable
{
    [SerializeField] private AntennaController[] antennas;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var antenna in antennas)
        {
            antenna.OnStatusChanged += OnAntennStatusChange;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkSpawn();
        foreach (var antenna in antennas)
        {
            antenna.OnStatusChanged -= OnAntennStatusChange;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        if (!IsServer) return;
        base.DamageServerRpc();
        foreach (var antenna in antennas)
        {
            antenna.Damage();
        }
    }

    private void OnAntennStatusChange()
    {
        if (!IsServer) return;
        if (IsOperative()) return;
        foreach (var antenna in antennas)
        {
            if (antenna.IsOperative() == false) return;
        }
        FixServerRpc();
    }
}
