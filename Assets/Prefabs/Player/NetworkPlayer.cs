using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IDamageable
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    public Renderer[] meshes;

    private NetworkVariable<int> health = new NetworkVariable<int>(100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private LocalPlayerControler localPlayer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (Renderer mesh in meshes)
            {
                mesh.enabled = false;
            }
        }
        localPlayer = FindObjectOfType<LocalPlayerControler>();
        health.OnValueChanged += OnDamage;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        health.OnValueChanged -= OnDamage;
        Die();
    }

    private void OnDamage(int previousValue, int newValue)
    {
        //play effect
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
        if(localPlayer == null) return;
        localPlayer.SetMovement(false);
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
        
    }

    public Transform GetHead() { return head; }
    public Transform GetRoot() { return root; }

    public void Damage(int dmg)
    {
        Debug.Log($"{transform.name} took {dmg} damage!");
        DamageServerRpc(dmg);
    }

    [ServerRpc]
    public void DamageServerRpc(int dmg)
    {
        health.Value -= health.Value - dmg;
        if (health.Value < 0) health.Value = 0;
        if (health.Value <= 0)
        {
            Debug.Log("[Server] player should be dead!");
        }
    }

}
