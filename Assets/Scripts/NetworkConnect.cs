using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkConnect : MonoBehaviour
{
    [ContextMenu("Connect")]
    public void Create()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("starship", LoadSceneMode.Additive);
        //SceneManager.LoadScene(2, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(1);
    }
    [ContextMenu("Join")]
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        SceneManager.UnloadSceneAsync(1);
    }
}
