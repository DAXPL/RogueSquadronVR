using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Cable : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] private Transform[] plugs;
    [SerializeField] private List<CableSocket> sockets = new List<CableSocket>();
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, plugs[0].transform.position);
        lineRenderer.SetPosition(1, plugs[1].transform.position);
    }

    public void ConnectSocket(CableSocket socket)
    {
        sockets.Add(socket);
    }
    public void DisconnectSocket(CableSocket socket)
    {
        sockets.Remove(socket);
    }
    
    public CableSocket GetOtherEnd(CableSocket s)
    {
        for (int i = 0; i < sockets.Count; i++)
        {
            if (sockets[i] != s) return sockets[i];
        }
        return null;
    }
}
