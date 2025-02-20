using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5;
    public int maxEnemies = 30;
    private int currentEnemyCount = 0;
    private EnemyAI enemyAI;

    private void Start()
    {
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        currentEnemyCount++;

        if (enemyAI != null)
        {
            enemyAI.OnEnemyDestroyed += EnemyDestroyed;
        }
    }

    private void EnemyDestroyed()
    {
        currentEnemyCount--;
    }

    //public override void OnNetworkSpawn()
    //{
    //    if(IsServer)
    //    {
    //        StartCoroutine(SpawnEnemies());
    //    }
    //    base.OnNetworkSpawn();
    //}

    //IEnumerator SpawnEnemies()
    //{
    //    yield return new WaitForSeconds(spawnInterval);

    //    if (currentEnemyCount < maxEnemies)
    //    {
    //        SpawnEnemyServerRpc();
    //    }
    //}

    //[ServerRpc]
    //void SpawnEnemyServerRpc()
    //{
    //    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
    //    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    //    enemy.GetComponent<NetworkObject>().Spawn();

    //    currentEnemyCount++;
    //}
}
