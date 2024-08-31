using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Blaster : NetworkBehaviour, IWeapon
{
    [SerializeField] private Transform barrel;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float delay;

    private float lastShootTimestamp;

    public float Delay()
    {
        return delay;
    }

    public void Shoot()
    {
        if (Time.time < lastShootTimestamp + delay) return; 
        if(projectile == null) return;
        lastShootTimestamp = Time.time;
        NetworkObject instance = Instantiate(NetworkManager.
                       GetNetworkPrefabOverride(projectile.gameObject),
                       barrel != null ? barrel.transform.position:transform.position,
                       barrel != null ? barrel.transform.rotation : transform.rotation).
                       GetComponent<NetworkObject>();

        instance.transform.SetParent(null, true);

        instance.Spawn();
    }

}
