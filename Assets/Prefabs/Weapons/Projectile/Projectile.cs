using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkTransformClient))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private int maxReflections = 1;
    [SerializeField] private GameObject holePrefab;
    private int damage = 10;
    private int force = 10;
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

    public void SetProjectileParameters(int _damage, int _force)
    {
        damage = _damage;
        force = _force;
    }

    public void OnCollisionEnter(Collision collision)
    {
        //Only owner can initiate damage sequence
        if(IsOwner == false) return;
        
        if(collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(damage);
        }

        if (collision.transform.CompareTag("Reflection"))
        {
            reflections++;
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal).normalized * force;
        }
        else
        {
            OnProjectileHitClientRpc();
            if (no!= null && no.IsSpawned) no.Despawn();
        }
    }
   
    [ClientRpc]
    private void OnProjectileHitClientRpc()
    {
        //play hit sound based on material
        if (holePrefab != null) Instantiate(holePrefab,transform.position-(this.transform.forward*0.05f),transform.rotation,null);
    }
}
