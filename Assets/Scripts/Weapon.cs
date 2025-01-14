using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int maxBullets = 25;
    private int currentBulletCount = 0;
    private bool isReloading = false;

    private void Start()
    {
        currentBulletCount = maxBullets;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            if (currentBulletCount > 0)
            {
                ShootServerRpc();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawn.forward * 10f;

        DecrementBulletCountClientRpc();
    }

    [ClientRpc]
    private void DecrementBulletCountClientRpc()
    {
        currentBulletCount--;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(2f);
        ReloadBulletsClientRpc();
        isReloading = false;
    }

    [ClientRpc]
    private void ReloadBulletsClientRpc()
    {
        currentBulletCount = maxBullets;
    }

    public void DecrementBulletCount()
    {
        currentBulletCount--;
    }
}