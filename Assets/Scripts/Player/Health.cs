using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public delegate void DeathAction();
    public event DeathAction OnDeath;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += (oldV, newV) =>
        {
            if (newV <= 0) Die();
        };
    }

    // Called locally (e.g. from bullet collision)
    public void TakeDamage(int damage)
    {
        if (IsServer)
            currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
        else
            TakeDamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int damage)
    {
        currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        if (IsServer)
            GetComponent<NetworkObject>().Despawn();
    }
}