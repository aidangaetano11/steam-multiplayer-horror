using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [Header("Spawnable Prefabs")]
    public GameObject keyPrefab;
    public GameObject spawnPrefab;
    public GameObject redPotionPrefab;
    public GameObject hammerPrefab;
    public GameObject tpPrefab;
    public GameObject plantPrefab;
    public GameObject SCPrefab;
    public GameObject blackHolePrefab;

    //player
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController> ();  //create list of every player connected

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby") 
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
             
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);  
        }
    }

    public override void OnStartClient() 
    {
        if (NetworkClient.Ready())
        {
            NetworkClient.RegisterPrefab(keyPrefab);           //these will register object prefabs for the client to see and use
            NetworkClient.RegisterPrefab(spawnPrefab);
            NetworkClient.RegisterPrefab(redPotionPrefab);
            NetworkClient.RegisterPrefab(hammerPrefab);
            NetworkClient.RegisterPrefab(tpPrefab);
            NetworkClient.RegisterPrefab(plantPrefab);
            NetworkClient.RegisterPrefab(SCPrefab);
            NetworkClient.RegisterPrefab(blackHolePrefab);
        }
              
    }

    public void StartGame(string SceneName) 
    {
        ServerChangeScene(SceneName);
    }
}
