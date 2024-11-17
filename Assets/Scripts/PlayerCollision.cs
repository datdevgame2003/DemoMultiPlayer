using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
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
            Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
            healthManager.TakeDamage(damageOnCollision);
            
        }
    }
}
