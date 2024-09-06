using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

//Audio: Blaster Shot by UNIVERSFIELD -- https://freesound.org/s/750234/ -- License: Attribution 4.0
public class Blaster : NetworkBehaviour, IWeapon
{
    [SerializeField] private int damage = 10;
    [SerializeField] private int force = 10;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private Transform barrel;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float delay;
    [SerializeField] private float heatDissipation = 1;
    [SerializeField] private bool fullAuto = false;

    private float lastShootTimestamp;
    private bool triggerState = false;
    private bool fullAutoLock = false;

    private float heatGeneration = 10;
    private float maxHeatLevel = 100;
    private float heatLevel = 0;

    private AudioSource audioSource;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        audioSource = GetComponent<AudioSource>();
    }

    public float Delay()
    {
        return delay;
    }

    void Update()
    {
        heatLevel = Mathf.Clamp(heatLevel - (heatDissipation * Time.deltaTime), 0, maxHeatLevel * 2); 
        if(triggerState && !fullAutoLock) Shoot();
    }

    public void Shoot()
    {
        if (Time.time < lastShootTimestamp + delay) return;
        if(heatLevel >= maxHeatLevel) return;
        if (projectile == null) return;

        lastShootTimestamp = Time.time;
        NetworkObject instance = Instantiate(NetworkManager.
                       GetNetworkPrefabOverride(projectile.gameObject),
                       barrel != null ? barrel.transform.position : transform.position,
                       barrel != null ? barrel.transform.rotation : transform.rotation).
                       GetComponent<NetworkObject>();
        
        instance.transform.SetParent(null, true);
        instance.Spawn();
        instance.GetComponent<Projectile>().SetProjectileParameters(damage,force);
        heatLevel += heatGeneration;
        if (fullAuto == false) fullAutoLock = true;
        //Sync effects on all clients
        ShootEffectsServerRpc();
    }

    public void ChangeTriggerState(bool newState)
    {
        triggerState = newState;
        if(newState) fullAutoLock = false;
    }

    [ServerRpc]
    private void ShootEffectsServerRpc()
    {
        ShootEffectsClientRpc();
    }
    [ClientRpc]
    private void ShootEffectsClientRpc()
    {
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);
    }

    [ContextMenu("PullTrigger")]
    public void DebugPullTrigger()
    {
        ChangeTriggerState(true);
    }
    [ContextMenu("ReleaseTrigger")]
    public void DebugReleaseTrigger()
    {
        ChangeTriggerState(false);
    }

}
