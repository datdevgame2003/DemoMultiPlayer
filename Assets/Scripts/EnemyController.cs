using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private EnemySpawner enemySpawner;
    private Rigidbody2D rb;
    [SerializeField]
    GameObject HitEffectPrefab;
    private Transform target;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (IsServer)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform; // di chuyen duoi theo player co tag Player
            }
        }
    }

    void Update()
    {
        if (!IsServer || target == null) //khong move tren server hoac khong co muc tieu
        {
            return; //chi kiem soat tren server
        }
        //move tren server va co muc tieu
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);

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
      
        if (TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
        }
    }



}