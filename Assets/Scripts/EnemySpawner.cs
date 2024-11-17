
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;  // Prefab của Enemy
    [SerializeField] private List<Transform> spawnPoints;  // Các điểm spawn ngẫu nhiên
    [SerializeField] private float spawnInterval = 2f;  // Thời gian giữa mỗi lần spawn kẻ thù
    private float timeSinceLastSpawn;

    void Start()
    {
        if (IsServer)
        {
            timeSinceLastSpawn = spawnInterval; // Khởi tạo thời gian spawn
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

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];


        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
        networkObject.Spawn(); // Spawn trên mạng
    }
}
