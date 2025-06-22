using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EmoteController : NetworkBehaviour
{
    public GameObject emoteObject;
    public float emoteDuration = 5f;
    public float cooldownDuration = 2f;

    private bool canEmote = true;

    private void Start()
    {
        if (emoteObject != null)
            emoteObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T) && canEmote)
        {
            TriggerEmoteServerRpc();
        }
    }

    [ServerRpc]
    private void TriggerEmoteServerRpc(ServerRpcParams rpcParams = default)
    {
        TriggerEmoteClientRpc();
    }

    [ClientRpc]
    private void TriggerEmoteClientRpc(ClientRpcParams rpcParams = default)
    {
        if (emoteObject != null)
        {
            StartCoroutine(EmoteRoutine());
        }
    }

    private IEnumerator EmoteRoutine()
    {
        canEmote = false;
        emoteObject.SetActive(true);
        yield return new WaitForSeconds(emoteDuration);
        emoteObject.SetActive(false);
        yield return new WaitForSeconds(cooldownDuration);
        canEmote = true;
    }
}