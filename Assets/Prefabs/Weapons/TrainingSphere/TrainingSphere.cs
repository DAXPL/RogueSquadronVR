using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrainingSphere : NetworkBehaviour
{
    [SerializeField] private IWeapon weapon;
    private NetworkVariable<bool> state = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        weapon = GetComponent<IWeapon>();
        state.OnValueChanged += OnSphereStateChanged;
        if (IsServer) StartCoroutine(TrainingSphereLogic());
    }

    private void OnSphereStateChanged(bool previousValue, bool newValue)
    {

    }

    [ContextMenu("ActivateSphere")]
    public void ActivateSphere()
    {
        ToggleSphereServerRPC(true);
    }
    [ContextMenu("DeactivateSphere")]
    public void DeactivateSphere()
    {
        ToggleSphereServerRPC(true);
    }
    [ContextMenu("ToggleSphere")]
    public void ToggleSphere()
    {
        ToggleSphereServerRPC();
    }

    [ServerRpc]
    private void ToggleSphereServerRPC()
    {
        state.Value = !state.Value;
    }
    [ServerRpc]
    private void ToggleSphereServerRPC(bool newState)
    {
        state.Value = newState;
    }

    private IEnumerator TrainingSphereLogic()
    {
        while (true)
        {
            if(weapon == null || state.Value == false)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                weapon.Shoot();
                yield return new WaitForSeconds(weapon.Delay()+0.1f);
            }
        }
    }
}
