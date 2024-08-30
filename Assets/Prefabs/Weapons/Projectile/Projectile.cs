using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(NetworkTransformClient))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private int maxReflections = 1;
    [SerializeField] private int force = 10;
    private int reflections = 0;
    private Rigidbody rb;

    private NetworkObject no;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody>();
        no = GetComponent<NetworkObject>();
        rb.velocity = transform.forward * force;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(IsOwner == false) return;
        
        if (!collision.transform.TryGetComponent(out Lightsaber saber) && reflections<=maxReflections)
        {
            Debug.Log($"Collided: {collision.transform.name}");
            reflections++;
            rb.velocity = -rb.velocity;
        }
        else
        {
            if(no!= null) no.Despawn();
        }
    }
}
