using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkRPCtest : NetworkBehaviour
{
    [SerializeField] private GameObject go;
    public void OnButtonClicked()
    {
        TestServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestServerRpc()
    {
        Debug.Log($"[SERWER] client {OwnerClientId} wants to do smth");
        TestClientRpc();
    }

    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log($"Server want me to do smth");
        go.SetActive(!go.activeSelf);
    }
}
