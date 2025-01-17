using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Tagged : NetworkBehaviour
{
    public NetworkVariable<ulong> TaggedPlayerId = new NetworkVariable<ulong>();
    private Renderer playerRenderer;
    public Material defaultMaterial;
    public Material taggedMaterial;
    public GameObject emoteEffectPrefab;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerRenderer = GetComponentInChildren<Renderer>();
        TaggedPlayerId.OnValueChanged += OnTaggedPlayerChanged;
    }

    private void OnTaggedPlayerChanged(ulong oldValue, ulong newValue)
    {
        if (newValue == OwnerClientId)
        {
            UpdateMaterialClientRpc(true);

            if (emoteEffectPrefab)
            {
                Instantiate(emoteEffectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            }
        }
        else
        {
            UpdateMaterialClientRpc(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!gameManager.GameStarted.Value || !IsServer || TaggedPlayerId.Value != OwnerClientId) return;

        NetworkObject otherPlayer = collision.gameObject.GetComponent<NetworkObject>();
        if (otherPlayer != null && otherPlayer.OwnerClientId != OwnerClientId)
        {
            TaggedPlayerId.Value = otherPlayer.OwnerClientId;
        }
    }

    [ClientRpc]
    private void UpdateMaterialClientRpc(bool isIt)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material = isIt ? taggedMaterial : defaultMaterial;
        }
    }
}
