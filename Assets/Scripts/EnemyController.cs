using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    
  
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
                target = player.transform;
            }
        }
    }

    void Update()
    {
        if (!IsServer || target == null)
        {
            return;
        }

            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
            TakeDamage(10);
        }
    }

    // Hàm giảm máu
    void TakeDamage(int damage)
    {
        

        if (TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
        }
    }


  
}