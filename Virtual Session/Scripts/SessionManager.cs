using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace dsineiro.VirtualSession {
    public class SessionManager : MonoBehaviour
    {

        // Dictionary to keep track of active sessions
        private Dictionary<string, GameObject> activeSessions;

        // Singleton pattern on server-side
        public static SessionManager Instance { get; private set; }

        [SerializeField] private GameObject sessionprefab;

        void Awake()
        {
            // Initialize instance of it has not been created yet.
            if (Instance == null)
            {
                activeSessions = new Dictionary<string, GameObject>();
                Instance = this;
                return;
            }

            // Otherwise, destroy the new instance
            Destroy(gameObject);
        }

        public GameObject GetSession(string sessionId)
        {
            // Returns the GameObject associated with <sessionId>
            return activeSessions[sessionId];
        }

        public void AssignToSession(string session, ulong clientId)
        {
            // If the session the client is trying to join does not exist, create it
            if (!activeSessions.ContainsKey(session))
            {
                CreateGameSession(session);
            }
            
            // Fetch the Session script of the session
            var _session = activeSessions[session].GetComponent<Session>();
            // Add the client to the session
            _session.AddPlayer(clientId);
        }
        
        public void CreateGameSession(string sessionId)
        {
            // Instantiate a new session object from the Prefabs folder
            GameObject sessionContainer = Instantiate(sessionprefab);
            // Assign the name of the GameObject to the <sessionId> for easy lookup 
            sessionContainer.name = sessionId;

            // Spawn the NetworkObject associated with the session
            var netObject = sessionContainer.GetComponent<NetworkObject>();
            netObject.Spawn();

            // Add the session to the active sessions dictionary
            activeSessions.Add(sessionId, sessionContainer);

            Debug.Log($"Session {sessionId} added to active sessions.");
        }

        public void RemoveGameSession(string sessionId)
        {
            // If the session exists, remove it from the active sessions dictionart and destroy its GameObject
            if (activeSessions.TryGetValue(sessionId, out GameObject session))
            {
                activeSessions.Remove(sessionId);
                Destroy(session);

                Debug.Log($"Session {sessionId} removed from active sessions.");
            }
        }
    }
}