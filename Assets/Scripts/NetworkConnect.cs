using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkConnect : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene(1,LoadSceneMode.Additive);
    }
    public void Create()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
