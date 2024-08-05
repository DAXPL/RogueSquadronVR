using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class UniversalRemoteProcedureCall : NetworkBehaviour
{
    [SerializeField] private UnityEvent servertEventToRaise;
    [SerializeField] private UnityEvent clientEventToRaise;

    [ContextMenu("Click")]
    public void OnButtonClicked()
    {
        UniversalServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UniversalServerRpc()
    {
        servertEventToRaise.Invoke();
        if(clientEventToRaise.GetPersistentEventCount()>0)
            UniversalClientRpc();
    }

    [ClientRpc]
    private void UniversalClientRpc()
    {
        clientEventToRaise.Invoke();
    }
}
