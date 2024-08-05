using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorControler : NetworkBehaviour
{
    private NetworkVariable<bool> doorState = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animator = GetComponent<Animator>();
        doorState.OnValueChanged += onDoorstateChanged;
    }

    private void onDoorstateChanged(bool previousValue, bool newValue)
    {
        animator.SetBool("DoorState",newValue);
    }

    public void SetDoorState(bool newState)
    {
        if(IsServer) doorState.Value = newState;
    }
    public void ToggleDoorState()
    {
        if (IsServer) doorState.Value = !doorState.Value;
    }
}
