using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour, IDamageable
{
    [SerializeField] private UniversalRemoteProcedureCall urpc;
    public void Damage(int dmg)
    {
        if (urpc != null) urpc.OnButtonClicked();
    }
}
