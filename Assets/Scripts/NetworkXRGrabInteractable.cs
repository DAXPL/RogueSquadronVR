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
    InteractionLayerMask baseInteractionLayerMask;
    LayerMask baseLayerMask;

    protected override void Awake()
    {
        base.Awake();
        networkInstance = GetComponent<NetworkObject>();
        networkTransform = GetComponent<NetworkTransformClient>();
        baseInteractionLayerMask = this.interactionLayers;
        baseLayerMask = gameObject.layer;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(networkTransform != null)
        {
            networkTransform.AskForOwnership();

            networkTransform.ChangeLayerServerRpc("NetworkGrabbed", "LocalGrabbed", NetworkManager.Singleton.LocalClientId);
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (networkTransform != null)
        {
            string newLayer = LayerMask.LayerToName(baseLayerMask);
            networkTransform.ChangeLayerServerRpc(newLayer, newLayer,0);
        }
    }
}
