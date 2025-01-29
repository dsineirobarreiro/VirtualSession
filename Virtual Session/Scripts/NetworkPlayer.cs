using Unity.Netcode;
using UnityEngine;

namespace dsineiro.VirtualSession {
    public class NetworkPlayer : NetworkBehaviour
    {

        // Custom properties could be added here

        // public string Username { get; provate set; }
        // public int Score { get; private set; }


        public override void OnNetworkSpawn()
        {
            Debug.Log("NetworkPlayer spawning. Owner: " + OwnerClientId + ", Client: " + NetworkManager.LocalClientId);

            if (IsOwner)
            {
                //Owner-side code

                // Request the server for assignment to the corresponding session.
                string session = "default";             // This value should be passed from user input or other source
                AssignToSessionServerRpc(session);
            }
            else if (IsServer)
            {
                // Server-side code

                // The server handles visibility checks and should subscribe when spawned locally on the server-side.
                // This allows the player to see himself, which by default spawns with no observers.
                NetworkObject.CheckObjectVisibility += CheckVisibility;
            }

            base.OnNetworkSpawn();
        }

        private bool CheckVisibility(ulong clientId)
        {
            // If not spawned, then always return false
            if (!IsSpawned)
            {
                return false;
            }

            // Return true if the client running the code is the owner of the object
            return OwnerClientId == clientId;
        }

        public void UnsubscribeVisibility()
        {
            // Reset the visibility check delegate
            NetworkObject.CheckObjectVisibility -= CheckVisibility;
        }

        [ServerRpc]
        void AssignToSessionServerRpc(string session, ServerRpcParams serverRpcParams = default)
        {
            // Asigna a la sesión al cliente que solicita
            SessionManager.Instance.AssignToSession(session, serverRpcParams.Receive.SenderClientId);
        }
    }
}