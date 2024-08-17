using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialManager : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene(1,LoadSceneMode.Additive);
    }
}
