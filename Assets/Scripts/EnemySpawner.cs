﻿
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Enemy Spawning Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private List<Transform> spawnPoints;  // tham chieu diem spawn
    [SerializeField] private float spawnInterval = 4f; //thoi gian moi lan spawn
    [SerializeField] private int maxEnemies = 50; //so luong spawn toi da
    private List<NetworkObject> activeEnemies = new List<NetworkObject>();  //danh sach enemies da spawn
    private float spawnTimer = 0f;

    private void Update()
    {
        if (!IsServer) return;//server spawn

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

        CleanUpDestroyedEnemies();
    }

    private void SpawnEnemy()
    {
        if (!IsServer) return;

       //random spawn
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // create enemy
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        
        NetworkObject networkObject = enemyInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); // Đong bo voi tat ca client
            activeEnemies.Add(networkObject); // Them vao danh sach quan ly
        }
        else
        {
            Debug.LogError("Prefab cua enemy khong co NetworkObject!");
        }
    }
    public void SpawnGold(Vector3 position)
    {
        if (!IsServer) return; //chi server moi spawn

       
        GameObject gold = Instantiate(goldPrefab, position, Quaternion.identity);
        NetworkObject networkObject = gold.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }
    private void CleanUpDestroyedEnemies() //xoa cac quai da bi chet khoi danh sach
    {
        
        activeEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
    }
}
