using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Serviceable : NetworkBehaviour
{
    protected NetworkVariable<bool> isOperative = new NetworkVariable<bool>(true,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] protected UnityEvent onFix;
    [SerializeField] protected UnityEvent onDamage;

    public delegate void OnStatusChangedDelegate();
    public OnStatusChangedDelegate OnStatusChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isOperative.OnValueChanged += onStatusChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        isOperative.OnValueChanged -= onStatusChanged;
    }

    private void onStatusChanged(bool previousValue, bool newValue)
    {
        if (previousValue == newValue) return;
        if(newValue) onFix.Invoke();
        else onDamage.Invoke();

        OnStatusChanged.Invoke();
    }

    [ContextMenu("FIX")]
    public virtual void Fix() 
    {
        FixServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    protected void FixServerRpc()
    {
        if (IsServer)
            isOperative.Value = true;
    }

    [ContextMenu("Damage")]
    public virtual void Damage() 
    {
        DamageServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    protected void DamageServerRpc()
    {
        if (IsServer)
            isOperative.Value = false;
    }
    public virtual bool IsOperative() 
    { 
        return isOperative.Value; 
    }
}
