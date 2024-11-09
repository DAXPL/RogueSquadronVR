using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.OpenXR.Input;

public class Charger : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI powerLevelText;
    private PowerModule powerModule;
    private float loadTimestamp;
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log($"Inserted {args.interactableObject.transform.gameObject.name}");
        if (!IsServer) return;
        if (args.interactableObject.transform.TryGetComponent(out PowerModule pm))
        {
            powerModule = pm;
            if(powerLevelText!=null) powerLevelText.SetText($"{powerModule.power.Value}");
        }
    }
    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log($"Removed {args.interactableObject.transform.gameObject.name}");
        if (!IsServer) return;
        if (args.interactableObject.transform.TryGetComponent(out PowerModule pm))
        {
            if(pm == powerModule) powerModule = null;
            if (powerLevelText != null) powerLevelText.SetText("Insert power module");
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        if(powerModule == null) return;
        if(Time.time <= loadTimestamp+5) return;
        loadTimestamp = Time.time;
        powerModule.LoadModule(10,150);
        if (powerLevelText != null) powerLevelText.SetText($"{powerModule.power.Value}");
    }
}
