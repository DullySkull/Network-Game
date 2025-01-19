using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Tagged : NetworkBehaviour
{
    public NetworkVariable<ulong> ItPlayerId = new NetworkVariable<ulong>();
    private bool isOnCooldown = false;
    private float lastTagTime = -1f;

    private void Start()
    {
        // Debug: Register a callback to track ItPlayerId changes
        ItPlayerId.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"[Tagged] ItPlayerId changed: Old = {oldValue}, New = {newValue}");
            UpdatePlayerColorsClientRpc(newValue);
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        // Debug: Log collision details
        Debug.Log($"[Tagged] Collision detected: {gameObject.name} collided with {collision.gameObject.name}");

        // Ensure the current player is "It" and not on cooldown
        if (ItPlayerId.Value != OwnerClientId)
        {
            Debug.Log($"[Tagged] This player is not 'It'. Current 'It': {ItPlayerId.Value}");
            return;
        }

        if (isOnCooldown)
        {
            Debug.Log("[Tagged] Cooldown active. Cannot tag yet.");
            return;
        }

        // Check if the collided object is another player
        NetworkObject otherPlayer = collision.gameObject.GetComponent<NetworkObject>();
        if (otherPlayer != null && otherPlayer.OwnerClientId != OwnerClientId)
        {
            ItPlayerId.Value = otherPlayer.OwnerClientId;
            Debug.Log($"[Tagged] Tagging player {otherPlayer.OwnerClientId}");
            UpdatePlayerColorsClientRpc(ItPlayerId.Value);
            StartCoroutine(StartCooldown());
            lastTagTime = Time.time;
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
                    renderer.material.color = Color.red;
                }
                else
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(3f);
        isOnCooldown = false;
        Debug.Log("[Tagged] Cooldown ended. Player can tag again.");
    }
}