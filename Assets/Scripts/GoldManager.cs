using Unity.Netcode;
using UnityEngine;

public class GoldManager : NetworkBehaviour
{
    private NetworkVariable<int> goldCount = new NetworkVariable<int>(0); // Bien mang luu so luong vang

    public static GoldManager Instance;

    private void Awake() //tranh tao ra nhieu instance,chi co 1 instance 
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

        goldCount.Value += amount;//tang vang tren server
        UpdateGoldUIClientRpc(goldCount.Value); // Cap nhat giao dien UI cho tat ca client
    }

    [ClientRpc]
    private void UpdateGoldUIClientRpc(int newGoldCount) //dc goi tu server,dong bo den clients khi thay doi tren server
    {
        GameManage.Instance.UpdateGoldUI(newGoldCount); //cap nhat gia tri vang moi
    }
}

