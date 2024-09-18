using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SurfaceInfo : MonoBehaviour
{
    [SerializeField] private SurfaceData surfaceData;

    public SurfaceData GetSurfaceData()
    {
        return surfaceData;
    }
}
