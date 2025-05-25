using TMPro;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Weapon : NetworkBehaviour
{
    [SerializeField] public GameObject bulletPrefab;
    public Transform firePoint;
    [SerializeField] public float bulletSpeed = 20f;
    [SerializeField] public float fireRate = 0.2f;
    public float nextFireTime = 0;
    [SerializeField] public int maxBullets = 30;
    public int currentBullets = 30;
    private bool reloading = false;
    private float reloadTime = 1.5f;
    [SerializeField] TextMeshProUGUI bulletCount;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;

    void Start()
    {
        currentBullets = maxBullets;
        UpdateBulletCount();
    }

    void Update()
    {
        if (!IsOwner) return;
        if (reloading) return;
        if ((currentBullets <= 0 || Input.GetKeyDown(KeyCode.R)) && !reloading)
            StartCoroutine(Reload());
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        currentBullets--;
        UpdateBulletCountClientRpc(currentBullets);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    private void UpdateBulletCountClientRpc(int newCount)
    {
        currentBullets = newCount;
        UpdateBulletCount();
    }

    IEnumerator Reload()
    {
        reloading = true;
        audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        currentBullets = maxBullets;
        reloading = false;
        UpdateBulletCount();
    }

    void UpdateBulletCount()
    {
        if (bulletCount != null)
        {
            bulletCount.text = $"{currentBullets} / {maxBullets}";
        }
    }
}