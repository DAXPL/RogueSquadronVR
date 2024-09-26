using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class ForceManager : MonoBehaviour
{
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [Space]
    [SerializeField] private InputActionReference leftHandPushAction;
    [SerializeField] private InputActionReference rightHandPushAction;
    [Space]
    [SerializeField] private InputActionReference leftHandGrabAction;
    [SerializeField] private InputActionReference rightHandGrabAction;
    [Space]
    [SerializeField] private LayerMask interactableLayer;
    [Space]
    [SerializeField] private float forceDistance;
    private Rigidbody forceRigidbody;
    private bool leftHolded = true;
    private float forceCurDistance;


    private float forceDelay = 1;
    private float forceUseTimestamp = 0;
    private float forceRadius = 0.75f;
    private float forceStrength = 5;
    private void Start()
    {
        leftHandPushAction.action.started += PushLeft;
        rightHandPushAction.action.started += PushRight;

        leftHandGrabAction.action.started += GrabLeft;
        leftHandGrabAction.action.canceled += GrabLeft;

        rightHandGrabAction.action.started += GrabRight;
        rightHandGrabAction.action.canceled += GrabRight;
    }

    private void FixedUpdate()
    {
        if(forceRigidbody == null) return;

        Vector3 targetPosition = (leftHolded ? leftHandTransform.position : rightHandTransform.position)
                            + (leftHolded ? leftHandTransform.forward : rightHandTransform.forward) * forceCurDistance;

        Vector3 dist = targetPosition - forceRigidbody.transform.position;
        forceRigidbody.velocity = dist / Time.fixedDeltaTime;
    }
    /*PUSHING*/
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
        if(left && (leftHolded && forceRigidbody!=null)) return;
        if (!left && (!leftHolded && forceRigidbody != null)) return;
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
    /*GRABBING*/
    private void GrabLeft(InputAction.CallbackContext context)
    {
        if(forceRigidbody == null || (forceRigidbody != null && leftHolded==true))
            Grab(true);
    }
    private void GrabRight(InputAction.CallbackContext context)
    {
        if (forceRigidbody == null || (forceRigidbody != null && leftHolded == false))
            Grab(false);
    }

    private void Grab(bool left)
    {
        //Throw grabbed rigidbody
        if (forceRigidbody != null)
        {
            forceRigidbody.velocity *= (forceStrength/2);
            forceRigidbody = null;
            forceCurDistance = forceDistance / 2;
            //Play throw sound
            return;
        }

        //Grab new one
        leftHolded = left;
        RaycastHit hit;
        
        if (Physics.Raycast(leftHolded ? leftHandTransform.position : rightHandTransform.position, leftHolded ? leftHandTransform.forward : rightHandTransform.forward, out hit, forceDistance, interactableLayer))
        { 
            if (hit.collider.gameObject.TryGetComponent(out Rigidbody newForceRigidbody))
            {
                if (hit.collider.TryGetComponent(out NetworkTransformClient ntc)) ntc.AskForOwnership();
                forceRigidbody = newForceRigidbody;
                forceCurDistance =  Vector3.Distance(leftHolded ? leftHandTransform.position : rightHandTransform.position, forceRigidbody.transform.position);

                Debug.DrawRay(leftHolded ? leftHandTransform.position : rightHandTransform.position, leftHolded ? leftHandTransform.forward : rightHandTransform.forward, Color.green, 10);
                //Play grab sound
            }
            else
            {
                Debug.DrawRay(leftHolded ? leftHandTransform.position : rightHandTransform.position, leftHolded ? leftHandTransform.forward : rightHandTransform.forward, Color.yellow, 10);
                //Play some sound
            }
        }
        else
        {
            Debug.DrawRay(leftHolded ? leftHandTransform.position : rightHandTransform.position, leftHolded ? leftHandTransform.forward : rightHandTransform.forward, Color.red, 10);
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
