using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadNotifier : MonoBehaviour
{
    void OnEnable()
    {
        // Subskrypcja zdarze� �adowania i usuwania scen
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        // Wyrejestrowanie zdarze�, aby zapobiec wyciekowi pami�ci
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Wywo�ywane, gdy scena zostanie za�adowana
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}, Load mode: {mode}");
    }

    // Wywo�ywane, gdy scena zostanie usuni�ta
    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"Scene unloaded: {scene.name}");
    }
}
