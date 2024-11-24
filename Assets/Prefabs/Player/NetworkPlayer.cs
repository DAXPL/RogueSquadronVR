using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IDamageable
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private ParticleSystem teleportEffect;
    [SerializeField] private AudioSource teleportSound;

    public Renderer[] meshes;
    private CapsuleCollider hitbox;

    [SerializeField] private Renderer[] meshesToDye;
    
    private NetworkVariable<int> health = new NetworkVariable<int>(100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private NetworkVariable<int> playerID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private LocalPlayerControler localPlayer;

    private static int playerCounter = 0;
    private static readonly object idLock = new object();
    private float regenTimestamp;
    private float regenTime;
    private int regenAmout = 5;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (Renderer mesh in meshes)
            {
                mesh.enabled = false;
            }
            //GetComponent<CapsuleCollider>().enabled = false;
            meshesToDye[0].enabled = false;
        }
        localPlayer = FindObjectOfType<LocalPlayerControler>();
        localPlayer.OnTeleport += OnTeleportEffectServerRpc;
        hitbox = GetComponent<CapsuleCollider>();
        health.OnValueChanged += OnDamage;
        GetPlayerDataServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void OnTeleportEffectServerRpc()
    {
        OnTeleportEffectClientRpc();
    }
   
    [ClientRpc]
    private void OnTeleportEffectClientRpc()
    {
        if(teleportEffect) teleportEffect.Play();
        if(teleportSound) teleportSound.Play();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        health.OnValueChanged -= OnDamage;
        Die();
    }


    [ServerRpc]
    private void GetPlayerDataServerRpc()
    {
        lock (idLock) 
        {
            playerID.Value = playerCounter;
            playerCounter++;
            SetPlayerDataClientRpc();
        }
    }
    [ClientRpc]
    private void SetPlayerDataClientRpc()
    {
        Color mainColor = GeneratePlayerColor(playerID.Value);
        foreach (Renderer mesh in meshesToDye)
        {
            mesh.material.SetColor("_EmissionColor", mainColor);
        }
    }
    private Color GeneratePlayerColor(int playerID)
    {
        float hue = (playerID * 0.618033988749895f) % 1f; // Sta³a to z³oty podzia³
        return Color.HSVToRGB(hue, 0.8f, 0.8f);
    }
    private void OnDamage(int previousValue, int newValue)
    {
        if (IsOwner) 
        {
            if (health.Value <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("I should be dead");
        if (!IsLocalPlayer) return;
        if(localPlayer == null) return;
        if (deathEffect)
        {
            Debug.Log("Summoning helmet");
            NetworkObject instance = Instantiate(NetworkManager.GetNetworkPrefabOverride(deathEffect.gameObject),
                                 head.position + (Vector3.up*0.1f), head.rotation, null).
                                 GetComponent<NetworkObject>();
            Debug.Log("Summoned");
        }
        localPlayer.OnDie();
        
    }

    private void Update()
    {
        if(IsOwner)
        {
            root.position = VRRigReferences.Singleton.root.position;
            root.rotation = VRRigReferences.Singleton.root.rotation;

            head.position = VRRigReferences.Singleton.head.position;
            head.rotation = VRRigReferences.Singleton.head.rotation;

            leftHand.position = VRRigReferences.Singleton.leftHand.position;
            leftHand.rotation = VRRigReferences.Singleton.leftHand.rotation;

            rightHand.position = VRRigReferences.Singleton.rightHand.position;
            rightHand.rotation = VRRigReferences.Singleton.rightHand.rotation;
        }
        if(hitbox == null) return;
        hitbox.height = head.localPosition.y;
        hitbox.center = Vector3.up * (hitbox.height / 2);

        if (IsServer) 
        { 
            if(health.Value >=100) return;
            if(Time.time <= regenTimestamp) return;
            regenTime += Time.deltaTime;
            if (regenTime > 1) 
            {
                health.Value = Mathf.Clamp(health.Value+ (int)(Mathf.FloorToInt(regenTime) * regenAmout),0,100);
                regenTime -= Mathf.FloorToInt(regenTime);
            } 
        }
    }

    public Transform GetHead() { return head; }
    public Transform GetRoot() { return root; }

    [ContextMenu("DebugDamage")]
    public void DebugDamage()
    {
        Damage(50);
    }

    public void Damage(int dmg)
    {
        Debug.Log($"{transform.name} took {dmg} damage!");
        DamageServerRpc(dmg);
    }

    [ServerRpc]
    public void DamageServerRpc(int dmg)
    {
        health.Value -=  dmg;
        regenTime = 0;
        regenTimestamp = Time.time+10;
        if (health.Value < 0) health.Value = 0;
        if (health.Value <= 0)
        {
            Debug.Log("[Server] player should be dead!");
        }
    }
}
