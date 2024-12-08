using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Planetmanager : NetworkBehaviour
{
    [SerializeField] private string planetName;
    [SerializeField] private GameObject rainEffect;
    private NetworkVariable<bool> isRaining = new NetworkVariable<bool>();
    [SerializeField] private Serviceable[] missions;
    [SerializeField] private bool alwaysFoggy = false;
    [SerializeField] private UnityEvent onFirstVisitServer;
    [SerializeField]
    private Transform center;
    void Start()
    {
        Debug.Log($"Welcome to {planetName}");
        isRaining.OnValueChanged += OnRainStateChanged;
        if (IsOwner || IsServer) 
        {
            OnPlayersArriveServerRpc();
            if (PlayerPrefs.GetInt($"WasOn{planetName}", 0) <= 0)
            {
                PlayerPrefs.SetInt($"WasOn{planetName}", 1);
                for(int i = 0; i < missions.Length; i++)
                {
                    missions[i].Damage();
                    
                }
                if (IsServer) onFirstVisitServer.Invoke();
            }
        } 
    }

    private void OnRainStateChanged(bool previousValue, bool newValue)
    {
        if (rainEffect) 
        { 
            rainEffect.SetActive(newValue); 
            RenderSettings.fog = (newValue || alwaysFoggy);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayersArriveServerRpc()
    {
        isRaining.Value = Random.Range(0.0f, 1.0f) > 0.7f ? true:false ;
        StartCoroutine(RainEffect());
    }

    private IEnumerator RainEffect()
    {
        while (true)
        {  
            yield return new WaitForSeconds(Random.Range(30, 60));
            isRaining.Value = Random.Range(0.0f, 1.0f) > 0.7f ? true : false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController cc))
        {
            other.transform.position = center.position;
            other.transform.rotation = Quaternion.identity;
        }
        else if (other.TryGetComponent(out Rigidbody rb) && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            other.transform.position = center.position;
            other.transform.rotation = Quaternion.identity;
        }
    }

    private void OnDestroy()
    {
        isRaining.OnValueChanged -= OnRainStateChanged;
        StopAllCoroutines();
    }
}