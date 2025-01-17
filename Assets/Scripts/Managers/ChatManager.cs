using Unity.Netcode;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public void SendMessage(string message)
    {
        SendMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(string message)
    {
        Debug.Log($"Message from client: {message}");
        SendMessageClientRpc(message);
    }

    [ClientRpc]
    private void SendMessageClientRpc(string message)
    {
        Debug.Log($"Chat: {message}");
    }
}
