using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawnerManager : MonoBehaviour, INetworkRunnerCallbacks, IGameManager
{
    [SerializeField] private NetworkPrefabRef _playerPolicemanPrefab;
    [SerializeField] private NetworkPrefabRef _playerHooliganPrefab;

    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public ManagerStatus Status { get; private set; }

    private bool _mouseButton0;
    private bool _mouseButton1;
    
    public void Startup(NetworkService networkService)
    {
        Debug.Log("Data manager ProcessStartInfo...");

        Status = ManagerStatus.Started;
    }
    
   
    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton0 = _mouseButton1 | Input.GetMouseButton(1);
    }
    
    async void StartGame(GameMode gameMode)
    {
        /*
         * Create the Fusion runner and let it know that we will be providing user input
         */
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        /*
         * Start or join (depends on gameMode) a session with a specific name
         *
         * The current scene index is passed in,
         * but this is only relevant for the host
         * as clients will be forced to use the scene specified by the host.
         */
        Dictionary<string, SessionProperty> sessionProperties = new Dictionary<string, SessionProperty>();
        switch (gameMode)
        {
            case GameMode.Host:
                sessionProperties["playerType"] = "policeman";
                break;
            case GameMode.Client:
                sessionProperties["playerType"] = "hooligan";
                break;
        }
        
        await _runner.StartGame(new StartGameArgs()
        {
           
           SessionProperties = sessionProperties,
            GameMode = gameMode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }

            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        /*
         * Create a unique position for the player
         */
        Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
        // todo spawn base on param
        NetworkObject networkPlayerObject;
        if (runner.SessionInfo.Properties != null 
            && runner.SessionInfo.Properties.ContainsKey("playerType"))
        {
            if (runner.SessionInfo.Properties["playerType"].PropertyValue.Equals("policeman"))
            {
                networkPlayerObject = runner.Spawn(_playerPolicemanPrefab, spawnPosition, Quaternion.identity, player);
            }
            else
            {
                networkPlayerObject = runner.Spawn(_playerHooliganPrefab, spawnPosition, Quaternion.identity, player);
            }
        }
        else
        {
            /*
             * Spawn hooligan by default
             */
            networkPlayerObject = runner.Spawn(_playerHooliganPrefab, spawnPosition, Quaternion.identity, player);
        }
        
        /*
         * Keep track of the player avatars so we can remove it when they disconnect
         */
        _spawnedCharacters.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        /*
         * Find and remove the players avatar
         */
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        if (_mouseButton0)
        {
            data.buttons |= NetworkInputData.MOUSEBUTTON1;
        }
        _mouseButton0 = false;

        if (Input.GetKey(KeyCode.F))
        {
            data.buttons |= NetworkInputData.MOUSEBUTTON2;
        }
        _mouseButton1 = false;
        
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("OnInputMissing");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("OnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadStart");
    }

}