using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerSync : NetworkBehaviour
{
    private Vector3 serverPosition;

    private void Update()
    {
        if (IsOwner)
        {
            UpdatePositionServerRpc(transform.position);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * 10f);
        }
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 position)
    {
        UpdatePositionClientRpc(position);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        serverPosition = position;
    }
}
