using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    private NetworkVariable<int> playerScore = new NetworkVariable<int>();

    public void AddScore()
    {
        if (IsServer)
        {
            playerScore.Value++;
        }
    }

    public int GetScore()
    {
        return playerScore.Value;
    }
}
