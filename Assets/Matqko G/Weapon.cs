using TMPro;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    //[SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] public float bulletSpeed = 20f;
    [SerializeField] public float fireRate = 0.2f;
    public float nextFireTime = 0;
    [SerializeField] public int maxBullets = 30;
    public int currentBullets = 30;
    private bool reloading = false;
    private float reloadTime = 1.5f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxAimDistance = 1000f;
    //[SerializeField] TextMeshProUGUI bulletCount;
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

            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = playerCamera.ScreenPointToRay(screenCenter);

            Vector3 aimPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
                aimPoint = hit.point;
            else
                aimPoint = ray.GetPoint(maxAimDistance);

            ShootServerRpc(aimPoint);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 aimPoint, ServerRpcParams rpcParams = default)
    {
        currentBullets--;
        UpdateBulletCountClientRpc(currentBullets);

        Vector3 spawnPos = firePoint.position;
        Vector3 direction = (aimPoint - spawnPos).normalized;
        Quaternion rot = Quaternion.LookRotation(direction);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);
        bullet.GetComponent<NetworkObject>().Spawn();

        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        audioSource.PlayOneShot(shootSound);
    }

   //[ServerRpc]
   //private void EquipWeaponServerRpc(ServerRpcParams rpcParams = default)
   //{
   //    var weaponInstance = Instantiate(weaponPrefab, transform);
   //    var newObj = weaponInstance.GetComponent<NetworkObject>();
   //    newObj.Spawn();
   //}

    [ClientRpc]
    private void UpdateBulletCountClientRpc(int newCount)
    {
        currentBullets = newCount;
        UpdateBulletCount();
    }

    //public override void OnNetworkSpawn()
    //{
    //    if (IsOwner)
    //        EquipWeaponServerRpc();
    //}

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
        /*if (bulletCount != null)
            bulletCount.text = $"{currentBullets} / {maxBullets}";*/
    }
}