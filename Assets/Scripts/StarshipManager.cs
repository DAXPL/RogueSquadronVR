using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarshipManager : NetworkBehaviour
{
    public static StarshipManager Instance;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform spawnPoint;
    private bool[] balloonsState = new bool[6];
    private NetworkVariable<bool> missionLockTravel = new NetworkVariable<bool>();

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
        if (localPlayer != null && startPoint != null)
        {
            localPlayer.transform.position = startPoint.position;
            localPlayer.transform.rotation = startPoint.rotation;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if(localPlayer != null && startPoint != null)
        {
            localPlayer.transform.position = startPoint.position;
            localPlayer.transform.rotation = startPoint.rotation;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("starship"));
    }

    public void SetLocalPlayerAtMedBay()
    {
        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if(!localPlayer) return;
        if (spawnPoint != null)
        {
            localPlayer.transform.position = spawnPoint.position;
            localPlayer.transform.rotation = spawnPoint.rotation;
        }
        else if (startPoint != null) 
        {
            localPlayer.transform.position = startPoint.position;
            localPlayer.transform.rotation = startPoint.rotation;
        }
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

    public void LockTravel(bool newState)
    {
        missionLockTravel.Value = newState;
    }
    public bool IsLockTravel()
    {
        return missionLockTravel.Value;
    }
}