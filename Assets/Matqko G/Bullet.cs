using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] public float lifeTime = 5;
    [SerializeField] public float damage = 50f;


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }



    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }

    }



}
