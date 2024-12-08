using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : MonoBehaviour
{
    [SerializeField] private ParticleSystem feathers;
    private void OnCollisionEnter(Collision collision)
    {
        if (!feathers) return;
        if(collision.impulse.magnitude / Time.fixedDeltaTime > 150.0f) feathers.Play();
    }
}
