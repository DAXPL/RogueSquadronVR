using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHandPanel : MonoBehaviour
{
    [SerializeField] private TaskList taskList;
    [SerializeField] private Transform[] tasksPanels;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
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
        int tasktCount = taskList.tasks.Count;
        int panelCount = tasksPanels.Length;
        int currentPanel = 0;

        for (int i = 0; i < tasktCount && i< panelCount; i++)
        {
            Task t = taskList.GetTask(i);
            if (t.GetTaskState() == Task.TaskStatus.active)
            {
                tasksPanels[currentPanel].gameObject.SetActive(true);
                tasksPanels[currentPanel].GetChild(0).GetComponent<TextMeshProUGUI>()
                        .SetText(t.desc);
                tasksPanels[currentPanel].GetChild(1).GetComponent<TextMeshProUGUI>()
                        .SetText(t.name);
                currentPanel++;
            }
        }

        for (; currentPanel< panelCount; currentPanel++)
        {
            tasksPanels[currentPanel].gameObject.SetActive(false);
        }
        if(source) source.Play();
    }
}
