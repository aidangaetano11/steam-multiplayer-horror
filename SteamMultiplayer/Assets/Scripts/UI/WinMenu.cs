using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
public class WinMenu : NetworkBehaviour
{
    public static bool WinMenuActive = false;
    public GameObject winMenuUI;


    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")   //if we are in game scene
        {
            if (hasAuthority)          //if we are the current user
            {

            }
        }
    }

    public void HandleMonsterDeath()           //we will search for the altar
    {
        Debug.Log("HANDLING MONSTER DEATH");
        if (isServer)                //if we are server
        {
            RpcShowWinMenu();   //we will check if monster is killed, if it is, then show win screen 
        }
        else CmdHandleMonsterDeath();     //if we arent server, call command function
    }


    [ClientRpc]   
    public void RpcShowWinMenu()        //we will show win screen on all clients screens
    {
        WinMenuActive = true;
        winMenuUI.SetActive(true);
        StartCoroutine("HideWinMenu", 3f);
    }

    public IEnumerator HideWinMenu(float delay) 
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);
            WinMenuActive = false;
            winMenuUI.SetActive(false);
            StopCoroutine("HideWinMenu");
        }
    }

    [Command]
    public void CmdHandleMonsterDeath()     //we will call handle monster death function, but as server this time
    {
        HandleMonsterDeath();
    }
}
