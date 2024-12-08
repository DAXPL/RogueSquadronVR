using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkConnect : MonoBehaviour
{
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
        SceneManager.UnloadSceneAsync(1);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.PostSynchronizationSceneUnloading = false;
        NetworkManager.Singleton.SceneManager.LoadScene("starship", LoadSceneMode.Additive);
    }
    [ContextMenu("Join")]
    public void Join()
    {
        Debug.Log("I want to join!");
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("ConnectionScene"));
        NetworkManager.Singleton.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
