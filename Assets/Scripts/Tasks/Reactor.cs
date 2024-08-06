using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reactor : Serviceable
{
    [SerializeField] private NetworkVariable<int> power = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private const int maxPowerLevel = 150;
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        if(!IsServer) return;
        if(args.interactableObject.transform.TryGetComponent(out PowerModule pm))
        {
            int neededPower = maxPowerLevel - power.Value;
            int pmPower = pm.power.Value;
            if(neededPower > pmPower)
            {
                power.Value += pmPower;
                pm.power.Value = 0;
            }
            else
            {
                power.Value += neededPower;
                pm.power.Value -= neededPower;
            }
        }
        Debug.Log($"New power level: {power.Value}");
    }
    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log($"{args.interactableObject.transform.gameObject.name}");
    }
}
