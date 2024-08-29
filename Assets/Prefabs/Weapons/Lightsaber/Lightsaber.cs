using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lightsaber : NetworkBehaviour, IWeapon
{
    [SerializeField] private GameObject beam;
    private NetworkVariable<bool> state = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        state.OnValueChanged += OnStateChanged;
    }

    public void Shoot()
    {
        ShootServerRPC();
    }

    [ServerRpc]
    private void ShootServerRPC()
    {
        state.Value = !state.Value;
    }

    private void OnStateChanged(bool previousValue, bool newValue)
    {
        beam.SetActive(newValue);
    }
}
