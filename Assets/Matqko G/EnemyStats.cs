using UnityEngine;
using Unity.Netcode;

public class EnemyStats : NetworkBehaviour
{[SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public delegate void DeathAction();
    public event DeathAction OnDeath;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += (oldVal, newVal) =>
        {
            if (newVal <= 0)
                Die();
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
    }

    // call this from your bullet/hit logic
    public void TakeDamage(int damage)
    {
        if (IsServer)
            currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
        else
            TakeDamageServerRpc(damage);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        if (IsServer)
            GetComponent<NetworkObject>().Despawn();
        ScoreManager.Instance.AddScore();
    }
}
