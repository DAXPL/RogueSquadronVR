using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MortarSystem : NetworkBehaviour
{
    [SerializeField] private float mortarDelay = 5;
    [SerializeField] private Transform croshair;
    [SerializeField] private Camera croshairCamera;
    [SerializeField] private GameObject effect;
    [SerializeField] private float explosionRadius = 10.0f;
    [SerializeField] private int baseDamage = 100;
    private Vector3 moveVector = new Vector3(0, 0, 0);
    private float sensitivity = 5;
    private bool canShoot=true;
    private float mortarShooTimestamp = 0;

    private void Start()
    {
        croshairCamera.enabled = false;
    }

    public void MoveX(float delta)
    {
        moveVector.y = delta;
    }
    public void MoveZ(float delta)
    {
        moveVector.x = -delta;
    }
    public void LockMortar(bool newState)
    {
        canShoot = newState;
    }
    public void Fire()
    {
        if (!canShoot) return;
        if (!IsOwner) return;
        if(Time.time <= mortarShooTimestamp + mortarDelay) return;
        Debug.Log("Mortar fire");
        mortarShooTimestamp = Time.time;

        RaycastHit hit;
        if(!Physics.Raycast(croshair.position, croshair.forward,out hit, 200f)) return;

        RaycastHit[] hits = Physics.SphereCastAll(hit.point, explosionRadius, Vector3.up);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == this.transform) continue;
            if (hits[i].transform.TryGetComponent(out IDamageable damageable))
            {
                float distance = Mathf.Clamp(Vector3.Distance(this.transform.position, hits[i].transform.position), 1, explosionRadius); ;
                float multipiler = explosionRadius / distance;
                damageable.Damage((int)(baseDamage * multipiler));
            }

            if (hits[i].collider.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(75, this.transform.position, explosionRadius * 1.2f);
            }

        }
        SyncShootEffectServerRpc(hit.point);
    }

    private void Update()
    {
        if(!canShoot)return;
        if(croshair == null) return;
        croshair.transform.Translate(moveVector*Time.deltaTime*3);
        croshairCamera.Render();
    }
    [ServerRpc]
    private void SyncShootEffectServerRpc(Vector3 pos)
    {
        SyncShootEffectClientRpc(pos);
    }

    [ClientRpc]
    private void SyncShootEffectClientRpc(Vector3 pos)
    {
        if (effect == null) return;
        GameObject go = Instantiate(effect, pos, Quaternion.identity);
        Destroy(go, 30);
    }
}
