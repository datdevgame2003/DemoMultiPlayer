using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 1.5f;
   
    private Rigidbody2D rb;
    [SerializeField]
    GameObject HitEffectPrefab;
    private Transform target;
    private Vector2 movement; 

    private Vector3 originalScale;//luu scale goc

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);//quay mat ben trai
        //chi server tim player
        if (IsServer)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform; // di chuyen duoi theo player co tag Player
            }
        }
    }

   private void Update()
    {
        if (!IsServer || target == null) 
        {
            return; //chi kiem soat tren server
        }
        //move tren server va co muc tieu
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);
        //xoay dua tren huong di chuyen cua player
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Quay mặt sang phải
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Quay mặt sang trái
        }
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet")) //enemy va cham voi tag bullet
        {
            GameObject explosion = Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);//hieu ung nhan sat thuong

            // Get the NetworkObject component attached to the explosion prefab
            NetworkObject networkObject = explosion.GetComponent<NetworkObject>(); //khoi tao doi tuong mang hieu ung no
            if (networkObject != null)
            {
                // Spawn the effect so it gets synchronized across all clients
                networkObject.Spawn();
            }
            
           //chiu 10 dame
            TakeDamage(10);
        }
    }

    // Ham giam mau
    private void TakeDamage(int damage)
    {
      
        if (TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth)) //truy xuat component
        {
            enemyHealth.TakeDamage(damage);
        }
    }



}