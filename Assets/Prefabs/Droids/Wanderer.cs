using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wanderer : NetworkBehaviour
{
    [SerializeField] private float searchRadius;
    private NavMeshAgent agent;
    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) return;

        if (!IsServer) 
        {
            agent.enabled = false;
            return;
        } 

        StartCoroutine(WandererThread());
    }

    IEnumerator WandererThread()
    {
        while (true)
        {
            if (agent.remainingDistance <= 0.1)
            {
                yield return new WaitForSeconds(Random.Range(0,3));
                agent.SetDestination(RandomNavmeshLocation(searchRadius));
            }
            yield return null;
        }
    }
    public Vector3 RandomNavmeshLocation(float radius)
    {   
        Vector3 randomDirection = (Random.insideUnitSphere +new Vector3(0.5f,0.0f,0.5f)) * (radius-0.5f);
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}
