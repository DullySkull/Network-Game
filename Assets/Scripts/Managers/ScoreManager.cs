using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    private Dictionary<ulong, int> playerScores = new Dictionary<ulong, int>();

    public void AddScore(ulong playerId, int points)
    {
        if (!IsServer) return;

        if (!playerScores.ContainsKey(playerId))
        {
            playerScores[playerId] = 0;
        }

        playerScores[playerId] += points;
        UpdateScoreClientRpc(playerId, playerScores[playerId]);
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(ulong playerId, int newScore)
    {
        Debug.Log($"Player {playerId} Score: {newScore}");
    }
}
