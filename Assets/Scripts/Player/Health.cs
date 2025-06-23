using System;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Health : NetworkBehaviour
{
    [Header("Health Settings")] [SerializeField]
    private int maxHealth = 100;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public int CurrentHealth => currentHealth.Value;
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += HandleHealthChanged;
        OnHealthChanged?.Invoke(currentHealth.Value);
        if (IsOwner && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterHealth(this);
        }
    }

    private void HandleHealthChanged(int oldValue, int newValue)
    {
        OnHealthChanged?.Invoke(newValue);

        if (newValue <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
        }
        else
        {
            TakeDamageServerRpc(damage);
        }
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