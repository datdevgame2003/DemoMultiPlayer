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
       
            if (collision.gameObject.CompareTag("Enemy"))
            {
            GameObject hit = Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
            NetworkObject networkObject = hit.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            healthManager.TakeDamage(damageOnCollision);
            }
    }
}
