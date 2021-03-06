using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using JamCat.Cameras;
using JamCat.Players;
using JamCat.Multiplayer;

namespace JamCat
{
    public class SysMultiplayer : NetworkBehaviour
    {
        // Static
        public static SysMultiplayer instance;
        public static SysMultiplayer Get() { return instance; }

        // Variables
        public NetworkManager networkManager;
        public UNetTransport uNetTransport;

        public MultiplayerMethods multiplayerMethods { get; private set; }

        public bool serverStarted;
        public bool clientConnectedToServer;
        private float timeConnecting;

        // Variables -> Sync
        public int curPlayers = 1, maxPlayers = 1;
        public Dictionary<ulong, bool> serverPlayersReady = new Dictionary<ulong, bool>();
        public Dictionary<ulong, int> serverCharacters = new Dictionary<ulong, int>();
        public int clientPlayersReady;


        // Methods -> Standard
        private void Awake() {
            instance = this;
        }

        private void Start() {
            
        }

        private void Update() {
            if (networkManager.IsClient == true && networkManager.IsServer == true) {
                // print("Connections: " + networkManager.ConnectedClients.Count);
                curPlayers = networkManager.ConnectedClients.Count;
            } 
        }

        // Methods -> Hosting / Connecting
        public void StartHost() {
            networkManager.StartHost();
            StartCoroutine(IE_Hosting());
        }

        IEnumerator IE_Hosting() {
            serverStarted = true;
            clientConnectedToServer = true;

            SysPlayer.Get().localPlayerObj = networkManager.ConnectedClients[0].PlayerObject.gameObject;
            SysPlayer.Get().localPlayerID = SysPlayer.Get().localPlayerObj.GetComponent<NetworkObject>().OwnerClientId;
            multiplayerMethods = SysPlayer.Get().localPlayerObj.GetComponent<MultiplayerMethods>();
            multiplayerMethods.OnStart();

            MultiplayerMethods.Get().OnConnectServerRpc();
            PlayerReady(SysPlayer.Get().localPlayerID, false, 0);
            // print ("Server Created!");
            yield return null;
        }


        public void StartClient() {
            networkManager.StartClient();
            StartCoroutine(IE_Connecting());
        }

        IEnumerator IE_Connecting() {
            timeConnecting = 0;
            while(networkManager.IsConnectedClient == false) {
                timeConnecting += Time.unscaledDeltaTime;
                if (timeConnecting > 5) {
                    // print ("Couldn't connect...");
                    StopAllCoroutines();
                }
                yield return null;
            }
            
            clientConnectedToServer = true;

            SysPlayer.Get().localPlayerObj = networkManager.LocalClient.PlayerObject.gameObject;
            SysPlayer.Get().localPlayerID = SysPlayer.Get().localPlayerObj.GetComponent<NetworkObject>().OwnerClientId;
            multiplayerMethods = SysPlayer.Get().localPlayerObj.GetComponent<MultiplayerMethods>();
            multiplayerMethods.OnStart();
            MultiplayerMethods.Get().OnConnectServerRpc();
            multiplayerMethods.OnReadyServerRpc(SysPlayer.Get().localPlayerID, false, 0);

            // print ("Connected!");
            yield return null;
        }

        // Players Ready
        public void PlayerReady(ulong id, bool state, int character) {
            if (serverPlayersReady.ContainsKey(id) == true) {
                serverPlayersReady[id] = state;
            } else {
                serverPlayersReady.Add(id, state);
            }
            bool[] playersReady = serverPlayersReady.Values.ToArray();

            if (serverCharacters.ContainsKey(id) == true) {
                serverCharacters[id] = character;
            } else {
                serverCharacters.Add(id, character);
            }
            ulong[] ids = serverCharacters.Keys.ToArray();
            int[] characters = serverCharacters.Values.ToArray();
           
            multiplayerMethods.OnReadyClientRpc(ids, characters, playersReady);
        }

        public int GetPlayersReadyCount() {
            int count = 0;
            foreach (var item in serverPlayersReady)
                if (item.Value == true)
                    count++;
            return count;
        }

        // Disconnect
        public void Disconnect() {
            // print("IsServer = " + networkManager.IsServer + ";   IsHost = " + networkManager.IsHost + ";   IsClient = " + networkManager.IsClient);
            serverCharacters = new Dictionary<ulong, int>();
            serverPlayersReady = new Dictionary<ulong, bool>();
            StopAllCoroutines();
            if (networkManager.IsServer == true) {
                if (clientConnectedToServer == true) {
                    DisconnectServer();
                }
            } else {
                DisconnectClient();
            }
        }

        private void DisconnectClient() {
            clientConnectedToServer = false;
            networkManager.Shutdown();
        }

        private void DisconnectServer() {
            serverStarted = false;
            clientConnectedToServer = false;
            clientPlayersReady = 0;
            curPlayers = 0;

            for (int i = 0; i < networkManager.ConnectedClients.Count; i++)
                networkManager.DisconnectClient(networkManager.ConnectedClientsIds[i]);

            networkManager.Shutdown();
        }
    }
} 