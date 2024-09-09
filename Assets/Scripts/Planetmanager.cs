using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Planetmanager : MonoBehaviour
{
    [SerializeField] private string planetName;
    void Start()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(planetName));
        Debug.Log($"Welcome to {planetName}");
    }
}
