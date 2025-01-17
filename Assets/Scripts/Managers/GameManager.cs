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

    public void StartGame()
    {
        if (IsServer)
        {
            ulong randomPlayerId = NetworkManager.Singleton.ConnectedClientsIds[Random.Range(0, NetworkManager.Singleton.ConnectedClientsIds.Count)];
            TaggedPlayerId.Value = randomPlayerId;
        }

        if (IsClient)
        {
            countdownTextObject.SetActive(true);
        }

        StartCoroutine(StartGameCountdown());
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
}
