using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransformClient))]

public class NetworkXRGrabInteractable : XRGrabInteractable
{
    NetworkObject networkInstance;
    NetworkTransformClient networkTransform;
    InteractionLayerMask baseLayerMask;

    protected override void Awake()
    {
        base.Awake();
        networkInstance = GetComponent<NetworkObject>();
        networkTransform = GetComponent<NetworkTransformClient>();
        baseLayerMask = this.interactionLayers;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(networkTransform != null)
        {
            networkTransform.AskForOwnership();
        }
    }

    public void EnableGrip()
    {
        this.interactionLayers = baseLayerMask;
    }
    public void DisableGrip()
    {
        this.interactionLayers = 0;
    }
}
