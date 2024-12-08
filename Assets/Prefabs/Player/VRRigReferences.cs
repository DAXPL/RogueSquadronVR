using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences Singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public CharacterController characterController;

    private void Awake()
    {
        Singleton = this;
    }
    private void OnDestroy()
    {
        Debug.Log("Destroyed local player - why?");
    }
}
