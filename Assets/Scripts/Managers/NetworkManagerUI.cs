using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    public void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void OnClientClicked()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void OnServerClicked()
    {
        NetworkManager.Singleton.StartServer();
    }
    
    public void OnDisconnectClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
