using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDoors : MonoBehaviour
{
    [SerializeField] private UniversalRemoteProcedureCall urpc;
    [SerializeField] private GameObject key;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == key)
        {
            urpc.OnButtonClicked();
        }
    }
}
