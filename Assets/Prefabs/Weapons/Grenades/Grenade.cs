using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grenade : NetworkBehaviour, IDamageable
{
    [SerializeField] private GameObject effect;
    [SerializeField] private int baseDamage = 50;
    [SerializeField] private float explosionTime = 3.0f;
    [SerializeField] private float explosionRadius = 10.0f;
    private bool armed = false;
    private bool fuseOff = false;
    private bool blown = false;
    private float explodeTimestamp;

    [ContextMenu("Activate")]
    public void OnActivate()
    {
        if(blown) return;
        armed = !armed;
    }

    [ContextMenu("Throw")]
    public void OnThrow()
    {
        if (blown) return;
        if (!armed) return;
        fuseOff = true;
        explodeTimestamp = Time.time + explosionTime;
    }

    private void FixedUpdate()
    {
        if (blown) return;
        if (!fuseOff) return;
        if(Time.time <= explodeTimestamp) return;
        Explode();
    }
    private void Explode()
    {
        blown = true;
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, explosionRadius, this.transform.up);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == this.transform) continue;
            if (hits[i].transform.TryGetComponent(out IDamageable damageable))
            {
                float distance = Mathf.Clamp(Vector3.Distance(this.transform.position, hits[i].transform.position), 1, explosionRadius); ;
                float multipiler = explosionRadius / distance;
                damageable.Damage((int)(baseDamage * multipiler));
            }

            if (hits[i].collider.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(50, this.transform.position, explosionRadius * 1.2f);
            }

        }
        ExplodeServerRpc();
    }

    [ServerRpc]
    public void ExplodeServerRpc()
    {
        OnExplodeClientRpc();
    }

    [ClientRpc]
    public void OnExplodeClientRpc()
    {
        if(effect == null) return;
        GameObject go = Instantiate(effect,transform.position, Quaternion.identity);
        Destroy(go,30);
        
        if (IsOwner && this.IsSpawned && TryGetComponent(out NetworkObject no))
        {
            no.Despawn();
        }
    }

    public void Damage(int dmg)
    {
        if (IsOwner == false) return;
        fuseOff = true;
        explodeTimestamp = Time.time;
    }
}
