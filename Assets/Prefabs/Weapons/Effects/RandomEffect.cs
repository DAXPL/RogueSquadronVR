using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] effects;

    private void Start()
    {
        int i = Random.Range(0, effects.Length);
        effects[i].Play();
    }
}
