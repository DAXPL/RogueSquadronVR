using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DummyController : NetworkBehaviour, IDamageable
{
    private NetworkVariable<int> health = new NetworkVariable<int>(100,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private TextMeshPro healthOutput;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += onHealthChanged;
        if (healthOutput != null) healthOutput.SetText($"{health.Value}");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        health.OnValueChanged -= onHealthChanged;
    }

    public void Damage(int dmg)
    {
        DamageServerRpc(dmg);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DamageServerRpc(int dmg)
    {
        health.Value -= dmg;
        if(health.Value <= 0)
        {
            this.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void onHealthChanged(int previousValue, int newValue)
    {
        if (healthOutput != null) healthOutput.SetText($"{health.Value}");
    }

}
