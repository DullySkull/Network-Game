using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;
    public Transform firePoint;
    [SerializeField] public float bulletSpeed = 15.0f;
    public float fireRate = 0.3f;
    public float nextFireTime = 0;




    void Update()
    {
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

    }
}
