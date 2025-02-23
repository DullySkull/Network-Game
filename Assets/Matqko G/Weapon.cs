using TMPro;
using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
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

        if (reloading)
            return;

        if (currentBullets <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }


        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)

        {
            Shoot();

            nextFireTime = Time.time + fireRate;

        }
    }



    void Shoot()
    {

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 targetPoint;
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }


        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        currentBullets -= 1;
        UpdateBulletCount();

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }


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
