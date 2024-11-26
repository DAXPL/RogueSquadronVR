using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CrashedPodController : Serviceable
{
    [SerializeField] private TextMeshProUGUI statusOutput;
    [SerializeField] private GameObject button;
    [SerializeField] private Fabricator[] adversariesSpawn;
    private NetworkVariable<int> downloadStatus = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> downloadStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        downloadStatus.OnValueChanged += UpdateStatus;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        downloadStatus.OnValueChanged -= UpdateStatus;
    }
    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        base.DamageServerRpc();
        downloadStatus.Value = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void FixServerRpc()
    {
        base.FixServerRpc();
        foreach (var fab in adversariesSpawn)
        {
            fab.ToggleActive(false);
        }
    }

    private void UpdateStatus(int previousValue, int newValue)
    {
        statusOutput.SetText((newValue<100)?$"{newValue}%":"Transfer completed");
        button.SetActive(downloadStatus.Value == 0);
    }
    [ContextMenu("StartDownloadData")]
    public void StartDownloadData()
    {
        Debug.Log("I want to download data");
        if(downloadStarted.Value == true || IsOperative()) return;
        DownloadDataServerRpc();
    }
    [ServerRpc]
    private void DownloadDataServerRpc()
    {
        Debug.Log("Downloading data");
        downloadStarted.Value = true;
        downloadStatus.Value = 1;
        StartCoroutine(DownloadCoroutnine());
    }
    private IEnumerator DownloadCoroutnine()
    {
        while (downloadStatus.Value<100)
        {
            downloadStatus.Value++;
            if (downloadStatus.Value == 20) 
            {
                foreach (var fab in adversariesSpawn)
                {
                    fab.ToggleActive(true);
                }
            }
            yield return new WaitForSeconds(1f);
        }
        FixServerRpc();
    }
}
