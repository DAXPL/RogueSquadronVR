using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandPanel : MonoBehaviour
{
    [SerializeField] private Transform[] tasksPanels;
    StarshipManager starshipManager;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(starshipManager != null) return;

        StarshipManager instance = StarshipManager.Instance;
        if(instance != null )
        {
            starshipManager = instance;
            starshipManager.OnTaskUpdate += UpdateTasks;
            UpdateTasks();
        }
    }
    private void OnDestroy()
    {
        if(starshipManager != null)
            starshipManager.OnTaskUpdate -= UpdateTasks;
    }

    [ContextMenu("UpdateTasks")]
    void UpdateTasks()
    {
        if(StarshipManager.Instance == null) return;

        List<Task> tasks = StarshipManager.Instance.tasks;
        int i = 0;
        int tasktCount = tasks.Count;

        foreach (var task in tasksPanels)
        {
            if (i >= tasksPanels.Length) break;

            if (i< tasktCount && tasks[i].serviceInterface.IsOperative() == false)
            {
                tasksPanels[i].gameObject.SetActive(true);
                tasksPanels[i].GetChild(0).GetComponent<TextMeshProUGUI>()
                        .SetText(tasks[i].desc);
                tasksPanels[i].GetChild(1).GetComponent<TextMeshProUGUI>()
                        .SetText(tasks[i].name);
            }
            else
            {
                tasksPanels[i].gameObject.SetActive(false);
            }
            i++;
        }
    }
}
