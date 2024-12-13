using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
public class ConnectionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionIP;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private UnityEvent onEndError;
    private string ip;
    public void OnEnteredNumber(string s)
    {
        ip += s;
        UpdateConnectionIP();
    }
    public void OnRemovedNumber()
    {
        ip=ip.Substring(0, ip.Length - 2);
        UpdateConnectionIP();
    }
    private void UpdateConnectionIP()
    {
        connectionIP.text = ip;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, 7777);
    }
    public void RaiseError(string errorCode)
    {
        if(errorText) errorText.SetText(errorCode);
        StartCoroutine(ErrorCorountine());
    }
    private IEnumerator ErrorCorountine()
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        errorPanel.SetActive(false);
        onEndError.Invoke();
    }
}
