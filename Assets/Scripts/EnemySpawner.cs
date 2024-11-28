
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
        if (!IsServer || spawnPoints.Count == 0) return;

      
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        
        if (enemy.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
        {
            networkObject.Spawn(); 
        }
        else
        {
            Debug.LogError("Prefab cua Enemy khong co NetworkObject!");
        }
    }

}

