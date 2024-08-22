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


}
