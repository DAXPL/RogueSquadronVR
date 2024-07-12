using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class ConnectionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionIP;

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
}
