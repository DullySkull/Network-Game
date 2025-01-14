using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private int damage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                DestroyBullet();
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamageServerRpc(damage);
            }
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        NotifyShooterClientRpc();

        Destroy(gameObject);
    }

    [ClientRpc]
    private void NotifyShooterClientRpc()
    {
        var shooter = FindObjectOfType<Weapon>();
        if (shooter != null)
        {
            shooter.DecrementBulletCount();
        }
    }
}
