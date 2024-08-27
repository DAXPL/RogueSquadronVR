using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class NavigationSystem : NetworkBehaviour
{
    [SerializeField] private PlanetData[] planets;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private DoorControler exitDoors;

    private NetworkVariable<int> choosenPlanet = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> activePlanet = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> inTravel = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        choosenPlanet.OnValueChanged += onDestinationPlanetChanged;
    }

    /*SELECTING PLANET*/
    public void SetSelectedPlanet(int newPlanet)
    {
        SetSelectedPlanetServerRPC(newPlanet);
    }
    [ServerRpc]
    public void SetSelectedPlanetServerRPC(int newPlanet)
    {
        Debug.Log($"SetSelectedPlanetServerRPC({newPlanet})");
        if(newPlanet >= planets.Length)
        {
            Debug.Log($"[Server] Invalid selection");
            return;
        }
        choosenPlanet.Value = newPlanet;
    }
    private void onDestinationPlanetChanged(int previousValue, int newValue)
    {
        if (newValue == previousValue) return;
        canvasAnimator.SetBool("showPlanetPanel", newValue >= 0);
        //zmienic opisy planety na takie jakie majo byc
        if (newValue >= 0)
        {
            Debug.Log($"Selected:{planets[newValue].planetName}");
        }
    }

    /*SETTING DESTINATION*/
    [ContextMenu("SetDestination")]
    public void SetDestination()
    {
        SetDestinationServerRPC();
    }
    [ServerRpc]
    public void SetDestinationServerRPC()
    {
        if(inTravel.Value == true)
        {
            Debug.Log($"[Serwer] Ship in hyperspace! Cant change destination");
            return;
        }

        if (choosenPlanet.Value == activePlanet.Value)
        {
            Debug.Log($"[Serwer] Same destination. Ignoring");
            return;
        }

        if (choosenPlanet.Value >= planets.Length || choosenPlanet.Value < 0)
        {
            Debug.Log($"[Serwer] Invalid destination {choosenPlanet.Value}");
            return;
        }

        StartCoroutine(TravelCorountine(10));
    }

    private IEnumerator TravelCorountine(int travelTime)
    {
        Debug.Log($"[Serwer] Setting new destination to {planets[choosenPlanet.Value].planetSceneName} system");
        if(exitDoors!=null) exitDoors.SetDoorState(false);
        inTravel.Value = true;
        if (activePlanet.Value != -1) NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName(planets[choosenPlanet.Value].planetSceneName));
        //Load space scene here
        yield return new WaitForSeconds(travelTime);
        Debug.Log($"[Serwer] Arrived to {planets[choosenPlanet.Value].planetSceneName} system!");
        NetworkManager.Singleton.SceneManager.LoadScene(planets[choosenPlanet.Value].planetSceneName, LoadSceneMode.Additive);
        activePlanet.Value = choosenPlanet.Value;
        inTravel.Value = false;
        if (exitDoors != null) exitDoors.SetDoorState(true);
    }

    /*DEBUG*/
    [ContextMenu("SetRandomPlanet")]
    public void SetRandomPlanet()
    {
        int rand = UnityEngine.Random.Range(0, planets.Length);
        Debug.Log($"Choosen random {rand}");
        SetSelectedPlanet(rand);
    }
}
[System.Serializable]
public class PlanetData
{
    public string planetName;
    public string planetDesc;
    public string planetSceneName;
}
