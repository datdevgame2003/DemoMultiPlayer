using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GoldPickup : NetworkBehaviour
{
    [SerializeField] private int goldValue = 1;
    [SerializeField]
    AudioSource coinSound;
  
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            if (IsServer)
            {
                PlayPickupSoundClientRpc();//am thanh -> all clients
                GoldManager.Instance.AddGold(goldValue);//update gold
                NetworkObject.Despawn(); //xoa vang sau khi nhat
            }
        }
    }
    [ClientRpc]
    private void PlayPickupSoundClientRpc() //goi tu server phat am thanh -> all clients
    {
  
        coinSound.Play();
    }
}




