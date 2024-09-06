using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomizeBuilding : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    [ContextMenu("Randomize assets")]
    public void RandomizeAssets()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(1==Random.Range(0,2));
        }
    }
}
