using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    private Transform target;

    void Start()
    {
        if (!IsServer) // Chỉ Server xử lý logic tìm mục tiêu
        {
            return;
        }

        // Tìm tất cả các Player trong game
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            target = GetClosestPlayer(players).transform; // Chọn mục tiêu gần nhất
        }
    }

    void Update()
    {
        if (!IsServer) // Chỉ Server điều khiển Enemy
        {
            return;
        }

        if (target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);

        // Gửi vị trí mới đến Client
        UpdatePositionClientRpc(transform.position);
    }

    // ClientRpc: Đồng bộ vị trí từ Server đến Client
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        if (IsServer) // Server không cần cập nhật từ ClientRpc
        {
            return;
        }

        transform.position = newPosition;
    }

    private GameObject GetClosestPlayer(GameObject[] players)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player;
            }
        }

        return closest;
    }
}
