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

    private void Start()
    {
        leftHandPushAction.action.started += PushLeft;
        rightHandPushAction.action.started += PushRight;
    }

    private void PushLeft(InputAction.CallbackContext context)
    {
        Debug.Log($"Push left");
    }
    private void PushRight(InputAction.CallbackContext context)
    {
        Debug.Log($"Push right");
    }

    private void Push(bool left)
    {

    }
}
