using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableConnector : MonoBehaviour
{
    private Cable cable;
    void Start()
    {
        cable = transform.parent.GetComponent<Cable>();
    }

    public Cable GetCable()
    {
        return cable;
    }
}
