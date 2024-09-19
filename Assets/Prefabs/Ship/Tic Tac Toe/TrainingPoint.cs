using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrainingPoint : NetworkBehaviour, IDamageable
{
    [SerializeField] private TrainingManager manager;
    private Vector2 range = new Vector2(2.5f, 1);
    public void Damage(int dmg)
    {
        transform.localPosition = new Vector2(UnityEngine.Random.Range(-range.x, range.x), UnityEngine.Random.Range(-range.y, range.y));
        if (manager == null) return;
        manager.OnPointHit();
    }
}
