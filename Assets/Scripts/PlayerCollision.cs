using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerCollision : NetworkBehaviour
{
    [SerializeField] private HealthManager healthManager;

    [SerializeField] private int damageOnCollision = 10;
    [SerializeField]
    GameObject ExplosionPrefab, HitEffectPrefab;
    

  
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
            if (collision.gameObject.CompareTag("Enemy")) //play va cham enemy tag enemy
            {
           
            GameObject hit = Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
            NetworkObject networkObject = hit.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn(); //spawn hieu ung,  // Spawn the effect so it gets synchronized across all clients
            }
            healthManager.TakeDamage(damageOnCollision); //nhan dame tu enemy
            }
    }
   
}
