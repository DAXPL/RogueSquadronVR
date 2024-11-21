using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private LayerMask CollisionMask;
    private LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    void FixedUpdate()
    {
        lineRenderer.SetPosition(0,transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100, CollisionMask)) lineRenderer.SetPosition(1, hit.point);
        else lineRenderer.SetPosition(1, this.transform.forward * 100);
    }
}
