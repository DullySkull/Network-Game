using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        clientButton.onClick.AddListener(OnClientClicked);
        serverButton.onClick.AddListener(OnServerClicked);
    }

    private void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void OnClientClicked()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnServerClicked()
    {
        NetworkManager.Singleton.StartServer();
    }
}
