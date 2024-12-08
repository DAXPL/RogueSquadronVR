using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/*
 Audio: 
    Blaster Shot by UNIVERSFIELD -- https://freesound.org/s/750234/ -- License: Attribution 4.0
    minigun_Overheat.wav by Marregheriti -- https://freesound.org/s/266102/ -- License: Creative Commons 0
*/

// Blaster class that implements the IWeapon interface and extends NetworkBehaviour for network functionality
public class Blaster : NetworkBehaviour, IWeapon
{
    [Header("Parameters")]
    [SerializeField] private int damage = 10; // Amount of damage dealt per shot
    [SerializeField] private int force = 10;  // Force applied to the projectile
    [SerializeField] private float delay;     // Delay between shots
    [SerializeField] private float heatDissipation = 1; // Rate of heat dissipation over time
    [SerializeField] private bool fullAuto = false; // Whether the weapon is fully automatic
    [SerializeField] private AnimationCurve xSpread; // Spread curve for x-axis (affects accuracy)
    [SerializeField] private AnimationCurve ySpread; // Spread curve for y-axis
    [Header("VFX")]
    [SerializeField] private ParticleSystem barellSmoke; // Smoke effect for the barrel
    [SerializeField] private ParticleSystem muzzleFlash; // Muzzle flash effect when shooting
    [SerializeField] private ParticleSystem overheatSmoke; // Smoke effect when overheated
    [Space]
    [SerializeField] private AudioClip shootSound; // Sound played when shooting
    [SerializeField] private AudioClip overheatSound; // Sound played when overheated
    [Header("Tech")]
    [SerializeField] private Transform barrel; // Reference to the barrel's transform
    [SerializeField] private Projectile projectile; // Projectile prefab

    private float lastShootTimestamp;
    private bool triggerState = false; // Whether the trigger is being pulled
    private bool fullAutoLock = false; // Lock for full-auto mode to prevent continuous firing
    private bool overheated = false; // Whether the weapon is overheated
    [Header("Heat")]
    [SerializeField] private float heatGeneration = 10; // Amount of heat generated per shot
    private float maxHeatLevel = 100; // Maximum heat before the weapon overheats
    private float heatLevel = 0; // Current heat level
    private float overheatPenalty = 5; // Time penalty for overheating

    private AudioSource audioSource; // AudioSource component for playing sounds

    // Called when the object is spawned and synchronized over the network
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        audioSource = GetComponent<AudioSource>();
        if (barellSmoke != null) barellSmoke.Play();
    }

    public float Delay()
    {
        return delay;
    }

    void Update()
    {
        // If the trigger is pulled and full auto isn't locked, shoot
        if (triggerState && !fullAutoLock) Shoot();

        // Dissipate heat over time, clamping it to a maximum of twice the max heat level
        heatLevel = Mathf.Clamp(heatLevel - (heatDissipation * Time.deltaTime), 0, maxHeatLevel * 2);

        // Emit smoke from the barrel when heat exceeds a certain level
        if (barellSmoke != null)
            barellSmoke.emissionRate = heatLevel > 25 ? heatLevel : 0;
    }

    // Function to handle shooting logic
    public void Shoot()
    {
        // Check if the weapon can shoot (based on time delay, heat, and overheat status)
        if (Time.time < lastShootTimestamp + delay + (overheated ? overheatPenalty : 0)) return;
        if (heatLevel >= maxHeatLevel) return;
        if (projectile == null) return;

        // Reduce heat, after overheat penalty
        if (overheated)
        {
            heatLevel -= (int)(maxHeatLevel / 3);
        }

        // Update the timestamp of the last shot
        lastShootTimestamp = Time.time;
        ShootProjectileServerRpc();
        
        // Increase heat after shooting
        heatLevel += heatGeneration;
        if (heatLevel < 0) heatLevel = 0;

        // Lock full auto if the weapon is not fully automatic
        if (fullAuto == false) fullAutoLock = true;

        // Set overheated state if heat exceeds max heat level
        overheated = (heatLevel > maxHeatLevel);

        // Synchronize shooting effects across all clients
        ShootEffectsServerRpc(overheated);
    }
    [ServerRpc(RequireOwnership =false)]
    private void ShootProjectileServerRpc() 
    {
        // Instantiate the projectile at the barrel position (or fallback to weapon's position if barrel is null)
        NetworkObject instance = Instantiate(NetworkManager.
                       GetNetworkPrefabOverride(projectile.gameObject),
                       barrel != null ? barrel.transform.position : transform.position,
                       barrel != null ? barrel.transform.rotation : transform.rotation).
                       GetComponent<NetworkObject>();

        instance.transform.SetParent(null, true); // Detach the projectile from the weapon

        // Apply weapon spread based on current heat level
        instance.transform.Rotate(
                                    xSpread.Evaluate(heatLevel / maxHeatLevel) * -1,
                                    ySpread.Evaluate(heatLevel / maxHeatLevel) * -1,
                                    0, Space.Self);

        instance.Spawn(); // Spawn the projectile in the network
        instance.GetComponent<Projectile>().SetProjectileParameters(damage, force); // Set damage and force for the projectile
    }

    // Change the trigger state (true for pulled, false for released)
    public void ChangeTriggerState(bool newState)
    {
        triggerState = newState;
        if (newState) fullAutoLock = false; // Unlock full-auto mode when trigger is pulled
    }

    // Server RPC to synchronize effects across clients
    [ServerRpc(RequireOwnership = false)]
    private void ShootEffectsServerRpc(bool isOverheated)
    {
        ShootEffectsClientRpc(isOverheated); // Call all clients to play effects
    }

    // Client RPC to play shooting effects on all clients
    [ClientRpc]
    private void ShootEffectsClientRpc(bool isOverheated)
    {
        // Play muzzle flash effect if set
        if (muzzleFlash != null) muzzleFlash.Play();

        // Play shooting sound if the audio source and sound are set
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);

        // Handle overheat effects
        if (isOverheated)
        {
            if (overheatSmoke == null) return;
            ParticleSystem.MainModule module = overheatSmoke.main;
            module.duration = overheatPenalty;
            overheatSmoke.Play(); // Play overheat smoke effect

            // Play overheat sound if set
            if (overheatSound == null) return;
                audioSource.PlayOneShot(overheatSound);
        }
    }

    // Debug method to simulate pulling the trigger (available via Unity Editor context menu)
    [ContextMenu("PullTrigger")]
    public void DebugPullTrigger()
    {
        ChangeTriggerState(true);
    }

    // Debug method to simulate releasing the trigger (available via Unity Editor context menu)
    [ContextMenu("ReleaseTrigger")]
    public void DebugReleaseTrigger()
    {
        ChangeTriggerState(false);
    }

}
