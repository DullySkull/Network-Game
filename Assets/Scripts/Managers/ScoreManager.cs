using UnityEngine;
using Unity.Netcode;
using TMPro;

[RequireComponent(typeof(NetworkObject))]
public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private NetworkVariable<int> networkScore = new NetworkVariable<int>(0);

    [Header("Health UI (Should not be here but mb)")]
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        networkScore.OnValueChanged += (oldVal, newVal) => UpdateScoreText(newVal);
        UpdateScoreText(networkScore.Value);
    }

    public void AddScore()
    {
        if (IsServer)
            networkScore.Value++;
    }

    public void RegisterHealth(Health h)
    {
        if (healthText == null)
        {
            var go = GameObject.Find("HealthText");
            healthText = go != null ? go.GetComponent<TextMeshProUGUI>() : null;
            if (healthText == null)
                Debug.LogError("ScoreManager: HealthText UI not found!");
        }

        h.OnHealthChanged += UpdateHealthText;
        UpdateHealthText(h.CurrentHealth);
    }

    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void UpdateHealthText(int health)
    {
        if (healthText != null)
            healthText.text = $"Health: {health}";
    }
}