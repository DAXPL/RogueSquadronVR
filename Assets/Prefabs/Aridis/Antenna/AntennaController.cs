using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AntennaController : Serviceable
{
    [SerializeField] private Transform antenna;
    private NetworkVariable<float> wantedAngle = new NetworkVariable<float>();
    private float angleTolerance = 15.0f;
    [SerializeField] private GameObject[] indicators;

    public delegate void OnAntennaStatusChangedDelegate();
    public OnAntennaStatusChangedDelegate OnStatusChanged;

    protected override void onStatusChanged(bool previousValue, bool newValue)
    {
        base.onStatusChanged(previousValue, newValue);
        if (OnStatusChanged != null) OnStatusChanged();
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DamageServerRpc()
    {
        base.DamageServerRpc();
        wantedAngle.Value = Random.Range(0, 24)*15.0f;
    }
   
    [ContextMenu("TryActivateAntenna")]
    public void TryActivateAntenna()
    {
        if (IsOperative() == true) return;
        TryActivateAntennaServerRpc(antenna.rotation.eulerAngles.y % 360);
    }
    [ServerRpc]
    public void TryActivateAntennaServerRpc(float angle)
    {
        if(IsAngleWithinTolerance(angle, wantedAngle.Value, angleTolerance))
        {
            FixServerRpc();
        }
    }

    public void TurnAntenna(float angle)
    {
        if (IsOperative()) return;
        antenna.rotation = Quaternion.Euler(0, angle * 180, 0);
        if(IsAngleWithinTolerance(antenna.rotation.eulerAngles.y % 360, wantedAngle.Value, angleTolerance))
        {
            indicators[0].SetActive(true);
            indicators[1].SetActive(false);
        }
        else
        {
            indicators[0].SetActive(false);
            indicators[1].SetActive(true);
        }
    }

    private bool IsAngleWithinTolerance(float currentAngle, float targetAngle, float tolerance)
    {
        return Mathf.Abs(currentAngle - targetAngle) <= tolerance;
    }
}
