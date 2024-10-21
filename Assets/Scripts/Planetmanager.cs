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
    [SerializeField] private Serviceable[] missions;
    void Start()
    {
        Debug.Log($"Welcome to {planetName}");
        isRaining.OnValueChanged += OnRainStateChanged;
        if (IsOwner || IsServer) 
        {
            OnPlayersArriveServerRpc();
            if (PlayerPrefs.GetInt($"WasOn{planetName}", 0) <= 0)
            {
                PlayerPrefs.SetInt($"WasOn{planetName}", 1);
                for(int i = 0; i < missions.Length; i++)
                {
                    missions[i].Damage();
                }
            }
        } 
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