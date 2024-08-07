using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reactor : Serviceable
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private NetworkVariable<int> power = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private const int maxPowerLevel = 150;
    private const int usablePowerLevel = 100;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        power.OnValueChanged += OnPowerLevelChange;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        power.OnValueChanged -= OnPowerLevelChange;
    }

    public void SetDamageState()
    {
        if (IsServer) power.Value = 0;
        base.Damage();
        powerText.SetText($"{power.Value}");
        powerText.color = power.Value >= usablePowerLevel ? Color.green : Color.red;
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        if(!IsServer) return;
        if(args.interactableObject.transform.TryGetComponent(out PowerModule pm))
        {
            int neededPower = maxPowerLevel - power.Value;
            int pmPower = pm.power.Value;
            if(neededPower >= pmPower)
            {
                power.Value += pmPower;
                pm.power.Value = 0;
            }
            else
            {
                power.Value += neededPower;
                pm.power.Value -= neededPower;
            }
            if (power.Value >= usablePowerLevel)
            {
                Fix();
            }
        }
    }

    private void OnPowerLevelChange(int previousValue, int newValue)
    {
        if (powerText == null) return;

        powerText.SetText($"{newValue}");
        powerText.color = newValue >= usablePowerLevel ? Color.green : Color.red;
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log($"{args.interactableObject.transform.gameObject.name}");
    }
}
