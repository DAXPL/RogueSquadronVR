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
    [SerializeField] private NetworkVariable<int> power = new NetworkVariable<int>(69, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private const int maxPowerLevel = 150;
    private const int usablePowerLevel = 100;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        power.OnValueChanged += OnPowerLevelChange;
        if (IsServer) power.Value = maxPowerLevel;
        OnPowerLevelChange(0, power.Value);
        
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        power.OnValueChanged -= OnPowerLevelChange;
    }

    public void SetDamageState()
    {
        Debug.Log("Destroyed");
        if (IsServer) power.Value = 0;
        base.Damage();
    }

    public int GetPowerLevel()
    {
        return power.Value;
    }

    public int ReducePowerLevel(int cost)
    {
        if (power.Value >= cost) // Jeœli reaktor ma wystarczaj¹co du¿o mocy, redukuj moc o koszt i zwróæ 0
        {
            power.Value -= cost;
            return 0;
        }
        else
        {
            int remainingCost = cost - power.Value; // Oblicz pozosta³y koszt, który musi byæ pokryty przez inne reaktory
            power.Value = 0;
            return remainingCost; // Zwróæ pozosta³y koszt
        }
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (!IsServer) return;
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
}
