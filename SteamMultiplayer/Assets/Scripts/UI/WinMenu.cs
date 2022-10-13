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
    public CameraController camController;


    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")   //if we are in game scene
        {
            if (hasAuthority)          //if we are the current user
            {

                if (Input.GetKeyDown(KeyCode.Escape) && WinMenuActive)   //and win menu is active and we press escape, then we resume game
                {
                    Resume();                 //we can also resume game by pressing "resume" button
                }
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
        Cursor.lockState = CursorLockMode.None;
        camController.enabled = false;
    }

    [Command]
    public void CmdHandleMonsterDeath()     //we will call handle monster death function, but as server this time
    {
        HandleMonsterDeath();
    }

    public void Resume()
    {
        winMenuUI.SetActive(false);
        WinMenuActive = false;
        camController.enabled = true;
        camController.LockCursor();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
