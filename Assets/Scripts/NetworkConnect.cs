using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkConnect : MonoBehaviour
{
    [ContextMenu("Create server")]
    public void Create()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene("starship", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(1);
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
