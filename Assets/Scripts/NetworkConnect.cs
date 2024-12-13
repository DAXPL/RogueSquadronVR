using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class NetworkConnect : MonoBehaviour
{
    private ConnectionPanel conPanel;
    private void Start()
    {
        conPanel = GetComponent<ConnectionPanel>();
        if (NetworkManager.Singleton) 
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        } 
    }
    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong obj)
    {
        if (conPanel) conPanel.RaiseError("ERROR! \n Can't connect to server!");
        Debug.Log("Womp womp!");
    }

    private void OnClientConnected(ulong obj)
    {
        Debug.Log("Connected!");
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyUp(KeyCode.Home)) Create();
            if (Input.GetKeyUp(KeyCode.Insert)) Join();
        }
    }
    [ContextMenu("Create server")]
    public void Create()
    {
        if (!NetworkManager.Singleton)
        {
            if (conPanel) conPanel.RaiseError("ERROR! \n There is no NetworkManager instance");
            return;
        }
        SceneManager.UnloadSceneAsync(1);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.PostSynchronizationSceneUnloading = false;
        NetworkManager.Singleton.SceneManager.LoadScene("starship", LoadSceneMode.Additive);
    }
  
    [ContextMenu("Join")]
    public void Join()
    {
        if (!NetworkManager.Singleton)
        {
            if (conPanel) conPanel.RaiseError("ERROR! \n There is no NetworkManager instance");
            return;
        }
        NetworkManager.Singleton.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
