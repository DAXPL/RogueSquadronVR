using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
/*
 Audio: 
    Blaster Shot by UNIVERSFIELD -- https://freesound.org/s/750234/ -- License: Attribution 4.0
    minigun_Overheat.wav by Marregheriti -- https://freesound.org/s/266102/ -- License: Creative Commons 0
*/
public class Blaster : NetworkBehaviour, IWeapon
{
    [SerializeField] private int damage = 10;
    [SerializeField] private int force = 10;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip overheatSound;
    [SerializeField] private Transform barrel;
    [SerializeField] private Projectile projectile;
    [SerializeField] private float delay;
    [SerializeField] private float heatDissipation = 1;
    [SerializeField] private bool fullAuto = false;

    [SerializeField] private ParticleSystem barellSmoke;
    [SerializeField] private ParticleSystem overheatSmoke;

    private float lastShootTimestamp;
    private bool triggerState = false;
    private bool fullAutoLock = false;
    private bool overheated = false;

    private float heatGeneration = 10;
    private float maxHeatLevel = 100;
    private float heatLevel = 0;
    private float overheatPenalty = 10;

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
        if (triggerState && !fullAutoLock) Shoot();
        heatLevel = Mathf.Clamp(heatLevel - (heatDissipation * Time.deltaTime), 0, maxHeatLevel * 2);

        if(barellSmoke!=null) barellSmoke.emissionRate= heatLevel>25?heatLevel:0;
    }

    public void Shoot()
    {
        if (Time.time < lastShootTimestamp + delay + (overheated? overheatPenalty : 0)) return;
        if(heatLevel >= maxHeatLevel) return;
        if (projectile == null) return;

        if (overheated) 
        {
            //instant cool after overheat penalty
            heatLevel -= (int)(maxHeatLevel / 4);
        }

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
        if(heatLevel<0)heatLevel = 0;

        if (fullAuto == false) fullAutoLock = true;

        overheated = (heatLevel > maxHeatLevel);
        //Sync effects on all clients
        ShootEffectsServerRpc(overheated);
    }

    public void ChangeTriggerState(bool newState)
    {
        triggerState = newState;
        if(newState) fullAutoLock = false;
    }

    [ServerRpc]
    private void ShootEffectsServerRpc(bool isOverheated)
    {
        ShootEffectsClientRpc(isOverheated);
    }
    [ClientRpc]
    private void ShootEffectsClientRpc(bool isOverheated)
    {
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);
        if (isOverheated) 
        {
            if(overheatSmoke == null)return;
            ParticleSystem.MainModule module = overheatSmoke.main;
            module.duration = overheatPenalty;
            overheatSmoke.Play();

            if(overheatSound == null) return;
            audioSource.PlayOneShot(overheatSound);
        }
        
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
