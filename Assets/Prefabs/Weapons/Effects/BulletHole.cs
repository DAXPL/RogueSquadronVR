using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletHole : MonoBehaviour
{
    [SerializeField] private SurfaceData defaultSurfaceData;
    private SurfaceData surfaceData;
    private void Start()
    {
        RaycastHit hit;
        GameObject g;
        if (Physics.Raycast(transform.position,transform.forward,out hit) == false) return;
        if(hit.collider.TryGetComponent(out SurfaceInfo surface))
            g =Instantiate(surface.GetSurfaceData().GetHitEffects(), transform.position, transform.rotation, hit.collider.transform);
        else
            g = Instantiate(defaultSurfaceData.GetHitEffects(), transform.position, transform.rotation, hit.collider.transform);
        Destroy(g,30);
        Destroy(this);

    }
}
