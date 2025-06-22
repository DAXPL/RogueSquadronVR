using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor.PackageManager;
using UnityEngine;

public class NetworkTransformClient : NetworkTransform
{
    // Nadpisanie metody OnIsServerAuthoritative, która wskazuje, ¿e serwer nie jest autorytatywny w przypadku tego obiektu.
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }


    // Metoda do za¿¹dania w³asnoœci obiektu przez klienta.
    public void AskForOwnership()
    {
        // Sprawdzenie, czy klient ju¿ jest w³aœcicielem.
        if (this.IsOwner) return;
        // Wywo³anie RPC serwera, aby za¿¹daæ w³asnoœci.
        AskForOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    // Serwerowa metoda wywo³ywana przez klienta w celu za¿¹dania w³asnoœci.
    // Parametr newOwnerID to ID klienta, który ma staæ siê w³aœcicielem obiektu.
    [ServerRpc(RequireOwnership = false)]// Ustawienie RequireOwnership na false oznacza, ¿e metoda nie wymaga bycia w³aœcicielem.
    private void AskForOwnershipServerRpc(ulong newOwnerID)
    {
        Debug.Log($"Ownership of {transform.name} granted to {newOwnerID}");
        // Zmiana w³aœciciela obiektu.
        GetComponent<NetworkObject>().ChangeOwnership(newOwnerID);
    }

    // Metoda RPC wywo³ywana przez klienta w celu synchronizacji zmiany warstwy
    [ServerRpc(RequireOwnership =false)]
    public void ChangeLayerServerRpc(string newLayermask, string localNewLayermask, ulong localPlayerID)
    {
        ChangeLayerClientRpc(newLayermask, localNewLayermask, localPlayerID);
    }
    // Wywo³anie metody RPC na wszystkich klientach, aby zaktualizowaæ warstwê obiektu.
    [ClientRpc]
    public void ChangeLayerClientRpc(string newLayermask, string localNewLayermask, ulong localPlayerID)
    {
        ulong localClient = NetworkManager.Singleton.LocalClientId;
        gameObject.layer = LayerMask.NameToLayer((localClient == localPlayerID) ? localNewLayermask: newLayermask);
    }

}
