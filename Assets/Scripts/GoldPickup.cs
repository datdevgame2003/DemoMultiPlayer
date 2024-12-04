using System;
using Unity.Netcode;
using UnityEngine;

public class GoldPickup : NetworkBehaviour
{
    [SerializeField] private int goldValue = 1; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (IsServer)
            {
                GoldManager.Instance.AddGold(goldValue);
                NetworkObject.Despawn(); 
            }
        }
    }
}




