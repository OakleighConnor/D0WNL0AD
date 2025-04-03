using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using Unity.VisualScripting;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef player1PF, player2PF;
    SessionListUIHandler sessionListUIHandler;
    public CodeManager codeManager;
    public int value;
    public bool localPlay;
    public GameObject playerInputPF;
    public PlayerInputScript inputP1, inputP2; // Make each client individually access this
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    void Awake()
    {
        value = PlayerPrefs.GetInt("LocalPlay");

        Debug.Log(value);

        if (value == 1)
        {
            localPlay = true;
        }
        else
        {
            localPlay = false;
        }
        
        // UI Elements
        sessionListUIHandler = FindAnyObjectByType<SessionListUIHandler>(FindObjectsInactive.Include);

        inputP1 = Instantiate(playerInputPF, transform, worldPositionStays: false).GetComponent<PlayerInputScript>();

        if (localPlay/*PlayerPrefs.GetInt("LocalPlay") == 1 DELETE THIS IF THE LOCAL MULTIPLAYER FUNCTIONS FINE*/)
        {
            inputP2 = Instantiate(playerInputPF, transform, worldPositionStays: false).GetComponent<PlayerInputScript>();
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (runner.IsServer)
        {
            Debug.Log("OnPlayerJoined we are server. Spawning player");

            runner.Spawn(player1PF, new Vector3(0,10,0), Quaternion.identity, player).GetComponent<PlayerScript>();

            if (PlayerPrefs.GetInt("LocalPlay") == 1)
            {
                runner.Spawn(player2PF, new Vector3(10,10,0), Quaternion.identity, player).GetComponent<PlayerScript>().player = 2;
            }
        }
        else
        {
            Debug.Log("OnPlayerJoined");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        
    }
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.directionP1 += inputP1.movementInput;

        if(inputP2 != null)
        {
            data.directionP2 += inputP2.movementInput;
        }

        input.Set(data);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        // Updates the helper's sessionlist so that it can compare new codes to existing ones to ensure all codes are unique
        codeManager.sessionList.Clear();
        foreach(SessionInfo sessionInfo in sessionList)
        {
            codeManager.sessionList.Add(sessionInfo.Name);
        }

        // Only update the list of sessions when the session list UI handler is active
        if(sessionListUIHandler == null) return;

        List<SessionInfo> publicSessions = new List<SessionInfo>();

        foreach (SessionInfo sessionInfo in sessionList)
        {
            sessionInfo.Properties.TryGetValue("Publicity", out var publicity);

            Debug.Log($"Found session {sessionInfo.Name} playerCount {sessionInfo.PlayerCount} publicity {publicity}");

            if (publicity == "public")
            {
                if(publicSessions.Count == 0)
                {
                    sessionListUIHandler.ClearList();
                }
                sessionListUIHandler.AddToList(sessionInfo);

                publicSessions.Add(sessionInfo);
            }
        }

        if (publicSessions.Count == 0)
        {
            Debug.Log("Joined Lobby no public sessions found");

            sessionListUIHandler.OnNoSessionFound();
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        Debug.Log("New Scene Loaded");
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Loading New Scene Start");
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){ }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ }
}