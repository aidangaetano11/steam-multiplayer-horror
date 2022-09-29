using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
public class PauseMenu : NetworkBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public CameraController camController;

    public LobbyController lobbyController;

    void Update()
    {
        if (hasAuthority && SceneManager.GetActiveScene().name == "Game") 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
        
    }

    public void Resume() 
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        camController.enabled = true;
        camController.LockCursor();
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        camController.enabled = false;      
    }

    public void LoadMenu() 
    {   
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame() 
    {
        Debug.Log("Quitting Game....");
        Application.Quit();
    }
}
