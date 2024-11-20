using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Baloon : NetworkBehaviour, IDamageable
{
    [SerializeField] private ParticleSystem confetti;


    public void Damage(int dmg)
    {
        if(!IsOwner) return;
        OnDestroyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnDestroyServerRpc()
    {
        OnDestroyClientRpc();
    }

    [ClientRpc]
    private void OnDestroyClientRpc()
    {
        if (confetti != null) 
        {
            ParticleSystem go = Instantiate(confetti, transform.position, transform.rotation, null);
            Destroy(go, 10);
        }
        if (!IsOwner) return;
        if (!TryGetComponent(out NetworkObject no)) return;
        no.Despawn();
    }
    [ContextMenu("POP")]
    public void PopBaloon()
    {
        Damage(0);
    }
}
