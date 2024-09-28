using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planetmanager : NetworkBehaviour
{
    [SerializeField] private string planetName;
    [SerializeField] private GameObject rainEffect;
    private NetworkVariable<bool> isRaining = new NetworkVariable<bool>();
    void Start()
    {
        Debug.Log($"Welcome to {planetName}");
        isRaining.OnValueChanged += OnRainStateChanged;
        if (IsOwner || IsServer) OnPlayersArriveServerRpc();
    }

    private void OnRainStateChanged(bool previousValue, bool newValue)
    {
        if(rainEffect)rainEffect.SetActive(newValue);
    }

    [ServerRpc]
    private void OnPlayersArriveServerRpc()
    {
        isRaining.Value = Random.Range(0.0f, 1.0f) > 0.5f ? true:false ;
    }

}