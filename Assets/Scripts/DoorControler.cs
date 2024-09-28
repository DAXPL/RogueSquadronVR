using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorControler : NetworkBehaviour
{
    private NetworkVariable<bool> doorState = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private Animator animator;
    private AudioSource audioSource;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        doorState.OnValueChanged += onDoorstateChanged;
    }

    private void onDoorstateChanged(bool previousValue, bool newValue)
    {
        if(previousValue == newValue) return;
        if(animator)animator.SetBool("DoorState",newValue);
        if(audioSource) audioSource.Play();
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
