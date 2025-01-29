using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace dsineiro.VirtualSession {
    public class Session : NetworkBehaviour
    {
        public string SessionId { get; private set; }
        public NetworkList<ulong> Players = new NetworkList<ulong>(new List<ulong>());
        public NetworkList<NetworkKeyValuePair> UsernameToId = new();

        
        void Awake()
        {
            // Suscribe to changes on the Players list
            Players.OnListChanged += PlayersChanged;

            // Initialize to empty a dictionary that will store the username to id mapping
            UsernameToId = new();
        }
        
        public void Initialize(string sessionId)
        {
            // Sets the session id
            SessionId = sessionId;

            // Custom session properties could be set below
            // MaxPlayers = 4, for example
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                // Client-side code

                // Request to parent the client to the session object (for convenience sake)
                ParentClientToSessionObjectServerRpc();

                // Loop through current clients in the session and show them to the new client (clients are spawned with no observers by default)
                foreach (var item in Players)
                {
                    if (item != NetworkManager.LocalClientId) ShowClientInSessionServerRpc(NetworkManager.LocalClientId, item);
                }
            }

            base.OnNetworkSpawn();
        }

        [ServerRpc(RequireOwnership = false)]
        void ShowClientInSessionServerRpc(ulong clientObjectId, ulong clientIdToShow)
        {
            // Make visible the NetworkObject associated with <clientObjectId> to <clientIdToShow>
            List<NetworkObject> clientObject = new(){NetworkManager.ConnectedClients[clientObjectId].PlayerObject};
            NetworkObject.NetworkShow(clientObject, clientIdToShow);
        }

        void PlayersChanged(NetworkListEvent<ulong> changeEvent)
        {
            // When a changed is made to the Players list, an action is performed depending on the type of change

            switch (changeEvent.Type)
            {
                case NetworkListEvent<ulong>.EventType.Add:

                    int index = changeEvent.Index;          // Position of the added value in the list
                    ulong clientId = changeEvent.Value;     // The new value at the index position

                    if (IsClient)
                    {
                        // Client-side code

                        // Make visible the new client to the rest of the clients in the session, as this code is run on every current client on the session
                        ShowClientInSessionServerRpc(NetworkManager.LocalClientId, clientId);
                    }

                    Debug.Log($"Client {clientId} at index {index}");

                    break;

                // Rest of possible events in case of future customization
                case NetworkListEvent<ulong>.EventType.Insert:
                    break;
                case NetworkListEvent<ulong>.EventType.Remove:
                    break;
                case NetworkListEvent<ulong>.EventType.RemoveAt:
                    break;
                case NetworkListEvent<ulong>.EventType.Value:
                    break;
                case NetworkListEvent<ulong>.EventType.Clear:
                    break;
                case NetworkListEvent<ulong>.EventType.Full:
                    break;
                default:
                    break;
            }
        }

        public void EndSession()
        {
            // Clean resources and destroy the session object
            // The current implementation is naive and incomplete, as it depends on the game logic

            Debug.Log($"Session {SessionId} terminated.");

            Destroy(gameObject);
        }

        public void AddPlayer(ulong clientId)
        {
            // When a player is added to the session, make the session visible to the new client (session are spawned with no observers by default)
            NetworkObject netObject = GetComponent<NetworkObject>();
            netObject.NetworkShow(clientId);

            // Add the client to the Players list
            Players.Add(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        void ParentClientToSessionObjectServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug.Log($"Cliente {serverRpcParams.Receive.SenderClientId} assigned to the session {SessionId}");

            // Fetches the NetworkObject of the client requesting session parenting and sets that client as a child of the session.
            NetworkObject client = NetworkManager.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject;
            client.transform.SetParent(transform, false);

            // Every time NetworkShow is called, CheckVisibility is called so it is reset to work by default,
            // returning true when NetworkShow is called and false for NetworkHide. As each client is spawned
            // with no observers but its own, we need to reset to default behavior to manage session visibility
            client.GetComponent<NetworkPlayer>().UnsubscribeVisibility();
        }

        [ServerRpc(RequireOwnership = false)]
        public void UsernameToIdServerRpc(string username, ulong clientId)
        {
            UsernameToId.Add(new NetworkKeyValuePair(username, clientId));
        }
    }

    // This class allows to pass a dictionary as a NetworkVariable. It is used for the UsernameToId
    public struct NetworkKeyValuePair : INetworkSerializable, IEquatable<NetworkKeyValuePair>
    {
        public FixedString32Bytes Key;
        public ulong Value;

        public NetworkKeyValuePair(FixedString32Bytes key, ulong value)
        {
            Key = key;
            Value = value;
        }

        public bool Equals(NetworkKeyValuePair other)
        {
            return Key.Equals(other.Key) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkKeyValuePair other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Key);
            serializer.SerializeValue(ref Value);
        }
    }
}
