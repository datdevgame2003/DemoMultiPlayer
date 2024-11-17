using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private int maxHealth = 100;  // Máu tối đa
    private int currentHealth;
    private Rigidbody2D rb;
    [SerializeField]
    GameObject HitEffectPrefab;
    private Transform target;
 
    public static event System.Action OnEnemyKilled;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;  // Khởi tạo máu
        if (!IsOwner)
        {
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);
           
        }
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
        if (!IsOwner)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Giới hạn máu trong khoảng 0 - max

        
        UpdateHealthServerRpc(currentHealth);

      
        if (currentHealth <= 0)
        {
            Die();
        }
    }

   
    void Die()
    {
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }

   
    [ServerRpc]
    private void UpdateHealthServerRpc(int newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthClientRpc(newHealth);
    }

   
    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        
    }
}
