using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
public class GameController : MonoBehaviour
{
    private CustomNetworkManager manager;
    PlayerMovementController playerController;

    public GameObject LocalPlayerObject;
    public PlayerObjectController LocalPlayerController;
   


    private void Start()
    {
        playerController = new PlayerMovementController();
    }

    private CustomNetworkManager Manager      //accesses the manager or some shit
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            if (NetworkClient.ready) 
            {
                Debug.Log("Client is lit");
            }
            //int randomIndex = Random.Range(0, Manager.GamePlayers.Count);
            //Debug.Log(randomIndex);
            //Debug.Log(Manager.GamePlayers[randomIndex].PlayerName);
        }
    }

}
