using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public TMP_InputField chatInput;
    public TMP_Text chatDisplay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatInput.isFocused)
        {
            SendChatMessage();
        }
    }

    private void SendChatMessage()
    {
        string message = chatInput.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            SendChatMessageServerRpc(message, OwnerClientId);
            chatInput.text = "";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ulong senderId)
    {
        string formattedMessage = $"Player {senderId}: {message}";
        ReceiveChatMessageClientRpc(formattedMessage);
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string message)
    {
        chatDisplay.text += message + "\n";
    }
}
