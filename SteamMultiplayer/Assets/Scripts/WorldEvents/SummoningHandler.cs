using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class SummoningHandler : NetworkBehaviour
{
    public Collider col;
    public Transform portalSpawn;
    public GameObject portal;

    [SyncVar (hook =nameof(OnMonsterKilled))]
    public bool killed = false;

    public void OnMonsterKilled(bool oldValue, bool newValue) 
    {
        killed = newValue;
    }

    [Server]
    public void ShowWinScreen() 
    {
        Debug.Log("THIS SHIT IS CALLED");
        foreach (WinMenu w in FindObjectsOfType<WinMenu>()) 
        {
            w.RpcShowWinMenu();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            Debug.Log("Monster Has Been Killed.");
            GameObject blackHole = Instantiate(portal, portalSpawn.position, Quaternion.Euler(90, 0, 0));
            NetworkServer.Spawn(blackHole);
            other.gameObject.transform.position = new Vector3(0f, 15f, 0f);
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            other.gameObject.GetComponent<MonsterAI>().enabled = false;      
            ShowWinScreen();
        }
    }

}
