using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [SerializeField] public float lifeTime = 5;
    [SerializeField] public int damage = 50;

    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke(nameof(DespawnSelf), lifeTime);
    }
    private void DespawnSelf()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Enemy"))
        {
            var h = other.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
            }
            else
            {
                var es = other.GetComponent<EnemyStats>();
                if (es != null)
                    es.TakeDamage(damage);
            }
            DespawnSelf();
        }
    }
}