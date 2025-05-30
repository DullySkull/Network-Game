using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    [Header("UI Elements")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    private void Start()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;
        currentHealth.OnValueChanged += (_, newVal) => UpdateUI(newVal);
        UpdateUI(currentHealth.Value);
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
            ApplyDamage(damage);
        else
            TakeDamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int damage) => ApplyDamage(damage);

    private void ApplyDamage(int damage)
    {
        currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
        if (currentHealth.Value == 0)
            GameManager.Instance.GameOver();
    }

    private void UpdateUI(int newHealth)
    {
        //healthBar.value = (float)newHealth / maxHealth;
        //healthText.text = $"Health: {newHealth}/{maxHealth}";
    }
}