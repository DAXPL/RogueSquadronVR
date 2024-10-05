using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandPanel : MonoBehaviour
{
    [SerializeField] private TaskList taskList;
    [SerializeField] private Transform[] tasksPanels;


    private void Start()
    {
        if(taskList == null) return;
        taskList.InitializeList();
        taskList.OnListChanged += UpdateTasks;
        UpdateTasks();
    }
    private void OnDestroy()
    {
        if (taskList == null) return;
        taskList.CloseList();
        taskList.OnListChanged -= UpdateTasks;
    }

    [ContextMenu("UpdateTasks")]
    void UpdateTasks()
    {
        if(StarshipManager.Instance == null) return;

        int i = 0;
        int tasktCount = taskList.tasks.Count;
        if(tasktCount <= 0 ) return;
        foreach (var task in tasksPanels)
        {
            if (i >= tasksPanels.Length) break;
            Task t = taskList.GetTask(i);
            if (i< tasktCount && t.GetTaskState() == Task.TaskStatus.active)
            {
                tasksPanels[i].gameObject.SetActive(true);
                tasksPanels[i].GetChild(0).GetComponent<TextMeshProUGUI>()
                        .SetText(t.desc);
                tasksPanels[i].GetChild(1).GetComponent<TextMeshProUGUI>()
                        .SetText(t.name);
            }
            else
            {
                tasksPanels[i].gameObject.SetActive(false);
            }
            i++;
        }
    }
}
