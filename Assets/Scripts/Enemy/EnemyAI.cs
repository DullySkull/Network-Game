using ParrelSync.NonCore;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : NetworkBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    public event Action OnEnemyDestroyed;

    void Start()
    {
        if (!IsServer) return;
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!IsServer) {  return; }
        if (agent != null)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.Log("Agent is null");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            return;
        }
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
