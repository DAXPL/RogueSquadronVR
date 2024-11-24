using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkTransformClient))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private LayerMask CollisionMask;
    [SerializeField] private SurfaceData defaultSurfaceData;

    private int damage = 10;
    private int force = 10;
    private Rigidbody rb;

    private NetworkObject no;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody>();
        no = GetComponent<NetworkObject>();
    }

    public void SetProjectileParameters(int _damage, int _force)
    {
        damage = _damage;
        force = _force;

        rb.velocity = transform.forward * force;
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
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal).normalized * force;
        }
        else
        {
            OnProjectileHitClientRpc();
        }
    }
   
    [ClientRpc]
    private void OnProjectileHitClientRpc()
    {
        RaycastHit hit;
        GameObject g;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10, CollisionMask) == false) return;
        if (hit.collider.TryGetComponent(out SurfaceInfo surface))
            g = Instantiate(surface.GetSurfaceData().GetHitEffects(), hit.point + (hit.normal * 0.1f), transform.rotation);
        else
            g = Instantiate(defaultSurfaceData.GetHitEffects(), hit.point + (hit.normal * 0.1f), transform.rotation);
       
        if(g == null) return;
        g.transform.SetParent(hit.collider.transform);
        Destroy(g, 30);
        if (IsOwner == false) return;
        if (no != null && no.IsSpawned) no.Despawn();
    }
}
