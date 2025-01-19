using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject startButton;
    public GameObject countdownTextObject;

    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(false);
    public NetworkVariable<ulong> TaggedPlayerId = new NetworkVariable<ulong>();
    private bool isOnCooldown = false;

    private void Start()
    {
        if (IsServer)
        {
            TaggedPlayerId.OnValueChanged += OnTaggedPlayerChanged;
        }
    }

    public void StartGame()
    {
        if (IsServer)
        {
            // Randomly select a player to be "It"
            ulong randomPlayerId = NetworkManager.Singleton.ConnectedClientsIds[
                Random.Range(0, NetworkManager.Singleton.ConnectedClientsIds.Count)
            ];
            TaggedPlayerId.Value = randomPlayerId;

            StartCoroutine(StartGameCountdown());
        }
        else
        {
            RequestStartGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestStartGameServerRpc()
    {
        StartGame();
    }

    private IEnumerator StartGameCountdown()
    {
        float countdown = 5f;

        while (countdown > 0)
        {
            UpdateCountdownClientRpc(Mathf.CeilToInt(countdown));
            countdown -= Time.deltaTime;
            yield return null;
        }

        GameStarted.Value = true;

        UpdateCountdownClientRpc(0);
        countdownTextObject.SetActive(false);
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(int countdownValue)
    {
        countdownText.text = countdownValue > 0 ? $"Game starts in: {countdownValue}" : "";
    }

    private void OnTaggedPlayerChanged(ulong oldValue, ulong newValue)
    {
        Debug.Log($"[GameManager] Tagged player changed: Old = {oldValue}, New = {newValue}");
        UpdatePlayerColorsClientRpc(newValue);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || !GameStarted.Value) return;

        // Ensure the current player is "It" and not on cooldown
        if (TaggedPlayerId.Value != OwnerClientId || isOnCooldown) return;

        // Check if the collided object is another player
        NetworkObject otherPlayer = collision.gameObject.GetComponent<NetworkObject>();
        if (otherPlayer != null && otherPlayer.OwnerClientId != OwnerClientId)
        {
            TaggedPlayerId.Value = otherPlayer.OwnerClientId;
            Debug.Log($"[GameManager] Player {otherPlayer.OwnerClientId} is now 'It'!");

            // Start cooldown for tagging
            StartCoroutine(StartCooldown());
        }
    }

    [ClientRpc]
    private void UpdatePlayerColorsClientRpc(ulong itPlayerId)
    {
        foreach (var player in FindObjectsOfType<Movement>())
        {
            Renderer renderer = player.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                if (player.OwnerClientId == itPlayerId)
                {
                    renderer.material.color = Color.red; // "It" player
                }
                else
                {
                    renderer.material.color = Color.white; // Non-"It" players
                }
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(3f);
        isOnCooldown = false;
        Debug.Log("[GameManager] Cooldown ended. Player can tag again.");
    }
}
