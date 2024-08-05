using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI output;
    void OnEnable()
    {
        Application.logMessageReceivedThreaded += Log;
    }
    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= Log;
    }

    private void Log(string logString, string stackTrace, LogType type)
    {
        output.text += $">{logString}\n";
    }
}
