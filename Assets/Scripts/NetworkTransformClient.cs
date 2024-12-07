using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkTransformClient : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    public void AskForOwnership()
    {
        if(this.IsOwner) return;
        AskForOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AskForOwnershipServerRpc(ulong newOwnerID)
    {
        Debug.Log($"Ownership of {transform.name} granted to {newOwnerID}");
        GetComponent<NetworkObject>().ChangeOwnership(newOwnerID);
    }


    [ServerRpc(RequireOwnership =false)]
    public void ChangeLayerServerRpc(string newLayermask, string localNewLayermask, ulong localPlayerID)
    {
        ChangeLayerClientRpc(newLayermask, localNewLayermask, localPlayerID);
    }
    [ClientRpc]
    public void ChangeLayerClientRpc(string newLayermask, string localNewLayermask, ulong localPlayerID)
    {
        ulong localClient = NetworkManager.Singleton.LocalClientId;
        gameObject.layer = LayerMask.NameToLayer((localClient == localPlayerID) ? localNewLayermask: newLayermask);
    }

}
