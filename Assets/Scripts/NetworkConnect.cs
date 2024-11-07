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
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Loading starship as host");
            NetworkManager.Singleton.SceneManager.LoadScene("starship", LoadSceneMode.Additive);
            Debug.Log("Unloading connection scene");
            SceneManager.UnloadSceneAsync(1);
            Debug.Log("Done");
        }
        else
        {
            Debug.LogError("Cant start host");
        }
        
    }
    [ContextMenu("Join")]
    public void Join()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            SceneManager.UnloadSceneAsync(1);
        }
        else
        {
            Debug.LogError("Cant connect to server");
        }
        
    }
}
