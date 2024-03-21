using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager Instance { get; private set; } = null;

    private FacepunchTransport transport = null;

    public Lobby? CurrentLobby { get; private set; } = null;

    public ulong hostId;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;

    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        if(NetworkManager.Singleton == null)
        {
            return;
        }
        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;

    }

    private void OnApplicationQuit()
    {
        Disconnected();
    }

    //when you accept the invite or Join on a friend
    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamId)
    {
        RoomEnter joinedLobby = await _lobby.Join();
        if(joinedLobby != RoomEnter.Success)
        {
            Debug.Log("Failed to create lobby");
        }
        else
        {
            CurrentLobby = _lobby;
            GameManager.Instance.ConnectedAsClient();
            Debug.Log("Joined Lobby");
        }
    }

    private void SteamMatchmaking_OnLobbyGameCreated(Lobby _lobby, uint _ip, ushort _port, SteamId _steamId)
    {
        Debug.Log("Lobby was created");
        GameManager.Instance.SendMessageToChat($"Lobby was created", NetworkManager.Singleton.LocalClientId, true);

    }

    //friend send you an steam invite
    private void SteamMatchmaking_OnLobbyInvite(Friend _steamId, Lobby _lobby)
    {
        Debug.Log($"Invite from {_steamId.Name}");
    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _steamId)
    {
        Debug.Log("member leave");
        GameManager.Instance.SendMessageToChat($"{_steamId.Name} has left", _steamId.Id, true);
        NetworkTransmission.Instance.RemoveMeFromDictionaryServerRPC(_steamId.Id);
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend _steamId)
    {
        Debug.Log("member join");
    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }
        StartClient(CurrentLobby.Value.Owner.Id);

    }

    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if(_result != Result.OK)
        {
            Debug.Log("lobby was not created");
            return;
        }
        _lobby.SetPublic();
        _lobby.SetJoinable(true);
        _lobby.SetGameServer(_lobby.Owner.Id);
        Debug.Log($"lobby created FakeSteamName");
        NetworkTransmission.Instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, "FakeSteamName", NetworkManager.Singleton.LocalClientId); //
    }

    public async void StartHost(int _maxMembers)
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();
        GameManager.Instance.myClientId = NetworkManager.Singleton.LocalClientId;
        CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(_maxMembers);
    }

    public void StartClient(SteamId _sId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        transport.targetSteamId = _sId;
        GameManager.Instance.myClientId = NetworkManager.Singleton.LocalClientId;
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client has started");
        }
    }

    public void Disconnected()
    {
        CurrentLobby?.Leave();
        if(NetworkManager.Singleton == null)
        {
            return;
        }
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        }
        NetworkManager.Singleton.Shutdown(true);
        GameManager.Instance.ClearChat();
        GameManager.Instance.Disconnected();
        Debug.Log("disconnected");
    }

    private void Singleton_OnClientDisconnectCallback(ulong _cliendId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
        if(_cliendId == 0)
        {
            Disconnected();
        }
    }

    private void Singleton_OnClientConnectedCallback(ulong _cliendId)
    {
        NetworkTransmission.Instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, _cliendId);
        GameManager.Instance.myClientId = _cliendId;
        NetworkTransmission.Instance.IsTheClientReadyServerRPC(false, _cliendId);
        Debug.Log($"Client has connected : AnotherFakeSteamName");
    }

    private void Singleton_OnServerStarted()
    {
        Debug.Log("Host started");
        GameManager.Instance.HostCreated();
    }

    public void Test()
    {
        Debug.Log("Clicked");
    }
}
