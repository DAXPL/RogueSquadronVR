using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SurfaceData")]

public class SurfaceData : ScriptableObject
{
    [SerializeField] private string surfaceName;
    [SerializeField] private GameObject hitEffects;

    public GameObject GetHitEffects()
    {
        return hitEffects;
    }
}
