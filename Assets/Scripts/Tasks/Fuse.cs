using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : Serviceable
{
    [SerializeField] private CableSocket[] sockets;

    private void Start()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].OnConnectionUpdate += UpdateSockets;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].OnConnectionUpdate -= UpdateSockets;
        }
    }

    private void UpdateSockets()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            if(sockets[i].isConnected() == false) return;
        }
        Fix();
    }
}
