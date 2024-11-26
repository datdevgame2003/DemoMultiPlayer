
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;  
    [SerializeField] private List<Transform> spawnPoints;  
    [SerializeField] private float spawnInterval = 2f;  
    private float timeSinceLastSpawn;

    void Start()
    {
        if (IsServer)
        {
            timeSinceLastSpawn = spawnInterval; 
        }
    }

    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        timeSinceLastSpawn += Time.deltaTime;


        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (!IsServer) return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];


        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        if (enemy.GetComponent<EnemyHealth>() == null)
        {
            Debug.LogWarning("Enemy does not have a health system!");
        }

        NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); // Đồng bộ hóa đối tượng với client
        }
        else
        {
            Debug.LogError("Prefab of Enemy have not NetworkObject!");
        }

    }
}
