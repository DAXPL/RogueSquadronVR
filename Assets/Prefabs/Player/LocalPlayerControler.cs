using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocalPlayerControler : MonoBehaviour
{
    private LocomotionSystem locomotion;
    [SerializeField] private XRDirectInteractor[] interactors;
    public delegate void OnTeleportDelegate();
    public OnTeleportDelegate OnTeleport;
    private void Start()
    {
        locomotion = GetComponent<LocomotionSystem>();    
    }

    public void SetMovement(bool newState)
    {
        if(locomotion == null) return;

        locomotion.enabled = newState;
        for (int i = 0; i < interactors.Length; i++) 
            interactors[i].enabled = newState;
    }
    public void OnTeleportPerformed()
    {
        OnTeleport.Invoke();
    }
}
