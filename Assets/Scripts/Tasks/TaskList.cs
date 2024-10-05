using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TaskList")]
public class TaskList : ScriptableObject
{
    public List<Task> tasks = new List<Task>();

    public delegate void OnTaskListUpdate();
    public OnTaskListUpdate OnListChanged;

    public void InitializeList()
    {
        foreach (Task task in tasks)
        {
            task.OnStatusChanged += OnTaskStateChanged;
        }
    }

    public void CloseList()
    {
        foreach (Task task in tasks)
        {
            task.OnStatusChanged -= OnTaskStateChanged;
        }
    }
    
    private void OnTaskStateChanged()
    {
        OnListChanged.Invoke();
    }

    public Task GetTask(int id)
    {
        if(id<tasks.Count)
            return tasks[id];
        return null;    
    }

    
}
