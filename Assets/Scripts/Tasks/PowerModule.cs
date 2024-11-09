using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PowerModule : NetworkBehaviour
{
    public NetworkVariable<int> power = new NetworkVariable<int>(50, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private TextMeshPro powerText;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        power.OnValueChanged += OnPowerLevelChange;
    }

    private void OnPowerLevelChange(int previousValue, int newValue)
    {
        if(powerText != null)
            powerText.SetText($"{newValue}");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        power.OnValueChanged -= OnPowerLevelChange;
    }

    public void LoadModule(int val, int maxPower)
    {
        if (!IsServer) return;
        power.Value = Mathf.Clamp(power.Value + val, 0, maxPower);
    }
    [ContextMenu("DischargeBattery")]
    public void DischargeBattery()
    {
        if (!IsServer) return;
        power.Value = 0;
    }
}