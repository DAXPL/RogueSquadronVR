using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Fabricator : NetworkBehaviour
{
    [SerializeField] private HarvexAdversary adversary;
    [SerializeField] private float wavesDelay = 5;
    [SerializeField] private int waveCount = 3;
    private float summonTimestamp;
    [Space]
    [SerializeField] private UnityEvent spawnEffect;
    private List<NetworkObject> adversaryList = new List<NetworkObject>();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    private void FixedUpdate()
    {
        if(!IsServer) return;
        if (Time.time <= summonTimestamp + wavesDelay) return;
        SpawnAdversary();
    }
    private void SpawnAdversary()
    {
        if (!IsServer) return;
        summonTimestamp = Time.time;

        int currentAdversariesCount = 0;
        for(int i = 0; i < adversaryList.Count; i++)
        {
            if(adversaryList[i] != null) currentAdversariesCount++;
        }
        if( adversaryList.Count > 5)
        {
            Debug.LogWarning("Too much adversaries! Skipping wave");
            return;
        }

        for (int i = 0; i < waveCount; i++)
        {
            NetworkObject instance = Instantiate(NetworkManager.
                           GetNetworkPrefabOverride(adversary.gameObject), transform.position,transform.rotation).
            GetComponent<NetworkObject>();

            instance.transform.SetParent(null, true);

            instance.Spawn();

            adversaryList.Add(instance);
        }
        SpawnEffectClientRpc();
    }

    [ClientRpc]
    private void SpawnEffectClientRpc()
    {
        spawnEffect.Invoke();
    }
}
