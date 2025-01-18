using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;

    public string playerName;

    void Awake() 
    { 
        ChatManager.Singleton = this; 
    }

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
    }

    public void SendChatMessage(string chatmessage, string playerID = null)
    { 
        if(string.IsNullOrEmpty(chatmessage)) return;

        string sent = playerID + ": " +  chatmessage;
        SendChatMessageServerRpc(sent); 
    }
   
    void AddMessage(string msg)
    {
        ChatMessage chatMessage = Instantiate(chatMessagePrefab);
        chatMessage.SetText(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message)
    {
        ChatManager.Singleton.AddMessage(message);
    }
}
