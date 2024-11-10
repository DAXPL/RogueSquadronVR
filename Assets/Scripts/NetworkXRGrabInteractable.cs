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
    LayerMask baseLayerMask;

    protected override void Awake()
    {
        base.Awake();
        networkInstance = GetComponent<NetworkObject>();
        networkTransform = GetComponent<NetworkTransformClient>();
        baseLayerMask = gameObject.layer;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(networkTransform != null)
        {
            networkTransform.AskForOwnership();
            string baseLayerName = LayerMask.LayerToName(baseLayerMask);
            //networkTransform.ChangeLayerServerRpc("NetworkGrabbed", "LocalGrabbed", NetworkManager.Singleton.LocalClientId);
            networkTransform.ChangeLayerServerRpc("NetworkGrabbed", baseLayerName, NetworkManager.Singleton.LocalClientId);
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (networkTransform != null)
        {
            string baseLayerName = LayerMask.LayerToName(baseLayerMask);
            networkTransform.ChangeLayerServerRpc(baseLayerName, baseLayerName, 0);
        }
    }
}
