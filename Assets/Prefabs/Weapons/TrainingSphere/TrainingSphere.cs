using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;

public class TrainingSphere : NetworkBehaviour
{
    [SerializeField] private IWeapon weapon;
    private Vector3 startPivot;
    private NetworkPlayer playerToFollow;
    [SerializeField] private float radius;
    private NetworkVariable<bool> state = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        weapon = GetComponent<IWeapon>();
        startPivot = transform.position;
        state.OnValueChanged += OnSphereStateChanged;
        if (IsServer) StartCoroutine(TrainingSphereLogic());
    }

    private void OnSphereStateChanged(bool previousValue, bool newValue)
    {
        
    }

    [ContextMenu("ActivateSphere")]
    public void ActivateSphere()
    {
        ToggleSphereServerRPC(true);
    }
    [ContextMenu("DeactivateSphere")]
    public void DeactivateSphere()
    {
        ToggleSphereServerRPC(true);
    }
    [ContextMenu("ToggleSphere")]
    public void ToggleSphere()
    {
        ToggleSphereServerRPC();
    }

    [ServerRpc]
    private void ToggleSphereServerRPC()
    {
        state.Value = !state.Value;
        if(state.Value == false)
        {
            playerToFollow = null;
            transform.position = startPivot;
        }
        else
        {
            //Searching for closest player
            NetworkPlayer[] players = GameObject.FindObjectsOfType<NetworkPlayer>();
            float dist = float.MaxValue;
            NetworkPlayer player = null;
            for (int i = 0; i<players.Length;i++)
            {
                float d = Vector3.Distance(transform.position, players[i].GetHead().position);
                if (d < dist)
                {
                    dist = d;
                    player = players[i];
                }
            }

            //Only close players count
            if(dist <= 3)
            {
                playerToFollow = player;
            }
        }
    }
    [ServerRpc]
    private void ToggleSphereServerRPC(bool newState)
    {
        state.Value = newState;
    }

    private Vector3 Target()
    {
        if (playerToFollow != null) return playerToFollow.GetHead().position;
        else return startPivot;
    }

    private IEnumerator TrainingSphereLogic()
    {
        while (true)
        {
            if(weapon == null || state.Value == false)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                //if(playerToFollow!=null) 
                Vector3 newPos = Random.insideUnitSphere.normalized * radius;
                newPos = new Vector3(newPos.x*1.5f, Mathf.Abs(newPos.y), newPos.z * 1.5f) + Target();

                float t = 0;
                Vector3 startPos = transform.position;
                while (t<=1)
                {
                    transform.position = Vector3.Lerp(startPos, newPos, t);
                    t += Time.deltaTime/2;
                    transform.LookAt(Target());
                    yield return null;
                }

                transform.position = newPos;
                transform.LookAt(Target());
                weapon.Shoot();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
