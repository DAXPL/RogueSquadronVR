using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Lightsaber : NetworkBehaviour, IWeapon
{
    [SerializeField] private GameObject beam;
    [SerializeField] private float delay = 1;
    private float lastShootTimestamp;
    private NetworkVariable<bool> state = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private UnityEvent onActivate;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        state.OnValueChanged += OnStateChanged;
    }

    public void Shoot()
    {
        if (Time.time < lastShootTimestamp + delay) return;
        if(!IsOwner) return;
        lastShootTimestamp=Time.time;
        state.Value = !state.Value;
    }

    private void OnStateChanged(bool previousValue, bool newValue)
    {
        beam.SetActive(newValue);
        if(newValue) onActivate.Invoke();
    }

    public float Delay()
    {
        return delay;
    }
}
