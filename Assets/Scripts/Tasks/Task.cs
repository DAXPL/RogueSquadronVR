using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Task")]
public class Task : ScriptableObject
{
    public string taskName;
    [Multiline]
    public string desc;

    public delegate void OnStatusChangedDelegate();
    public OnStatusChangedDelegate OnStatusChanged;

    public enum TaskStatus {active, deactive }
    [SerializeField] private TaskStatus taskState;

    public void onStatusChanged(bool newValue)
    {
        taskState = newValue ? TaskStatus.deactive : TaskStatus.active;

        OnStatusChanged.Invoke();
    }

    public TaskStatus GetTaskState()
    {
        return taskState;
    }
    public void SetTaskState(TaskStatus newTaskState) 
    {
        taskState=newTaskState;
    }
    [ContextMenu("DebugOnStatusChanged")]
    public void DebugOnStatusChanged()
    {
        OnStatusChanged.Invoke();
    }

}
