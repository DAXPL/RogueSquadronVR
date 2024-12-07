using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MagazineController : NetworkBehaviour
{
    public static MagazineController Instance;
    [SerializeField] private TextMeshProUGUI creditsText;
    private NetworkVariable<int> credits = new NetworkVariable<int>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddCreditBalanceServerRpc(int value)
    {
        credits.Value += value;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        credits.OnValueChanged += OnCreditsUpdates;
    }

    private void OnCreditsUpdates(int previousValue, int newValue)
    {
        if(creditsText == null) return;
        creditsText.SetText($"{credits.Value}$");
    }
}
