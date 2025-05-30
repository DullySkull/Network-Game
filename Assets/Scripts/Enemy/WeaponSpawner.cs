using UnityEngine;
using Unity.Netcode;
using UnityEditor.SceneManagement;

public class WeaponSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject weaponPrefab;
    [SerializeField] private Transform weaponMountPoint;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SpawnWeaponOnServer();
    }

    private void SpawnWeaponOnServer()
    {
        var weaponInstance = Instantiate(
            weaponPrefab,
            weaponMountPoint.position,
            weaponMountPoint.rotation);

        weaponInstance.transform.SetParent(weaponMountPoint, worldPositionStays: false);
        weaponInstance.SpawnWithOwnership(OwnerClientId);
    }
}