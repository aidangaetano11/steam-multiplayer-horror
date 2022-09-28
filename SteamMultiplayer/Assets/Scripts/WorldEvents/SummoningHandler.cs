using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class SummoningHandler : NetworkBehaviour
{
    public Collider col;

    [SyncVar (hook =nameof(OnMonsterKilled))]
    public bool killed = false;

    public void OnMonsterKilled(bool oldValue, bool newValue) 
    {
        killed = newValue;
        Debug.Log("Monster Has Been Killed");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            Debug.Log("Monster Has Been Killed.");
        }
    }

}
