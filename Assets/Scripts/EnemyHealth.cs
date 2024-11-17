using Unity.Netcode;
using UnityEngine;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField]
    GameObject ExplosionPrefab;
    void Start()
    {
        currentHealth = maxHealth;
    }

    // giảm máu
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Giới hạn máu trong khoảng 0 - max

        if (currentHealth <= 0)
        {
            Die();  // Kẻ thù chết
        }
    }

    // Hủy đối tượng khi chết
    void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
