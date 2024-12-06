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
    private NetworkVariable<int> health = new NetworkVariable<int>(50,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private float detectionRadius = 20;
    [SerializeField] private float fireDelay = 1;
    [SerializeField] private Transform barrel;
    [SerializeField] private int damage = 10;
    [SerializeField] private int force = 10;
    private float fireTimestamp;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Projectile projectile;
    [SerializeField] private LayerMask raycastMask;
    private Transform target;
    private NavMeshAgent agent;
    private float baseSpeed;
    private Vector3 startPos;

    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null) agent.updateRotation = true;
        baseSpeed = agent.speed;
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if(IsServer == false)return;
        Attack();
        if (ScanForTargets())
        {
            agent.speed = baseSpeed;
        }
        else if (agent.remainingDistance <= 0.1)
        {
            agent.speed = baseSpeed / 2;
            agent.SetDestination(RandomNavmeshLocation(10));
        }
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
    private bool ScanForTargets()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, detectionRadius, this.transform.up, detectionRadius, raycastMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == this.transform) continue;
            if (hits[i].transform.CompareTag("Player") || hits[i].transform.CompareTag("NetworkPlayer"))
            {
                target = hits[i].transform;
                if (agent != null) agent.SetDestination(target.position);
                return true;
            }
        }
        target = null;
        return false;
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
        health.Value -= dmg;
        if (health.Value < 0) health.Value = 0;
        if (health.Value <= 0)
        {
            DeathClientRpc();
        }
    }

    [ClientRpc]
    public void DeathClientRpc()
    {
        if(deathEffect != null)
        {
            GameObject go = Instantiate(deathEffect, this.transform.position, Quaternion.identity,null);
            Destroy(go, 20);
        }
        
        if (IsOwner && this.IsSpawned && TryGetComponent(out NetworkObject no))
        {
            no.Despawn();
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = (Random.insideUnitSphere + new Vector3(0.5f, 0.0f, 0.5f)) * (radius - 0.5f);
        randomDirection += startPos;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}