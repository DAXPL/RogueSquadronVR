using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private Transform crystalPivot;
    Vector3 turningVector = new Vector3(0,0,0);
    float sensitivity = 1;
    public void TurnCrystalY(float angle)
    {
        turningVector.z = angle;
    }
    public void TurnCrystalX(float angle)
    {
        turningVector.y = angle;
    }
    public void ChangeSensitivity(float sens)
    {
        sensitivity = Mathf.Clamp(sens,0.1f,1f);
    }

    private void Update()
    {
        //crystalPivot.rotation = Quaternion.Euler(turningVector.x * 180* sensitivity, turningVector.y * 180* sensitivity, 0);
        crystalPivot.Rotate(turningVector * (sensitivity/2));
        Vector3 rot = crystalPivot.eulerAngles;
        if(rot.z < -10) rot.z = -10;
        if (rot.z > 10) rot.z = 10;
        rot = new Vector3(0, rot.y, rot.z);
        crystalPivot.eulerAngles = rot;
    }
}
