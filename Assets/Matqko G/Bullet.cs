using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [SerializeField] public float lifeTime = 5;
    [SerializeField] public int damage = 50;

    private void Start()
    {
        if (IsServer)
            Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Enemy"))
        {
            var h = other.GetComponent<Health>();
            if (h != null) h.TakeDamage(damage);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}

    /*void OnTriggerEnter(Collider other)
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
    }*/