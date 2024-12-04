using Unity.Netcode;
using UnityEngine;

public class GoldManager : NetworkBehaviour
{
    private NetworkVariable<int> goldCount = new NetworkVariable<int>(0); // Biến mạng lưu số lượng vàng

    public static GoldManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddGold(int amount)
    {
        if (!IsServer) return;

        goldCount.Value += amount;//tang vang
        UpdateGoldUIClientRpc(goldCount.Value); // Cập nhật giao diện UI cho tất cả client
    }

    [ClientRpc]
    private void UpdateGoldUIClientRpc(int newGoldCount)
    {
        GameManage.Instance.UpdateGoldUI(newGoldCount);
    }
}

