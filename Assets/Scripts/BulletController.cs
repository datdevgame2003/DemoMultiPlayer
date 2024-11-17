using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
   
    private Vector2 direction;

    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là kẻ thù không
        if (other.CompareTag("Enemy"))
        {
            // Truy cập script EnemyHealth và gọi phương thức TakeDamage()
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(10);  // Gọi hàm giảm máu của enemy
            }

            // Hủy bullet sau khi va chạm
            Destroy(gameObject);
        }
    }

}
