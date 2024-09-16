using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarshipManager : NetworkBehaviour
{
    public static StarshipManager Instance;
    [SerializeField] private Transform spawnPoint;

    public List<Task> tasks = new List<Task>();

    public delegate void OnTaskUpdateDelegate();
    public OnTaskUpdateDelegate OnTaskUpdate;

    private void Awake()
    {
        if (Instance != null) 
        { 
            Destroy(this); 
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if (localPlayer != null && spawnPoint != null)
        {
            localPlayer.transform.position = spawnPoint.position;
            localPlayer.transform.rotation = spawnPoint.rotation;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (Task task in tasks)
        {
            task.serviceInterface.OnStatusChanged += OnTaskStatusChanged;
        }

        GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
        if(localPlayer != null && spawnPoint!= null)
        {
            localPlayer.transform.position = spawnPoint.position;
            localPlayer.transform.rotation = spawnPoint.rotation;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("starship"));
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
        OnTaskUpdate.Invoke();
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
        Debug.Log("[Server] Setting tasks");
        foreach (Task task in tasks)
        {
            if (Random.Range(0,5)>3)
                task.serviceInterface.Damage();
        }
    }
    [ContextMenu("SetAllTasks")]
    public void SetAllTasks()
    {
        if (!IsServer) return;
        Debug.Log("[Server] Setting all tasks");
        foreach (Task task in tasks)
        {
            task.serviceInterface.Damage();
        }
    }

}
[System.Serializable]
public class Task
{
    public string name;
    [Multiline]
    public string desc;
    public Serviceable serviceInterface;
}