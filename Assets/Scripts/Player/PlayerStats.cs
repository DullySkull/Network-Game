using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Elements")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    GameManager gameManager;

    private void Start()
    {
        
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        Debug.Log("Player had dieded");
        if (gameManager != null)
        {
            gameManager.GameOver();
        }

    }



    void UpdateUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }
}
