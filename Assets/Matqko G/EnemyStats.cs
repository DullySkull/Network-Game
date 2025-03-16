using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    [SerializeField] public float health = 100;

    void Start()
    {
        health = 100;
    }


    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }



    void Die()
    {
        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore();
        }

        Destroy(gameObject);
    }


}
