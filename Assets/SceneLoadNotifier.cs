using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadNotifier : MonoBehaviour
{
    void OnEnable()
    {
        // Subskrypcja zdarzeñ ³adowania i usuwania scen
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        // Wyrejestrowanie zdarzeñ, aby zapobiec wyciekowi pamiêci
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Wywo³ywane, gdy scena zostanie za³adowana
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}, Load mode: {mode}");
    }

    // Wywo³ywane, gdy scena zostanie usuniêta
    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"Scene unloaded: {scene.name}");
    }
}
