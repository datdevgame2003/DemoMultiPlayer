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
                PlayPickupSoundClientRpc();
                GoldManager.Instance.AddGold(goldValue);
                NetworkObject.Despawn(); 
            }
        }
    }
    [ClientRpc]
    private void PlayPickupSoundClientRpc()
    {
  
        coinSound.Play();
    }
}




