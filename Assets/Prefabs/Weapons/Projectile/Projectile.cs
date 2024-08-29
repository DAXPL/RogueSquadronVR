using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(NetworkTransformClient))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private const int maxReflections = 1;
    private int reflections = 0;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(IsOwner == false) return;
        Debug.Log($"Collided: {collision.transform.name}");
        if (!collision.transform.TryGetComponent(out Lightsaber saber) && reflections<=maxReflections)
        {
            reflections++;
            rb.velocity = -rb.velocity;
        }
        else
        {
            Destroy(this);
        }
    }
}
