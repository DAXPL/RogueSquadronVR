using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarSystem : MonoBehaviour
{
    [SerializeField] private float mortarDelay = 5;
    [SerializeField] private Transform croshair;
    private Vector3 moveVector = new Vector3(0, 0, 0);
    private float sensitivity = 5;

    private float mortarShooTimestamp = 0;
    public void MoveX(float delta)
    {
        moveVector.x = delta;
    }
    public void MoveZ(float delta)
    {
        moveVector.y = delta;
    }

    public void Fire()
    {
        if(Time.time <= mortarShooTimestamp + mortarDelay) return;
        Debug.Log("Mortar fire");
        mortarShooTimestamp = Time.time;
    }

    private void Update()
    {
        if(croshair == null) return;
        croshair.transform.Translate(moveVector*Time.deltaTime);
    }
}
