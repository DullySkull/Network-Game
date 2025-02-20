using ParrelSync.NonCore;
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    public event Action OnEnemyDestroyed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (agent != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (other.CompareTag("Player"))
        {
            if(playerStats != null)
            {
                playerStats.TakeDamage(10);
            }
        }
    }

    private void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke();
    }
}
