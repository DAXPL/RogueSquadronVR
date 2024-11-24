using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocalPlayerControler : MonoBehaviour
{
    private LocomotionSystem locomotion;
    [SerializeField] private XRDirectInteractor[] interactors;
    public delegate void OnTeleportDelegate();
    public OnTeleportDelegate OnTeleport;
    private Coroutine currentDieSequence;
    private int respawnTime = 5;
    private void Start()
    {
        locomotion = GetComponent<LocomotionSystem>();    
    }

    public void SetMovement(bool newState)
    {
        if(locomotion == null) return;

        locomotion.enabled = newState;
        for (int i = 0; i < interactors.Length; i++) 
            interactors[i].enabled = newState;
    }
    public void OnTeleportPerformed()
    {
        OnTeleport.Invoke();
    }

    public void OnDie()
    {
        if(currentDieSequence != null)StopCoroutine(currentDieSequence);
        currentDieSequence = StartCoroutine(DieSequence());
    }
    private IEnumerator DieSequence()
    {
        locomotion.enabled = false;
        if (StarshipManager.Instance) StarshipManager.Instance.SetLocalPlayerAtMedBay();
        for (int i = 0;i < respawnTime; i++)
        {
            Debug.Log($"{respawnTime-i} seconds to spawn");
            yield return new WaitForSeconds(1);
        }
        locomotion.enabled = true;
        Debug.Log("Heroes never die!");
        if (Time.time % 10 == 0) Debug.Log("For a price :)");
    }
}
