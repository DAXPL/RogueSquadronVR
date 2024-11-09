using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransformClient))]
[RequireComponent(typeof(NavMeshAgent))]
public class HarvexAdversary : NetworkBehaviour, IDamageable
{
    private NetworkVariable<int> health = new NetworkVariable<int>(100,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private float detectionRadius = 20;
    [SerializeField] private float fireDelay = 1;
    [SerializeField] private Transform barrel;
    [SerializeField] private int damage = 10;
    [SerializeField] private int force = 10;
    private float fireTimestamp;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Projectile projectile;

    private Transform target;
    private NavMeshAgent agent;

    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null) agent.updateRotation = true;
    }

    private void FixedUpdate()
    {
        if(IsServer == false)return;
        Attack();
        ScanForTargets();
    }
    private void Attack()
    {
        if(target == null) return;
        if (Time.time < fireTimestamp + fireDelay) return;
        if (barrel == null || projectile == null)return;
        Debug.Log($"Fire at {target.gameObject.name}");
        fireTimestamp = Time.time;
        barrel.LookAt(target.position+Vector3.up);
        // Instantiate the projectile at the barrel position (or fallback to weapon's position if barrel is null)
        NetworkObject instance = Instantiate(NetworkManager.
                       GetNetworkPrefabOverride(projectile.gameObject),
                       barrel.transform.position,
                       barrel.transform.rotation).
                       GetComponent<NetworkObject>();

        instance.transform.SetParent(null, true); // Detach the projectile from the droid

        instance.Spawn(); // Spawn the projectile in the network
        instance.GetComponent<Projectile>().SetProjectileParameters(damage, force); // Set damage and force for the projectile
    }
    private void ScanForTargets()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, detectionRadius, this.transform.up);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == this.transform) continue;
            if (hits[i].transform.CompareTag("Player") || hits[i].transform.CompareTag("NetworkPlayer"))
            {
                target = hits[i].transform;
                if (agent != null) agent.SetDestination(target.position);
                return;
            }
        }
        target = null;
    }
    public void Damage(int dmg)
    {
        DamageServerRpc(dmg);
    }
    [ContextMenu("Debug Damage")]
    public void DebugDamage()
    {
        DamageServerRpc(50);
    }

    [ServerRpc]
    public void DamageServerRpc(int dmg)
    {
        Debug.Log($"Taken damage {dmg}");
        health.Value -= dmg;
        if (health.Value < 0) health.Value = 0;
        if (health.Value <= 0)
        {
            DeathClientRpc();
            Debug.Log($"[Server] {gameObject.name} should be dead!");
        }
    }

    [ClientRpc]
    public void DeathClientRpc()
    {
        if(deathEffect != null)
        {
            GameObject go = Instantiate(deathEffect, this.transform.position, Quaternion.identity);
            Destroy(go, 30);
        }
        
        if (IsOwner && this.IsSpawned && TryGetComponent(out NetworkObject no))
        {
            no.Despawn();
        }
    }
}