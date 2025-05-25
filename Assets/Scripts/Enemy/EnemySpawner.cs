using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxEnemies = 30;

    private int currentEnemyCount = 0;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (currentEnemyCount < maxEnemies)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var enemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        var netObj = enemyGO.GetComponent<NetworkObject>();
        netObj.Spawn();
        currentEnemyCount++;

        var health = enemyGO.GetComponent<Health>();
        if (health != null)
            health.OnDeath += EnemyDestroyed;
    }

    private void EnemyDestroyed()
    {
        currentEnemyCount--;
        ScoreManager.Instance.AddScore();
        if (currentEnemyCount <= 0)
            GameManager.Instance.WinGame();
    }
}