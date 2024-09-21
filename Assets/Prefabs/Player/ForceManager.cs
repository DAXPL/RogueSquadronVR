using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForceManager : MonoBehaviour
{
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [Space]
    [SerializeField] private InputActionReference leftHandPushAction;
    [SerializeField] private InputActionReference rightHandPushAction;
    [Space]
    [SerializeField] private LayerMask interactableLayer;
    
    private float forceDelay = 1;
    private float forceUseTimestamp = 0;
    private float forceRadius = 0.75f;
    private float forceStrength = 2;
    private void Start()
    {
        leftHandPushAction.action.started += PushLeft;
        rightHandPushAction.action.started += PushRight;
    }

    private void PushLeft(InputAction.CallbackContext context)
    {
        Push(true);
    }
    private void PushRight(InputAction.CallbackContext context)
    {
        Push(false);
    }

    private void Push(bool left)
    {
        if(Time.time < forceUseTimestamp + forceDelay) return;
        forceUseTimestamp = Time.time;
        Vector3 startPoint = (left ? leftHandTransform.position : rightHandTransform.position) + ((left ? leftHandTransform.forward : rightHandTransform.forward)* forceRadius * 0.9f);
        RaycastHit[] hits = Physics.SphereCastAll(startPoint, forceRadius, transform.forward,forceRadius, interactableLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out Rigidbody rb) == false) continue;
            if (hit.collider.TryGetComponent(out NetworkTransformClient ntc))
                ntc.AskForOwnership();
            Vector3 direction = left?leftHandTransform.forward : rightHandTransform.forward;
            Vector3 forceDirection = direction * forceStrength * UnityEngine.Random.Range(0.75f, 1.25f);

            rb.AddForce(forceDirection, ForceMode.Impulse);
            float randTorqX = UnityEngine.Random.Range(-15,15);
            float randTorqY = UnityEngine.Random.Range(-15, 15);
            float randTorqZ = UnityEngine.Random.Range(-15, 15);
            rb.AddTorque(new Vector3(randTorqX, randTorqY, randTorqZ),ForceMode.Impulse);

            Debug.DrawRay(rb.transform.position, forceDirection, Color.green,5);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftHandTransform.position + (leftHandTransform.forward * forceRadius * 0.9f), forceRadius);
        Gizmos.DrawWireSphere(rightHandTransform.position + (rightHandTransform.forward * forceRadius * 0.9f), forceRadius);
    }

    [ContextMenu("Push left")]
    public void DebugPushLeft()
    {
        Push(true);
    }
}
