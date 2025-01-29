using Unity.Netcode;
using UnityEngine;

namespace dsineiro.VirtualSession {
    public class ServerManager : MonoBehaviour
    {

        [SerializeField] private Canvas canvas;

        public void StartServer()
        {
            HideCanvas();
            NetworkManager.Singleton.StartServer();
        }

        public void StartHost()
        {
            HideCanvas();
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            HideCanvas();
            NetworkManager.Singleton.StartClient();
        }

        void HideCanvas()
        {
            canvas.enabled = false;
        }
    }
}
