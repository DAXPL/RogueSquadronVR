using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationSystem : NetworkBehaviour
{
    [SerializeField] private PlanetData[] planets;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private DoorControler exitDoors;

    [SerializeField] private Serviceable navigationSystem;
    [SerializeField] private Reactor[] engines;

    [SerializeField] private MortarSystem mortarSystem;
    private NetworkVariable<int> choosenPlanet = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> activePlanet = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> travelStatus = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> inTravel = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Planet UI")]
    [SerializeField] private GameObject travelPanel;
    [SerializeField] private TextMeshProUGUI planetNameUGUI;
    [SerializeField] private TextMeshProUGUI planetDescUGUI;
    [SerializeField] private Image planetImageUGUI;
    [SerializeField] private Button startButton;
    [Header("Error UI")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorDesc;
    [Header("Travel UI")]
    [SerializeField] private Slider travelStatusSlider;
    [SerializeField] private Image startImage;
    [SerializeField] private Image destinationImage;
    [Header("Effects")]
    [SerializeField] private GameObject starsEmmiter;
    [SerializeField] private GameObject hyperspaceEmmiter;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        choosenPlanet.OnValueChanged += onDestinationPlanetChanged;
        NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;

        inTravel.OnValueChanged += OnTravelStateChanged;
        travelStatus.OnValueChanged += OnTravelTimeStatusChanged;

        travelStatusSlider.value = 0;

        if (starsEmmiter == null) return;
        starsEmmiter.SetActive(!inTravel.Value);
        if (hyperspaceEmmiter == null) return;
        hyperspaceEmmiter.SetActive(inTravel.Value);
    }

    private void OnTravelTimeStatusChanged(float previousValue, float newValue)
    {
        if(travelStatusSlider == null) return;
        travelStatusSlider.value = newValue;
    }

    private void OnTravelStateChanged(bool previousValue, bool newValue)
    {
        if(travelPanel != null)travelPanel.SetActive(newValue);
        if(starsEmmiter != null) starsEmmiter.SetActive(!newValue && SceneManager.GetActiveScene().name == "starship");
        if(hyperspaceEmmiter != null) hyperspaceEmmiter.SetActive(newValue);
        if (mortarSystem != null) mortarSystem.LockMortar(!newValue);

        if (startImage)
        {
            startImage.sprite = (activePlanet.Value == -1) ? null : planets[activePlanet.Value].planetSprite;
            startImage.enabled = (activePlanet.Value != -1);
        }
        if (destinationImage) destinationImage.sprite = planets[choosenPlanet.Value].planetSprite;
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        if(!IsClient) return;
        Debug.Log($"[Client] {sceneEvent.SceneEventType} Loaded {sceneEvent.SceneName} {sceneEvent.LoadSceneMode} ");
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete) 
        {
            SetActiveSceneClientRpc(sceneEvent.SceneName);
        }
    }

    /*SELECTING PLANET*/
    public void SetSelectedPlanet(int newPlanet)
    {
        SetSelectedPlanetServerRPC(newPlanet);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetSelectedPlanetServerRPC(int newPlanet)
    {
        if(newPlanet >= planets.Length)
        {
            Debug.Log($"[Server] Invalid selection");
            return;
        }
        Debug.Log($"[Server] SetSelectedPlanetServerRPC({newPlanet})");
        choosenPlanet.Value = newPlanet;
    }

    [ContextMenu("SelectRandomPlanet")]
    public void SelectRandomPlanet()
    {
        int rand = UnityEngine.Random.Range(0, planets.Length);
        SetSelectedPlanet(rand);
    }
    [ContextMenu("GoToAridis")]
    public void SelectAridis()
    {
        SetSelectedPlanet(0);
        SetDestinationServerRPC();
    }
    [ContextMenu("GoToLirwen")]
    public void SelectLirwen()
    {
        SetSelectedPlanet(1);
        SetDestinationServerRPC();
    }

    /*SETTING DESTINATION*/
    [ContextMenu("SetDestination")]
    public void SetDestination()
    {
        if (StarshipManager.Instance && StarshipManager.Instance.IsLockTravel())
        {
            StartCoroutine(ErrorSequence("Harvex jammed systems!"));
            return;
        }

        if (!navigationSystem.IsOperative())
        {
            StartCoroutine(ErrorSequence("Navigation system broken!"));
            return;
        }

        for (int i = 0; i < engines.Length; i++) 
        {
            if (!engines[i].IsOperative())
            {
                StartCoroutine(ErrorSequence("Engine offline!"));
                return;
            }
        }
        if (startImage)
        {
            startImage.sprite = (activePlanet.Value == -1) ? null : planets[activePlanet.Value].planetSprite;
        }
        if (destinationImage) destinationImage.sprite = planets[choosenPlanet.Value].planetSprite;
        SetDestinationServerRPC();
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetDestinationServerRPC()
    {
        if(StarshipManager.Instance && StarshipManager.Instance.IsLockTravel())
        {
            Debug.Log($"[Serwer] Mission locked systems! Deal with it");
            return;
        }

        if (!navigationSystem.IsOperative())
        {
            Debug.Log($"[Serwer] Cant travel with navigation broken!");
            return;
        }

        int availablePower = 0;
        for (int i = 0; i < engines.Length; i++)
        {
            if (!engines[i].IsOperative())
            {
                Debug.Log($"[Serwer] Engine error!");
                return;
            }
            availablePower += engines[i].GetPowerLevel();
        }
        if (availablePower < 10) 
        {
            Debug.Log($"[Serwer] Not enough power! {availablePower}");
            return ;
        }

        if (inTravel.Value == true)
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

        //Check if all players are in starship

        StartCoroutine(TravelCorountine(planets[choosenPlanet.Value].cost));
    }

    //This is on server site
    private IEnumerator TravelCorountine(int travelTime)
    {
        Debug.Log($"[Serwer] Setting new destination to {planets[choosenPlanet.Value].planetSceneName} system");
        if(exitDoors!=null) exitDoors.SetDoorState(false);
        inTravel.Value = true;

        if (activePlanet.Value != -1) NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName(planets[activePlanet.Value].planetSceneName));
        SetActiveSceneClientRpc("starship");

        /*
        if (startImage) 
        {
            startImage.sprite = (activePlanet.Value == -1) ? null : planets[activePlanet.Value].planetSprite;
            startImage.enabled = (activePlanet.Value != -1);
        } 
        if (destinationImage) destinationImage.sprite = planets[choosenPlanet.Value].planetSprite;
        */
        int allTravelTime = Application.isEditor ? 5 : travelTime * 2;
        int timePassed = 0;
        travelStatus.Value = 0;

        while (timePassed <= allTravelTime)
        {
            yield return new WaitForSeconds(5);
            timePassed+=5;
            travelStatus.Value = Mathf.Clamp((float)timePassed / (float)allTravelTime,0,1);
        }
        
        Debug.Log($"[Serwer] Arrived to {planets[choosenPlanet.Value].planetSceneName} system!");
        NetworkManager.Singleton.SceneManager.LoadScene(planets[choosenPlanet.Value].planetSceneName, LoadSceneMode.Additive);
        activePlanet.Value = choosenPlanet.Value;
        inTravel.Value = false;
        if (exitDoors != null) exitDoors.SetDoorState(true);

        if (UnityEngine.Random.Range(0, 1.0f) >= 0.75f) navigationSystem.Damage();

        int allTravelCost = travelTime;
        for (int i = 0; i < engines.Length; i++)
        {
            allTravelCost = engines[i].ReducePowerLevel(allTravelCost); // Zaktualizuj pozosta�y koszt
            if (allTravelCost <= 0) break; // Przerwij p�tl�, je�li koszt zosta� ca�kowicie pokryty
            if (UnityEngine.Random.Range(0, 1.0f) > 0.9f) engines[i].SetDamageState();
        }
    }

    private void onDestinationPlanetChanged(int previousValue, int newValue)
    {
        if (newValue == previousValue) return;
        canvasAnimator.SetBool("showPlanetPanel", newValue >= 0);
        
        if (newValue >= 0)
        {
            Debug.Log($"Selected:{planets[newValue].planetName}");
            planetNameUGUI.SetText(planets[newValue].planetName);
            planetDescUGUI.SetText(planets[newValue].planetDesc);
            planetImageUGUI.sprite = planets[newValue].planetSprite;
            startButton.interactable = planets[newValue].unlocked;
        }
    }

    [ClientRpc]
    private void SetActiveSceneClientRpc(string sceneName)
    {
        if (starsEmmiter != null) 
        {
            starsEmmiter.SetActive(!inTravel.Value && sceneName == "starship");
        }
        
        if(sceneName == "InitialScene" || sceneName == "ConnectionScene") return;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        Debug.Log($"[Client] {sceneName} is active scene");
    }

    private IEnumerator ErrorSequence(string desc)
    {
        errorPanel.SetActive(true);
        errorDesc.SetText(desc);
        yield return new WaitForSeconds(5);
        errorPanel.SetActive(false);
    }

}
[System.Serializable]
public class PlanetData
{
    public string planetName;
    [Multiline]
    public string planetDesc;
    public Sprite planetSprite;
    public string planetSceneName;
    public int cost;
    public bool unlocked;
}
