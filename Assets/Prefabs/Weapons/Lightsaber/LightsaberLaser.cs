using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LightsaberLaser : NetworkBehaviour
{
    [SerializeField] private Lightsaber saber;
    public void OnCollisionEnter(Collision collision)
    {
        //Only owner can initiate damage sequence
        if (IsOwner == false) return;

        if (collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage((saber!=null)?saber.GetDamage():0);
        }

    }
}
