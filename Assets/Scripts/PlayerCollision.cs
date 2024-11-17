using Unity.Netcode;
using UnityEngine;

public class PlayerCollision : NetworkBehaviour
{
    private HealthManager healthManager;

    private void Start()
    {
        // Lấy component HealthManager của Player
        healthManager = GetComponent<HealthManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Nếu va chạm với enemy, giảm máu
            float damage = 10f; // Mức độ sát thương có thể thay đổi
            if (healthManager != null)
            {
                healthManager.TakeDamage(damage); // Gọi phương thức giảm máu
            }
        }
    }
}
