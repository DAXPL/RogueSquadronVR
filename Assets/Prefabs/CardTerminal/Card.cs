using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private string cardId = "";
    
    public string GetCardID()
    {
        return cardId;
    }
}
