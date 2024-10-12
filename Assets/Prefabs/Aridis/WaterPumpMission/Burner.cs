using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Burner : NetworkBehaviour, IWeapon
{
    [SerializeField] private Transform barell;
    [SerializeField] private GameObject sparks;
    private bool triggerState = false; // Whether the trigger is being pulled
    private float lastShootTimestamp;
    private int fixSpeed = 10;
    public float Delay()
    {
        return 0.5f;
    }
    private void Update()
    {
        if (triggerState && Time.time >= lastShootTimestamp+Delay())
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        lastShootTimestamp = Time.time;
        float distance = 2;
        RaycastHit hit;
        if (!Physics.Raycast(barell.position, barell.forward, out hit, distance)) 
        {
            Debug.DrawRay(barell.position, barell.forward* distance, Color.white,1);
            sparks.SetActive(false);
            return; 
        }
        
        if(!hit.collider.TryGetComponent(out WaterPumpController wpc))
        {
            Debug.Log($"{hit.collider.name} - {wpc == null}");
            Debug.DrawRay(barell.position, barell.forward * distance, Color.yellow, 1);
            sparks.SetActive(false);
            return;
        }
        sparks.transform.position = hit.point;
        sparks.SetActive(true);
        Debug.DrawRay(barell.position, barell.forward * distance, Color.green, 1);
        wpc.Fixing(fixSpeed);
    }

    // Change the trigger state (true for pulled, false for released)
    public void ChangeTriggerState(bool newState)
    {
        triggerState = newState;
        if( triggerState == false ) sparks.SetActive(false);
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

