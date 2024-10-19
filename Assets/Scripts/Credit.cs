using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Credit : NetworkBehaviour
{
    [SerializeField] private int value;
    public void AddCredit()
    {
        MagazineController mc = MagazineController.Instance;
        if(mc == null ) return;


        mc.AddCreditBalanceServerRpc(value);
        NetworkXRGrabInteractable interactable = GetComponent<NetworkXRGrabInteractable>();
        NetworkTransformClient ntc = GetComponent<NetworkTransformClient>();
        ntc.AskForOwnership();
        interactable.interactionManager.CancelInteractableSelection(interactable);

        ntc.NetworkObject.Despawn();
    }
}
