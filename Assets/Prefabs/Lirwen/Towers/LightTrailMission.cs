using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrailMission : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        RaycastHit hit;
        Transform endRay = this.transform;
        bool hasHit = true;
        Debug.DrawRay(endRay.transform.position, endRay.forward * 1000, Color.blue);

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if (hasHit)
            {
                lineRenderer.SetPosition(i, endRay.position);
                Debug.DrawRay(endRay.position, endRay.forward * 1000, Color.blue);
            }
            else
            {
                lineRenderer.SetPosition(i, endRay.position + endRay.forward * 100);
                Debug.DrawRay(endRay.position, endRay.forward * 1000, Color.red);
            }
            Physics.Raycast(endRay.position, endRay.forward, out hit);
            hasHit = hit.collider != null && (hit.collider.name == "crystal" || hit.collider.name == "crystal_end");
            if (hasHit)
            {
                endRay = hit.collider.transform;
                if(hit.collider.name != "crystal_end") continue;
                Debug.Log("End!");
            }
            
        }
    }
}
