using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StarshipManager : NetworkBehaviour
{
    [SerializeField] private List<Task> tasks = new List<Task>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (Task task in tasks)
        {
            task.serviceInterface.OnStatusChanged += OnTaskStatusChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (Task task in tasks)
        {
            task.serviceInterface.OnStatusChanged -= OnTaskStatusChanged;
        }
    }

    public void OnTaskStatusChanged()
    {
        WriteoutTasks();
    }

    [ContextMenu("OutputTasks")]
    public void WriteoutTasks()
    {
        foreach (Task task in tasks)
        {
            if(task.serviceInterface.IsOperative() == false)
                Debug.Log($"Task: {task.desc}");
        }
    }

    [ContextMenu("SetTasks")]
    public void SetTasks()
    {
        if (!IsServer) return;
        foreach (Task task in tasks)
        {
            if (Random.Range(0,5)>3)
                task.serviceInterface.Damage();
        }
    }

}
[System.Serializable]
public class Task
{
    public string desc;
    public Serviceable serviceInterface;
}