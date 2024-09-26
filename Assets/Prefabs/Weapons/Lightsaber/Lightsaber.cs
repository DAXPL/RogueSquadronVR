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
    [SerializeField] private int baseDamage = 10;
    private float lastShootTimestamp;
    private NetworkVariable<bool> state = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private UnityEvent onActivate;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        state.OnValueChanged += OnStateChanged;
    }
    [ContextMenu("Toggle")]
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
    public void DropWeapon()
    {
        if (!IsOwner) return;
        state.Value = false;
    }
    public float Delay()
    {
        return delay;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(state.Value == false) return;
        //Only owner can initiate damage sequence
        if (IsOwner == false) return;
        if (collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(baseDamage);
        }

    }
}
