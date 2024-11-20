using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarshipManager : NetworkBehaviour
{
    public static StarshipManager Instance;
    [SerializeField] private Transform spawnPoint;
    private bool[] balloonsState = new bool[6];

    private void Awake()
    {
        if (Instance != null) 
        { 
            Destroy(this); 
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if (localPlayer != null && spawnPoint != null)
        {
            localPlayer.transform.position = spawnPoint.position;
            localPlayer.transform.rotation = spawnPoint.rotation;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if(localPlayer != null && spawnPoint!= null)
        {
            localPlayer.transform.position = spawnPoint.position;
            localPlayer.transform.rotation = spawnPoint.rotation;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("starship"));
    }

    public void PopBalloon(int id)
    {
        if(!IsServer) return;
        if(id >= balloonsState.Length) return;
        balloonsState[id] = true;
    }
    public bool IsBalloonActive(int id)
    {
        if (id >= balloonsState.Length) return false;
        return balloonsState[id];
    }
}