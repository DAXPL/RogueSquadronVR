using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.OpenXR.Input;

public class CableSocket : MonoBehaviour
{
    private Cable connectedCable;
    [SerializeField] private int socketType;
    [SerializeField] private UnityEvent OnConnectionChanged;
    public delegate void OnConnectionUpdateDelegate();
    public OnConnectionUpdateDelegate OnConnectionUpdate;
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out CableConnector c))
        {
            connectedCable = c.GetCable();
            connectedCable.ConnectSocket(this);
            OnConnectionUpdate.Invoke();
            OnConnectionChanged.Invoke();
        }
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out CableConnector c))
        {
            if(c.GetCable() == connectedCable)
            {
                connectedCable.DisconnectSocket(this);
                connectedCable = null;
                OnConnectionUpdate.Invoke();
                OnConnectionChanged.Invoke();
            }  
        }
    }

    public int GetSocketType()
    {
        return socketType;
    }

    public bool isConnected()
    {
        if (connectedCable == null) return false;
        CableSocket otherEnd = connectedCable.GetOtherEnd(this);
        if (otherEnd == null) return false;
        return otherEnd.GetSocketType() == socketType;
    }
}