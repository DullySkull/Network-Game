using Unity.Netcode;
using TMPro;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")] 
    public int score = 0;
    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    private NetworkVariable<int> networkScore = new NetworkVariable<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        networkScore.OnValueChanged += (oldVal, newVal) => UpdateScoreText(newVal);
        UpdateScoreText(networkScore.Value);
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + newScore;
    }

    public void AddScore()
    {
        if (IsServer)
            networkScore.Value++;
        else
            AddScoreServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddScoreServerRpc()
    {
        networkScore.Value++;
    }
}