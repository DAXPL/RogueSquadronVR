using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Fabricator : NetworkBehaviour, IDamageable
{
    [SerializeField] private HarvexAdversary adversary;
    [SerializeField] private float wavesDelay = 5;
    [SerializeField] private int waveCount = 3;
    [SerializeField] private int maxCount = 4;
    private float summonTimestamp;
    [Space]
    [SerializeField] private UnityEvent spawnEffect;
    private List<NetworkObject> adversaryList = new List<NetworkObject>();
    private NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private GameObject deathEffect;

    public delegate void OnFabricatorDestroyedDelegate();
    public OnFabricatorDestroyedDelegate OnFabricatorDestroyed;

    [SerializeField] private bool isFabricatorActive = true;

    private void Update()
    {
        if(!IsServer) return;
        if(!isFabricatorActive) return;
        if (Time.time <= summonTimestamp + wavesDelay) return;
        SpawnAdversaryServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void SpawnAdversaryServerRpc()
    {
        summonTimestamp = Time.time + Random.Range(1,10);

        int currentAdversariesCount = 0;
        for(int i = 0; i < adversaryList.Count; i++)
        {
            if(adversaryList[i] != null) currentAdversariesCount++;
        }
        if( adversaryList.Count > maxCount)
        {
            //Too much adversaries! Skipping wave
            return;
        }

        for (int i = 0; i < waveCount; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(Random.Range(0.3f, 2) * (Random.Range(0, 10) % 2 == 1 ? -1:1),
                                                                0, 
                                                                Random.Range(0.3f, 2) * (Random.Range(0, 10) % 2 == 1 ? -1:1));
            Debug.LogError("Instance havex");
            NetworkObject instance = Instantiate(NetworkManager.
                           GetNetworkPrefabOverride(adversary.gameObject), randomPos, transform.rotation).
            GetComponent<NetworkObject>();

            instance.Spawn();
            instance.transform.SetParent(null, true);
            adversaryList.Add(instance);
        }
        SpawnEffectClientRpc();
    }

    [ClientRpc]
    private void SpawnEffectClientRpc()
    {
        Debug.Log("Spawn effect");
        spawnEffect.Invoke();
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
    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(int dmg)
    {
        Debug.Log($"Taken damage {dmg}");
        health.Value -= dmg;
        if (health.Value < 0) health.Value = 0;
        if (health.Value <= 0)
        {
            Debug.Log($"[Server] {gameObject.name} should be dead!");
            if (this.IsSpawned && TryGetComponent(out NetworkObject no))
            {
                OnFabricatorDestroyed();
                no.Despawn();
            }
        }
    }
   
    public override void OnDestroy()
    {
        if (deathEffect != null)
        {
            GameObject go = Instantiate(deathEffect, this.transform.position, Quaternion.identity);
            Destroy(go, 30);
        }
        base.OnDestroy();
    }

    public void ToggleActive(bool active)
    {
        isFabricatorActive = active;
    }
}
