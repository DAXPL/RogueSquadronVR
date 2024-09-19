using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TrainingManager : NetworkBehaviour
{
    [SerializeField] private TrainingPoint point;
    private NetworkVariable<bool> inProgress = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> points = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject stoptButton;

    float shootTimestamp = 0;
    float shoots = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        inProgress.OnValueChanged += ProgressChanged;
        points.OnValueChanged += PointsChanged;
    }

    private void PointsChanged(int previousValue, int newValue)
    {
        if (pointsText != null) pointsText.SetText($"{points.Value} pkt");
    }

    private void ProgressChanged(bool previousValue, bool newValue)
    {
        point.gameObject.SetActive(newValue);
        startButton.SetActive(!newValue);
        stoptButton.SetActive(newValue);
    }

    public void OnPointHit()
    {
        OnPointHitServerRpc();
    }
   
    public void ChangeTrainingState(bool newState)
    {
        ChangeTrainingStateServerRpc(newState);
    }

    [ServerRpc]
    public void OnPointHitServerRpc()
    {
        if(inProgress.Value == false) return;

        shoots++;

        float shootTime = Time.time - shootTimestamp;
        shootTimestamp = Time.time;

        int p = (int)Mathf.Clamp(4-shootTime+(0.5f),0,5);
        points.Value += p;

        if (shoots >= 10) ChangeTrainingStateServerRpc(false);
    }
    [ServerRpc]
    public void ChangeTrainingStateServerRpc(bool newState)
    {
        if (newState != inProgress.Value) inProgress.Value = newState;

        if (newState)
        {
            shoots = 0;
            points.Value = 0;
            shootTimestamp = Time.time;
        }
    }

    /*DEBUG*/

    [ContextMenu("Change state")]
    public void DebugChangeState()
    {
        ChangeTrainingState(!inProgress.Value);
    }
    [ContextMenu("Hit")]
    public void DebugHit()
    {
        point.Damage(0);
    }
}
