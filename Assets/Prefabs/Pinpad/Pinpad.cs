using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pinpad : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> onInputNumber;
    [SerializeField] private UnityEvent onClear;
    public void InputNumber(int number)
    {
        onInputNumber.Invoke(number.ToString());
    }
    public void InputDot()
    {
        onInputNumber.Invoke(".");
    }
    public void Clear()
    {
        onClear.Invoke();
    }
}
