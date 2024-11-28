using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerCollision : NetworkBehaviour
{
    [SerializeField] private HealthManager healthManager;

    [SerializeField] private int damageOnCollision = 10;
    [SerializeField]
    GameObject ExplosionPrefab, HitEffectPrefab;
    //AudioSource coinSound;

    [SerializeField]
    List<AudioClip> listAudios;
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
            if (collision.gameObject.CompareTag("Enemy")) //play va cham enemy tag enemy
            {
            GameObject hit = Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);

            // Get the NetworkObject component attached to the explosion prefab
            NetworkObject networkObject = hit.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Spawn the effect so it gets synchronized across all clients
                networkObject.Spawn(); //spawn hieu ung 
            }
            healthManager.TakeDamage(damageOnCollision); //nhan dame tu enemy
            }
    }
}
