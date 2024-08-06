using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHandPanel : MonoBehaviour
{
    [SerializeField] private Transform[] tasksPanels;
    void Start()
    {
        StarshipManager.Instance.OnTaskUpdate += UpdateTasks;
        UpdateTasks();
    }
    private void OnDestroy()
    {
        StarshipManager.Instance.OnTaskUpdate -= UpdateTasks;
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
