using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerPouch : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor interactor;
    public void OnUsed(SelectEnterEventArgs args)
    {
        if(args.interactableObject.transform.gameObject.TryGetComponent(out Credit c))
        {
            interactor.EndManualInteraction();
            c.AddCredit();
        }
    }
}
