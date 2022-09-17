using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;           //syncvar syncs variable to every client to server
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;     //every time this variable changes, it will call a function
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    //Cosmetics
    [SyncVar(hook = nameof(SendPlayerColor))] public int playerColor;
    [SyncVar(hook = nameof(SendChosenPlayerNameText))] public int chosenPlayerNameIndex;
    

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
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

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }
        if (isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CMdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CMdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)   //Host
        {
            this.PlayerName = NewValue;
        }
        if (isClient)   //Client
        {
            LobbyController.instance.UpdatePlayerList();
        }

    }

    //Start Game

    public void CanStartGame(string SceneName)
    {
        if (hasAuthority)
        {
            CmdCanStartGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }


    //Cosmetics

    [Command]
    public void CmdUpdatePlayerColor(int newValue)
    {
        SendPlayerColor(playerColor, newValue);
    }

    [Command]
    public void CmdUpdateChosenPlayerNameText(int newValue) 
    {
        SendChosenPlayerNameText(chosenPlayerNameIndex, newValue);
    }

    public void SendPlayerColor(int oldValue, int newValue) 
    {
        if (isServer) 
        {
            playerColor = newValue;
        }

        if (isClient && (oldValue != newValue)) 
        {
            playerColor = newValue;
        }
    }

    public void SendChosenPlayerNameText(int oldValue, int newValue) 
    {
        if (isServer) 
        {
            chosenPlayerNameIndex = newValue;
        }

        if (isClient && (oldValue != newValue)) 
        {
            chosenPlayerNameIndex = newValue;
        }
    }
}
